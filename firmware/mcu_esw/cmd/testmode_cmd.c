/**********************************************************************************************************************
 * \file testmode_cmd.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "common/global_defines.h"
#include "common/spi_data_types.h"
#include "common/watchdog_types.h"
#include "common/bit_manipulation.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "periphery/timer.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "watchdog.h"
#include "pwm.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define FLM_POWERSTAGE_GUARD_TIMEOUT     (5)  /** \brief If powerstage test is not finished after 
                                                      5 expected durations - skip test */

#define FLM_POWERSTAGE_EXPECTED_DURATION (2)  /** \brief Expected duration of one powerstage test: 85 us/ 50 us
                                                    where 50 us - timer interrupt periodicity on MCU */

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

    // SPIReceiveDataNormal dataFLMStatus1;
    // SPIReceiveDataNormal dataFLMStatus2;
    // flm_flm_status1_ut FLM_Status1;
    // flm_flm_status2_ut FLM_Status2;

    // // Preconditions check
    // // Channels status
    // isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS1, &dataFLMStatus1.dw);
    // isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS2, &dataFLMStatus2.dw);
    // FLM_Status1.as_uint16 = dataFLMStatus1.bf.output_data;
    // FLM_Status2.as_uint16 = dataFLMStatus2.bf.output_data;

    // // Construct packages based on error status
    // packageToSend.dataLength = 1;
    // if (isSuccessfulFlag == FALSE)
    // {
    //     // Common error frame setup
    //     packageToSend.status = USB_STATUS_ERROR;
    //     packageToSend.data[0] = FLM_PST_ERR_SPI_FAIL;
    // }
    // else if (FLM_Status1.as_s.)
    // {
    //     packageToSend.status = USB_STATUS_ACK;
    //     packageToSend.data[0] =
    // }

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Union for powerstage test result
 */
typedef union
{
    struct
    {
        uint32 pst_pretest_s2x_err : 1;    /** \brief short detected before test activation */
        uint32 pst_test_s2x_err    : 1;    /** \brief short detected after test activation */
        uint32 pst_timeout_err     : 1;    /** \brief powerstage timeout limit reached, test canceled */
        uint32 test_guard_fail     : 1;    /** \brief guard failed - diag_ready bit is not set for too long */
        uint32 unused              : 4;    /** \brief unused bits */
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
    boolean isTestRunning;                    /** \brief true if channle test is running */
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

/** \brief Function to provide a Response word for Challenge of AB12's Watchdog 1 and 2 
 * \param challengeValue value of AB12's Watchdog 1 and 2 Challenge word
 * \return Returns Response word
 */
uint16 GetResponseWordAB12(uint16 challengeValue);

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdStartTestMode1(USBReceiveData const * const commandPackage)
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

    // Stop running tests
    isSuccessfulFlag &= QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, RESET_VAL_FLM_FLM_DIAG_START);

    // Send data back to MCU
    SendUSBPackage(&packageToSend);

    // Configure testmode
    g_pstConfiguration.lsPowerstageEn = TRUE;   // Difference between HS and LS - TODO: make universal
    g_pstConfiguration.hsPowerstageEn = FALSE;
    g_pstConfiguration.isTestRunning = FALSE;
    g_pstConfiguration.channelIndex = 1;
    g_pstConfiguration.guardCounter = FLM_POWERSTAGE_GUARD_TIMEOUT;

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

    // Configure periodicity of Test mode check interrupt
    ConfigureTestModePeriodicity(FLM_POWERSTAGE_EXPECTED_DURATION);

    // Turn on Test mode interrupt of MCU
    EnableTestModeInterrupt();
}

void CmdStopTestMode12(USBReceiveData const * const commandPackage) // TODO: not needed? short duration
{
    // Turn off Test mode interrupt of MCU
    DisableTestModeInterrupt();

    // Reset internal flags
    g_pstConfiguration.lsPowerstageEn = FALSE;
    g_pstConfiguration.hsPowerstageEn = FALSE;

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void CmdStartTestMode2(USBReceiveData const * const commandPackage)
{
    // empty for now
    return;
}

void IntCmdExecutePowerstageTest(void)
{
    // TODO: check if ready
    // TODO: start diagnostics
    // TODO: increment channel
    // TODO: store diagn data
    // TODO: increment guard - if <= 0 set error in diagn data, go to next channel
    // TODO: reset variables for next iteration

    // based on high low side write
}
