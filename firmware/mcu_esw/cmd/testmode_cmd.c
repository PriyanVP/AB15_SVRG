/**********************************************************************************************************************
 * \file testmode_cmd.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Ifx_Types.h"
#include "Ifx_Ssw_Compilers.h"
#include "common/global_defines.h"
#include "common/spi_data_types.h"
#include "common/watchdog_types.h"
#include "common/bit_manipulation.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "periphery/timer.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "testmode_cmd.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define FLM_POWERSTAGE_GUARD_TIMEOUT     (9)   /** \brief If powerstage test is not finished after 
                                                    3 (3*3) expected durations - skip test */

#define FLM_POWERSTAGE_STEP_DURATION     (1)   /** \brief Expected duration of one step in powerstage test. 
                                                    Expected to have 3 steps for completion for 1 channel */

#define FLM_POWERSTAGE_CHANNELS_COUNT    (20)  /** \brief Number of channels for powerstage test */

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Defines error codes
 */
typedef enum
{
    FLM_PST_ERR_SPI_FAIL    = 0,        /** \brief error during SPI transaction received */
    FLM_PST_ERR_CH_EN_OFF   = 1,        /** \brief expected channels are not enabled */
    FLM_PST_ERR_LSENQ_HIGH  = 2,        /** \brief DIS_ALP/LSENQ pin is high */
    FLM_PST_ERR_DIAG_ACTIVE = 3,        /** \brief flm_diag_active is set for PST test */
    FLM_PST_ERR_PARITY_FAIL = 4         /** \brief parity fail - configuration issue */
} PstErrStateEnum;

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Union for powerstage test result
 */
typedef union
{
    struct
    {
        uint32 pst_not_valid       : 1;    /** \brief powerstage test not valid */
        uint32 pst_pretest_s2x_err : 1;    /** \brief short detected before test activation */
        uint32 pst_test_s2x_err    : 1;    /** \brief short detected after test activation */
        uint32 pst_timeout_err     : 1;    /** \brief powerstage timeout limit reached, test canceled */
        uint32 test_guard_fail     : 1;    /** \brief guard failed - diag_ready bit is not set for too long */
        uint32 spi_on_ch_fail      : 1;    /** \brief spi communication on channel failed */
        uint32 unused              : 3;    /** \brief unused bits */
    } bf;
    uint8 dw;
} PstChannelTestResult;

/** \brief Structure for Watchdog configuration
 */
typedef struct
{
    uint8   msg_id;                           /** \brief message id for sending response */
    boolean hsPowerstageEn;                   /** \brief highside powerstage test enabled */
    boolean lsPowerstageEn;                   /** \brief lowside powerstage test enabled */
    boolean isTestSeqFinished;                /** \brief true if channel test sequence was finished */
    uint8   channelIndex;                     /** \brief index of currently tested channel */
    sint8   guardCounter;                     /** \brief guard variable to check if diagnostics finished in time */
} PstRuntimeConfiguration;

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Local static variable to store powerstage test configuration
 */
static PstRuntimeConfiguration g_pstConfiguration;

/** \brief Local static variable to store powerstage test results
 */
static PstChannelTestResult g_pstResults[FLM_POWERSTAGE_CHANNELS_COUNT];

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Function to start test mode 1 or 2
 * \param isTestMode1 flag telling if test mode 1 activation was requested
 * \param USBReceiveData package with command
 * \return Returns nothing
 */
IFX_INLINE void StartTestMode(boolean isTestMode1, USBReceiveData const * const commandPackage);

/** \brief Procedure to get address and data for firing in PST
 * \param p_regAddres pointer to address of register to execute firing
 * \param p_regData pointer to data for register to execute PST for channel
 * \return Returns nothing
 */
void GetPSTFiringSPiCommand(uint16 *p_regAddress, uint16 *p_regData);

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdStartTestMode1(USBReceiveData const * const commandPackage)
{
    StartTestMode(TRUE, commandPackage);
}

void CmdStartTestMode2(USBReceiveData const * const commandPackage)
{
    StartTestMode(FALSE, commandPackage);
}

void CmdStopTestMode12(USBReceiveData const * const commandPackage)
{
    // Turn off Test mode interrupt of MCU
    DisableTestModeInterrupt();

    // Reset internal flags
    g_pstConfiguration.lsPowerstageEn = FALSE;
    g_pstConfiguration.hsPowerstageEn = FALSE;

    // Lock high and lowsides
    QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_UNLOCK, RESET_VAL_FLM_FLM_UNLOCK);

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void IntCmdExecutePowerstageTest(void)
{
    SPIReceiveDataNormal data;
    flm_flm_status2_ut FLM_Status2 = { .as_uint16 = 0 };
    flm_flm_diag_start_ut FLM_Diag_Start = { .as_uint16 = 0 };
    flm_flm_read_powerstage_ut FLM_Read_Powerstage = { .as_uint16 = 0 };
    boolean isDiagDataReady = FALSE;
    boolean isSuccessfulFlag;

    // Precondition (strictly 1 should be enabled)
    if ((g_pstConfiguration.lsPowerstageEn ^ g_pstConfiguration.hsPowerstageEn) == FALSE)
    {
        return;
    }

    // Decrement guard - if will become negative test takes to long, skip to the next one
    g_pstConfiguration.guardCounter++;

    // Execute test if preconditions in ASIC are met and configuration was done
    isSuccessfulFlag = QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS2, &data.dw);
    FLM_Status2.as_uint16 = data.bf.output_data;
    if (FLM_Status2.as_s.FlmDiagPstActive_u1)
    {
        uint16 regAddress;
        uint16 regData;

        // Execute procedure to define what address and data to use
        GetPSTFiringSPiCommand(&regAddress, &regData);

        //
        QSPIWriteNormal(SPI1_CS1MASTER, regAddress, regData);

        // Update flag indicating that test sequence was finished
        g_pstConfiguration.isTestSeqFinished = TRUE;
    }

    isSuccessfulFlag = QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS2, &data.dw);
    FLM_Status2.as_uint16 = data.bf.output_data;
    isDiagDataReady = FLM_Status2.as_s.FlmDiagReady_u1 & FLM_Status2.as_s.FlmDiagPstActive_u1;

    // Check if test finished or max test duration elapsed
    if (isDiagDataReady || (g_pstConfiguration.guardCounter > FLM_POWERSTAGE_GUARD_TIMEOUT))
    {
        // Powerstage test results ready
        isSuccessfulFlag = QSPIReadNormal(SPI1_CS1MASTER, RESET_VAL_FLM_FLM_READ_POWERSTAGE, &data.dw);
        FLM_Read_Powerstage.as_uint16 = data.bf.output_data;

        // Reset firing registers
        QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_HS_LS_ON_CH7_1, (ENUM_FLM_FLM_HS_LS_ON_CH7_1_FLM_CODE_CH7_1_VAL1 << FLM_FLM_HS_LS_ON_CH7_1_FLM_CODE_CH7_1_BITOFFSET));
        QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_HS_LS_ON_CH14_8, (ENUM_FLM_FLM_HS_LS_ON_CH14_8_FLM_CODE_CH14_8_VAL2 << FLM_FLM_HS_LS_ON_CH14_8_FLM_CODE_CH14_8_BITOFFSET));
        QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_HS_LS_ON_CH20_15, (ENUM_FLM_FLM_HS_LS_ON_CH20_15_FLM_CODE_CH20_15_VAL3 << FLM_FLM_HS_LS_ON_CH20_15_FLM_CODE_CH20_15_BITOFFSET));

        // Save data
        uint8 currentChannelIdx = g_pstConfiguration.channelIndex - 1;
        g_pstResults[currentChannelIdx].dw = FLM_Read_Powerstage.as_uint16 & MASK_USED_BITS_FLM_FLM_READ_POWERSTAGE;
        g_pstResults[currentChannelIdx].bf.pst_not_valid ^= 0x1;  // invert values as ASIC register displays valid flag and here used NOT valid flag
        g_pstResults[currentChannelIdx].bf.test_guard_fail = g_pstConfiguration.guardCounter < 0;
        g_pstResults[currentChannelIdx].bf.spi_on_ch_fail = !isSuccessfulFlag;

        // Increment channel & reset test running flag & reset guard
        g_pstConfiguration.channelIndex++;
        g_pstConfiguration.isTestSeqFinished = FALSE;
        g_pstConfiguration.guardCounter = FLM_POWERSTAGE_GUARD_TIMEOUT;

        // Finish test in case last channel data was received
        if (g_pstConfiguration.channelIndex > FLM_POWERSTAGE_CHANNELS_COUNT)
        {
            // Turn off Test mode interrupt of MCU
            DisableTestModeInterrupt();

            // Reset internal flags
            g_pstConfiguration.lsPowerstageEn = FALSE;
            g_pstConfiguration.hsPowerstageEn = FALSE;

            // Lock high and lowsides
            isSuccessfulFlag = QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_UNLOCK, RESET_VAL_FLM_FLM_UNLOCK);

            // Prepare acknowledge message
            USBTransmitData packageToSend;
            packageToSend.device_id = 1;
            packageToSend.msg_id = SetResponseBit(g_pstConfiguration.msg_id);
            packageToSend.status = USB_STATUS_DATA;
            packageToSend.dataLength = FLM_POWERSTAGE_CHANNELS_COUNT;
            for (uint8 i = 0; i < FLM_POWERSTAGE_CHANNELS_COUNT; i++)
            {
                packageToSend.data[i] = g_pstResults[i].dw;
            }

            // Send message to GUI
            SendUSBPackage(&packageToSend);

            return;
        }
    }

    // Arm new test (after arming can be executed in 20 us)
    if (g_pstConfiguration.isTestSeqFinished == FALSE)
    {
        // Construct start condition
        FLM_Diag_Start.as_s.FlmDiagStart_u1   = 0x1;
        FLM_Diag_Start.as_s.FlmDiagMode_u5    = (g_pstConfiguration.lsPowerstageEn) ? (ENUM_FLM_FLM_DIAG_START_FLM_DIAG_MODE_LOWSIDE_POWERSTAGE) 
                                                                                    : (ENUM_FLM_FLM_DIAG_START_FLM_DIAG_MODE_HIGHSIDE_POWERSTAGE);
        FLM_Diag_Start.as_s.FlmDiagChannel_u5 = g_pstConfiguration.channelIndex;

        // Arm next test
        isSuccessfulFlag = QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, FLM_Diag_Start.as_uint16);
    }
}

IFX_INLINE void StartTestMode(boolean isTestMode1, USBReceiveData const * const commandPackage)
{
    // Local variables
    USBTransmitData packageToSend;
    SPIReceiveDataNormal dataFLMStatus2;
    flm_flm_status2_ut FLM_Status2;
    boolean isSuccessfulFlag = TRUE;

    // Construct package to PC
    packageToSend.status = _USB_STATUS_MIN; // default status, for further error reporting
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // Precondition
    if ((g_pstConfiguration.lsPowerstageEn == TRUE) || (g_pstConfiguration.hsPowerstageEn == TRUE))
    {
        return;
    }

    // Stop running tests
    isSuccessfulFlag &= QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, RESET_VAL_FLM_FLM_DIAG_START); // TODO: may not be needed

    // Configure testmode
    g_pstConfiguration.msg_id = commandPackage->msg_id;
    g_pstConfiguration.lsPowerstageEn = isTestMode1; 
    g_pstConfiguration.hsPowerstageEn = !isTestMode1;
    g_pstConfiguration.isTestSeqFinished = FALSE;
    g_pstConfiguration.channelIndex = 1;        // Channels numeration starts from 1
    g_pstConfiguration.guardCounter = 0;

    // Reset results
    for (uint8 i = 0; i < FLM_POWERSTAGE_CHANNELS_COUNT; i++)
    {
        g_pstResults[i].dw = 0;
    }

    // Preconditions check
    // Channels status
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS2, &dataFLMStatus2.dw);
    FLM_Status2.as_uint16 = dataFLMStatus2.bf.output_data;

    // Verify if no blocking errors present
    packageToSend.dataLength = 1;
    if (isSuccessfulFlag == FALSE)
    {
        // Set error status
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.data[0] = FLM_PST_ERR_SPI_FAIL;
    }
    else if (FLM_Status2.as_s.LsenqDisAlpStatus_u1 == 1)
    {
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.data[0] = FLM_PST_ERR_LSENQ_HIGH;
    }
    else if (FLM_Status2.as_s.FlmDiagActive_u1 == 1)
    {
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.data[0] = FLM_PST_ERR_DIAG_ACTIVE;
    }

    // Construct packages based on error status if present
    if (packageToSend.status == USB_STATUS_ERROR)
    {
        g_pstConfiguration.lsPowerstageEn = FALSE;
        g_pstConfiguration.hsPowerstageEn = FALSE;

        // Send error frame
        SendUSBPackage(&packageToSend);
        return;
    }

    // Unlock high or low sides
    if (isTestMode1)
    {
        // Unlock lowsides
        flm_flm_unlock_ut unlockRegContentLS;
        unlockRegContentLS.as_uint16 = 0x0;
        unlockRegContentLS.as_s.FlmUnlockLsModule1_u1 = 0x1;
        unlockRegContentLS.as_s.FlmUnlockLsModule2_u1 = 0x1;
        unlockRegContentLS.as_s.FlmUnlockLsModule3_u1 = 0x1;
        unlockRegContentLS.as_s.FlmUnlockLsModule4_u1 = 0x1;
        unlockRegContentLS.as_s.FlmUnlockLsModule5_u1 = 0x1;
        unlockRegContentLS.as_s.FlmCodeUnlock_u2      = 0x00;
        isSuccessfulFlag &= QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_UNLOCK, unlockRegContentLS.as_uint16);
    }
    else
    {
        // Unlock highsides
        flm_flm_unlock_ut unlockRegContentHS;
        unlockRegContentHS.as_uint16 = 0x0;
        unlockRegContentHS.as_s.FlmUnlockHsModule1_u1 = 0x1;
        unlockRegContentHS.as_s.FlmUnlockHsModule2_u1 = 0x1;
        unlockRegContentHS.as_s.FlmUnlockHsModule3_u1 = 0x1;
        unlockRegContentHS.as_s.FlmUnlockHsModule4_u1 = 0x1;
        unlockRegContentHS.as_s.FlmUnlockHsModule5_u1 = 0x1;
        unlockRegContentHS.as_s.FlmCodeUnlock_u2      = 0x00;
        isSuccessfulFlag &= QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_UNLOCK, unlockRegContentHS.as_uint16);
    }

    // Configure periodicity of Test mode check interrupt
    ConfigureTestModePeriodicity(FLM_POWERSTAGE_STEP_DURATION);

    // Turn on Test mode interrupt of MCU
    EnableTestModeInterrupt();
}

void GetPSTFiringSPiCommand(uint16 *p_regAddress, uint16 *p_regData)
{
    // TODO: can be optimised to function, currently implemented as procedure (usage of global variables)

    // Construct reg address and data
    if (g_pstConfiguration.channelIndex =< 7)
    {
        // Choose address for channels in range
        *p_regAddress = FLM_FLM_HS_LS_ON_CH7_1;

        // Create register model
        flm_flm_hs_ls_on_ch7_1_ut regModel = { .as_uint16 = 0 };
        regModel.as_s.FlmCodeCh71_u2 = ENUM_FLM_FLM_HS_LS_ON_CH7_1_FLM_CODE_CH7_1_VAL1;

        // Enable required channel
        switch (g_pstConfiguration.channelIndex)
        {
            case 1:
                regModel.as_s.FlmLsOnCh1_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh1_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 2:
                regModel.as_s.FlmLsOnCh2_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh2_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 3:
                regModel.as_s.FlmLsOnCh3_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh3_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 4:
                regModel.as_s.FlmLsOnCh4_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh4_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 5:
                regModel.as_s.FlmLsOnCh5_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh5_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 6:
                regModel.as_s.FlmLsOnCh6_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh6_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 7:
                regModel.as_s.FlmLsOnCh7_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh7_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
        }

        // Content of register
        *p_regData = regModel.as_uint16;
    }
    else if (g_pstConfiguration.channelIndex =< 14)
    {
        // Choose address for channels in range
        *p_regAddress = FLM_FLM_HS_LS_ON_CH14_8;

        // Create register model
        flm_flm_hs_ls_on_ch14_8_ut regModel = { .as_uint16 = 0 };
        regModel.as_s.FlmCodeCh148_u2 = ENUM_FLM_FLM_HS_LS_ON_CH14_8_FLM_CODE_CH14_8_VAL2;

        // Enable required channel
        switch (g_pstConfiguration.channelIndex)
        {
            case 8:
                regModel.as_s.FlmLsOnCh8_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh8_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 9:
                regModel.as_s.FlmLsOnCh9_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh9_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 10:
                regModel.as_s.FlmLsOnCh10_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh10_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 11:
                regModel.as_s.FlmLsOnCh11_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh11_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 12:
                regModel.as_s.FlmLsOnCh12_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh12_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 13:
                regModel.as_s.FlmLsOnCh13_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh13_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case 14:
                regModel.as_s.FlmLsOnCh14_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh14_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
        }

        // Content of register
        *p_regData = regModel.as_uint16;
    }
    else if (g_pstConfiguration.channelIndex =< 20)
    {
        // Choose address for channels in range
        *p_regAddress = FLM_FLM_HS_LS_ON_CH20_15;

        // Create register model
        flm_flm_hs_ls_on_ch20_15_ut regModel = { .as_uint16 = 0 };
        regModel.as_s.FlmCodeCh20158_u2 = ENUM_FLM_FLM_HS_LS_ON_CH20_15_FLM_CODE_CH20_15_VAL3;

        // Enable required channel
        switch (g_pstConfiguration.channelIndex)
        {
            case :
                regModel.as_s.FlmLsOnCh_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case :
                regModel.as_s.FlmLsOnCh_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case :
                regModel.as_s.FlmLsOnCh_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case :
                regModel.as_s.FlmLsOnCh_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case :
                regModel.as_s.FlmLsOnCh_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case :
                regModel.as_s.FlmLsOnCh_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
            case :
                regModel.as_s.FlmLsOnCh_u1 = g_pstConfiguration.lsPowerstageEn;
                regModel.as_s.FlmHsOnCh_u1 = g_pstConfiguration.hsPowerstageEn;
                break;
        }

        // Content of register
        *p_regData = regModel.as_uint16;
    }
}