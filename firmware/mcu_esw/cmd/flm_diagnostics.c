/**********************************************************************************************************************
 * \file flm_diagnostics.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "common/spi_data_types.h"
#include "common/usb_data_types.h"
#include "common/bit_manipulation.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "periphery/timer.h"
#include "flm_diagnostics.h"

// TODO: change approach to naming: FLM_DIAG -> DIAG

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define FLM_DIAG_INTERRUPT_PERIODICITY      (2000)  /** \brief Periodicity of FLM diagnostic performing interrupt, in 
                                                    GENERAL_TIMER_PERIODICITY tics (50us) */

#define FLM_DIAG_READ_SHORT_REGS_COUNT      (5)     /** \brief Number of registers with results of IGL/IGH short and 
                                                    leakage to battery or ground detection */

#define FLM_DIAG_READ_SQUIB_REGS_COUNT      (2)     /** \brief Number of registers with results of squib detection */

#define FLM_DIAG_READ_RES_REGS_COUNT        (21)    /** \brief Number of registers with results of loop resistance 
                                                    measurement, including SQREF */

#define FLM_DIAG_READ_VHX_REGS_COUNT        (12)    /** \brief Number of registers with results of VHx measurement 
                                                    results*/

#define DIAG_READ_SVRG_DIAG_REGS_COUNT      (1)     /** \brief Number of registers with results of SVRG diagnostic */

/*************************************************************************************************************************/
/*--------------------------------------------------Enumerations---------------------------------------------------------*/
/*************************************************************************************************************************/

/** \brief Order of execution of FLM Cyclic diagnostics
 */
typedef enum 
{
    FLM_DIAG_ORDER_SHORT_DET        = 1,        /** \brief IGL/IGH short and leakage to battery/ground (all loops by default) */
    FLM_DIAG_ORDER_VHX_MEAS         = 2,        /** \brief VHx voltage measurement (all loops) */
    FLM_DIAG_ORDER_LOOP_RES_MEAS    = 3,        /** \brief Loop resistance measurement (all loops) */
    FLM_DIAG_ORDER_SQUIB_DET        = 4,        /** \brief Squib presence test (all loops) */
    DIAG_ORDER_FLM_SVRG_TST         = 5,        /** \brief SVRG test from FLM Diagnostics*/
    DIAG_ORDER_SVRG_DIAG            = 6,        /** \brief SVRG diagnostic from SVRG module */ // TODO: probably obsolete
    FLM_DIAG_ORDER_SAVE_RESULTS     = 7         /** \brief Save results of diagnostics into package for GUI */
} FLMDiagExecOrderEnum;

/** \brief Execution status of each diagnostic
 * Idle -> (Ongoing -> Evaluated -> Finished -> Ongoing -> ... )
 */
typedef enum 
{
    FLM_DIAG_EXEC_STATUS_IDLE           = 0,   /** \brief Start of Cyclic diagnostic round after enable start command */
    FLM_DIAG_EXEC_STATUS_ONGOING        = 1,   /** \brief Diagnostic is started in ASIC */
    FLM_DIAG_EXEC_STATUS_EVALUATED      = 2,   /** \brief Diagnostic is done in ASIC */
    FLM_DIAG_EXEC_STATUS_FINISHED       = 3    /** \brief Results are read from ASIC, can proceed to next diagnostic */
} FLMDiagExecStatusEnum;

/*************************************************************************************************************************/
/*-------------------------------------------------Data Structures-------------------------------------------------------*/
/*************************************************************************************************************************/

/** \brief Structure to organize enable flags of each diagnostic 
*/
typedef union
{
    struct 
    {
        uint8     shortDetEn      :1;
        uint8     VHxMeasEn       :1;
        uint8     squibDetEn      :1;
        uint8     loopResMeasEn   :1;
        uint8     SVRGtestEn      :1;
        uint8     SVRGdiagEn      :1;
        uint8     unused          :2;
    } bf;
    uint8 sw;
} DiagEnableFlags;

/** \brief Structure to store results of FLM channel short detection (IGH/IGL short to ground/battery)
 * 10 bytes
 */
typedef struct
{
    uint16          readShortCh4_1;
    uint16          readShortCh8_5;
    uint16          readShortCh12_9;
    uint16          readShortCh16_13;
    uint16          readShortCh20_17;
} FLMShortDiagStruct;

/** \brief Structure to store results of one FLM channel VH voltage diagnostic
 * 11 channels, 24 bytes
 */ 
typedef struct 
{
    uint16 readVHxVoltageValue;
    boolean readVHxVoltageValid;
} FLMDiagReadVHx;

/** \brief Structure to store results of FLM Loop resistanse diagnostic
 * 20 loops, 48 bytes
 */
typedef struct
{
    uint16 readSquibResValue;
    boolean readSquibResErr;
    boolean readSquibResValid;
    boolean readSquibResPgndxLoss;
} FLMReadSquibRes;

/** \brief Structure to store results of SVRG capacity diagnostic
 * 2 bytes
 */
typedef struct
{
    uint16 readSVRGcapacityValue;
    boolean readSVRGcapacityValid;
} DiagReadSVRGdiag;

/** \brief Structure to store results of cyclic tests
 * 86 bytes
 * TODO: add field/fields to indicate if diagnostics failed on MCU level (i.e., SPI read was unsuccessful)
 */
typedef struct
{
    FLMDiagReadVHx      resultVHxDiag[12];
    boolean             resultSquibErrorDiag[20];
    FLMReadSquibRes     resultLoopResDiag[20];
    FLMShortDiagStruct  resultShortDiag;
    DiagReadSVRGdiag     resultSVRGdiag;
} FLMCycDiagResults;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Cyclic test performed automatically, no need to set FLM_DIAG_START
 */ 
void FLMShortDiag(void);

/** \brief SPI-triggered test, must be set by FLM_diag_mode = VHx_volt_meas_all and started FLM_DIAG_START = 1
 */
void FLMVHxDiag(void);

/** \brief SPI-triggered test, must be set by FLM_diag_mode = Loop_res_meas_all_ch and started FLM_DIAG_START = 1
 */
void FLMLoopResDiag(void);

/** \brief SPI-triggered test, must be set by FLM_diag_mode = Squib_pres_test_all and started FLM_DIAG_START = 1
 */
void FLMSquibDetErrDiag(void);

/** \brief SPI-triggered test, enabled by FML_diag_mode = SVRG_test and started FLM_DIAG_START = 1
 */
void DiagFLMSVRGTest(void);

/** \brief  SVRG module test, configured and started by GUI, MCU only reads results (SVRG_STATUS reg)
 */
void DiagSVRGDiag(void);

/** \brief Save diag results into package to be sent to GUI
 */
void FLMSaveDiagResults(void);

/** \brief Select diagnostic to be run by FLM diag module and start diagnostic
 * \param diagMode configuration to select required diagnostic in ASIC
 */
void StartFLMDiag(uint8 diagMode);

/** \brief Get FLM diagnostic execution status from ASIC (ongoing/evaluated)
 */
void FLMUpdateDiagExecStatus(void);

/** \brief Measure Battery voltage, normal range to perform diagnostics is 6...18V
 */
boolean CheckBatVoltage(void);

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static boolean g_isflmDiagEn = FALSE;
static DiagEnableFlags g_diagEnableFlags;
static FLMCycDiagResults g_resultsValues;
static FLMDiagExecStatusEnum g_diagExecStatus = FLM_DIAG_EXEC_STATUS_IDLE;
static FLMDiagExecOrderEnum g_diagExecNumber = FLM_DIAG_ORDER_SHORT_DET;
static USBTransmitData g_resultsToSend;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdEnableFLMDiag(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;

    // Update enable flags from GUI command
    g_diagEnableFlags.sw = commandPackage->data[0];

    // Enable command should not be executed twice
    if (g_isflmDiagEn == TRUE)
    {
        // Skip further function execution - FLM Diag interrupt already is enabled,
        // GUI will see no response to repeated USB_CMD_FLM_DIAG_ENABLE command
        return;
    }

    // Supply voltage must be in normal range to perform FLM diagnostics
    if (CheckBatVoltage()==FALSE) 
    {
        // TODO: don't enable diagnostics, send Error report to GUI
    }

    // Enable FLM diag functionality
    // FLM diag state flag is set
    g_isflmDiagEn = TRUE;

    // Configure periodicity of FLM diagnoscics MCU interrupt
    ConfigureTimerPeriodicity(FLM_DIAG_TIMER, FLM_DIAG_INTERRUPT_PERIODICITY);

    // Turn on FLM diagnostics performing interrupt of MCU
    EnableTimerInterrupt(FLM_DIAG_TIMER);
    
    // Initialise execution status to Idle
    g_diagExecStatus = FLM_DIAG_EXEC_STATUS_IDLE;

    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send report to GUI
    SendUSBPackage(&packageToSend);
}

void CmdDisableFLMDiag(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;

    // Disable command should not be executed twice
    if (g_isflmDiagEn == FALSE)
    {
        // Skip further function execution - FLM Diag already is disabled, 
        // GUI will see no response to repeated USB_CMD_FLM_DIAG_DISABLE command
        return;
    }

    // Disable FLM diag functionality
    // FLM diag state flag is reset
    g_isflmDiagEn = FALSE;
    // Set status and number of diag to init values for proper start
    // of diagnostics on next enable
    g_diagExecStatus = FLM_DIAG_EXEC_STATUS_IDLE;
    g_diagExecNumber = FLM_DIAG_ORDER_SHORT_DET;

    // Reset all Diagnostic enable flags
    g_diagEnableFlags.sw = 0;

    // Turn off FLM diagnostics performing interrupt of MCU
    DisableTimerInterrupt(FLM_DIAG_TIMER);

    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send report to GUI
    SendUSBPackage(&packageToSend);
}

void CmdReadFLMDiagResults(USBReceiveData const * const commandPackage)
{
    g_resultsToSend.device_id = 0;
    g_resultsToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    g_resultsToSend.status = USB_STATUS_DATA;
    g_resultsToSend.dataLength = 86;

    // Send data back to MCU
    SendUSBPackage(&g_resultsToSend);
}

void IntCmdExecuteFLMDiag()
{
    // Initial FLM Diagnostic execution state is initialised as Idle
    // and on later rounds updated from ASIC 
    if ((g_diagExecStatus != FLM_DIAG_EXEC_STATUS_IDLE) &&
        (g_diagExecNumber != DIAG_ORDER_SVRG_DIAG)) // skip for non-FLM diag
    {
        FLMUpdateDiagExecStatus();
    }

    // Start diagnostic and get out
    // On next entries, check execution status
    switch (g_diagExecNumber)
    {
    case FLM_DIAG_ORDER_SHORT_DET:
        // Check enable and status of diag execution
        // Dont enter any diagnostic if disabled, or last diagnostic didn't finish yet
        if ((g_diagEnableFlags.bf.shortDetEn == TRUE)&&
            ((g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)||(g_diagExecStatus == FLM_DIAG_EXEC_STATUS_IDLE)))
        {
            FLMShortDiag();
        }
        // Move on to next diagnostic
        g_diagExecNumber = FLM_DIAG_ORDER_VHX_MEAS;
        break;

    case FLM_DIAG_ORDER_VHX_MEAS: 
        if (g_diagEnableFlags.bf.VHxMeasEn == TRUE)
        {
            FLMVHxDiag();
        }
        if ((g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)||(g_diagEnableFlags.bf.VHxMeasEn == FALSE))
        {
            // Move on to next diagnostic
            g_diagExecNumber = FLM_DIAG_ORDER_LOOP_RES_MEAS;
        }
        break;

    case FLM_DIAG_ORDER_LOOP_RES_MEAS:
        if (g_diagEnableFlags.bf.loopResMeasEn == TRUE)
        {
            FLMLoopResDiag();
        }
        if ((g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)||(g_diagEnableFlags.bf.loopResMeasEn == FALSE))
        {
            // Move on to next diagnostic
            g_diagExecNumber = FLM_DIAG_ORDER_SQUIB_DET;
        }
        break;

    case FLM_DIAG_ORDER_SQUIB_DET:
        if (g_diagEnableFlags.bf.squibDetEn == TRUE)
        {
            FLMSquibDetErrDiag();
        }
        if ((g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)||(g_diagEnableFlags.bf.squibDetEn == FALSE))
        {
            // Move on to next diagnostic
            g_diagExecNumber = DIAG_ORDER_FLM_SVRG_TST;
        }
        break;

    case DIAG_ORDER_FLM_SVRG_TST:
        if (g_diagEnableFlags.bf.SVRGtestEn == TRUE)
        {
            DiagFLMSVRGTest();
        }
        if ((g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)||(g_diagEnableFlags.bf.SVRGtestEn == FALSE))
        {
            // Move on to next diagnostic
            g_diagExecNumber = DIAG_ORDER_SVRG_DIAG;
        }
        break;

    case DIAG_ORDER_SVRG_DIAG:
        if (g_diagEnableFlags.bf.SVRGdiagEn == TRUE)
        {
            DiagSVRGDiag();
        }
        if ((g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)||(g_diagEnableFlags.bf.SVRGdiagEn == FALSE))
        {
            // Move on to next diagnostic
            g_diagExecNumber = FLM_DIAG_ORDER_SAVE_RESULTS;
        }
        break;
    
    case FLM_DIAG_ORDER_SAVE_RESULTS:
        FLMSaveDiagResults();
        if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
        {
            // Move on to next diagnostic
            g_diagExecNumber = FLM_DIAG_ORDER_SHORT_DET;
        }
        break;

    default:
        break;
    }
}

// Diagnostic is running automatically by default, just read results
void FLMShortDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_SHORT_REGS_COUNT];
    uint16 length = FLM_DIAG_READ_SHORT_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    static const uint16 flmDiagShortsRegsAddresses[FLM_DIAG_READ_SHORT_REGS_COUNT] = {
        FLM_FLM_READ_SHORT_CH4_1, FLM_FLM_READ_SHORT_CH8_5, FLM_FLM_READ_SHORT_CH12_9, 
        FLM_FLM_READ_SHORT_CH16_13, FLM_FLM_READ_SHORT_CH20_17};
    
    // read FLM_Read_Short_ch4_1 through FLM_Read_Short_ch20_17, store in resultShortDiag
    g_diagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

    // Read related registers from ASIC
    isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagShortsRegsAddresses, &data[0].dw, &length);

    // Store results
    g_resultsValues.resultShortDiag.readShortCh4_1 = data[0].bf.output_data;
    g_resultsValues.resultShortDiag.readShortCh8_5 = data[1].bf.output_data;
    g_resultsValues.resultShortDiag.readShortCh12_9 = data[2].bf.output_data;
    g_resultsValues.resultShortDiag.readShortCh16_13 = data[3].bf.output_data;
    g_resultsValues.resultShortDiag.readShortCh20_17 = data[4].bf.output_data;

    // results are stored, get back
    g_diagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    
    return;
}

void FLMVHxDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_VHX_REGS_COUNT];
    uint16 length = FLM_DIAG_READ_VHX_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    static const uint16 flmDiagVHxRegsAddresses[FLM_DIAG_READ_VHX_REGS_COUNT] =  { 
        FLM_FLM_READ_DIAG_VH1A, FLM_FLM_READ_DIAG_VH2, FLM_FLM_READ_DIAG_VH3, 
        FLM_FLM_READ_DIAG_VH4, FLM_FLM_READ_DIAG_VH5, FLM_FLM_READ_DIAG_VH6, 
        FLM_FLM_READ_DIAG_VH7, FLM_FLM_READ_DIAG_VH8, FLM_FLM_READ_DIAG_VH9, 
        FLM_FLM_READ_DIAG_VH10, FLM_FLM_READ_DIAG_VH1B, FLM_FLM_READ_DIAG_SVRG_GATE};

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode and start diagnostic
        StartFLMDiag(ENUM_FLM_FLM_DIAG_START_FLM_DIAG_MODE_VH_MEASE_ALL);
        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // Read FLM_FLM_READ_DIAG_VH1A...FLM_FLM_READ_DIAG_VH10, FLM_FLM_READ_DIAG_VH1B
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagVHxRegsAddresses, &data[0].dw, &length);
        // Store results
        for (uint8 i = 0; i < FLM_DIAG_READ_VHX_REGS_COUNT ; i++)
        {
            flm_flm_read_diag_vh1a_ut flmReadDiagVHxTmp;
            flmReadDiagVHxTmp.as_uint16 = (data[i].bf.output_data);
            g_resultsValues.resultVHxDiag[i].readVHxVoltageValid = flmReadDiagVHxTmp.as_s.FlmVhVoltageValid_u1;
            g_resultsValues.resultVHxDiag[i].readVHxVoltageValue = flmReadDiagVHxTmp.as_s.FlmVhVoltageValue_u12;
        }

        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    }

    return;
}

void FLMSquibDetErrDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_SQUIB_REGS_COUNT];
    uint16 length = FLM_DIAG_READ_SQUIB_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    static const uint16 flmDiagSquibRegsAddresses[FLM_DIAG_READ_SQUIB_REGS_COUNT] = {
        FLM_FLM_READ_SQUIB_CH16_1, FLM_FLM_READ_SQUIB_CH20_17};

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode and start diagnostic
        StartFLMDiag(ENUM_FLM_FLM_DIAG_START_FLM_DIAG_MODE_SQUIB_PRES_ALL);
        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // Read FLM_READ_SQUIB_CH16_1, FLM_READ_SQUIB_CH20_17
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagSquibRegsAddresses, &data[0].dw, &length);
        // Store results
        for (uint8 i = 0; i < 20 ; i++)
        {
            if (i < 16)
            {
                // Load results of channels 1-16
                g_resultsValues.resultSquibErrorDiag[i] = ((data[0].bf.output_data)&(1<<i));
            }
            else
            {
                // Load results of channels 17-20
                g_resultsValues.resultSquibErrorDiag[i] = ((data[1].bf.output_data)&(1<<(i-16)));
            }
        }

        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    }

    return;
}

void FLMLoopResDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_RES_REGS_COUNT];
    uint16 length = FLM_DIAG_READ_RES_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    static const uint16 flmDiagResRegsAddresses[FLM_DIAG_READ_RES_REGS_COUNT] = {
        FLM_FLM_READ_SQUIB_RES_CH1, FLM_FLM_READ_SQUIB_RES_CH2, FLM_FLM_READ_SQUIB_RES_CH3, 
        FLM_FLM_READ_SQUIB_RES_CH4, FLM_FLM_READ_SQUIB_RES_CH5, FLM_FLM_READ_SQUIB_RES_CH6, 
        FLM_FLM_READ_SQUIB_RES_CH7, FLM_FLM_READ_SQUIB_RES_CH8, FLM_FLM_READ_SQUIB_RES_CH9, 
        FLM_FLM_READ_SQUIB_RES_CH10, FLM_FLM_READ_SQUIB_RES_CH11, FLM_FLM_READ_SQUIB_RES_CH12,
        FLM_FLM_READ_SQUIB_RES_CH13, FLM_FLM_READ_SQUIB_RES_CH14, FLM_FLM_READ_SQUIB_RES_CH15, 
        FLM_FLM_READ_SQUIB_RES_CH16, FLM_FLM_READ_SQUIB_RES_CH17, FLM_FLM_READ_SQUIB_RES_CH18,
        FLM_FLM_READ_SQUIB_RES_CH19, FLM_FLM_READ_SQUIB_RES_CH20, FLM_FLM_READ_SQUIB_RES_SQREF};

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode and start diagnostic
        StartFLMDiag(ENUM_FLM_FLM_DIAG_START_FLM_DIAG_MODE_SQUIB_RES_ALL);
        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // Read FLM_READ_SQUIB_RES_CH1...FLM_READ_SQUIB_RES_CH20
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagResRegsAddresses, &data[0].dw, &length);
        // Store results
        for (uint8 i = 0; i < FLM_DIAG_READ_RES_REGS_COUNT ; i++)
        {
            flm_flm_read_squib_res_ch1_ut flmReadSquibResChxTmp; 
            flmReadSquibResChxTmp.as_uint16 = (data[i].bf.output_data);
            g_resultsValues.resultLoopResDiag[i].readSquibResValue = flmReadSquibResChxTmp.as_s.FlmSquibResValue_u13;
            g_resultsValues.resultLoopResDiag[i].readSquibResErr = flmReadSquibResChxTmp.as_s.FlmSquibResErr_u1;
            g_resultsValues.resultLoopResDiag[i].readSquibResValid = flmReadSquibResChxTmp.as_s.FlmSquibResValid_u1;     
            g_resultsValues.resultLoopResDiag[i].readSquibResPgndxLoss = flmReadSquibResChxTmp.as_s.FlmSquibResPgndxLoss_u1;
        }

        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    }

    return;
}

void DiagFLMSVRGTest()
{
    // TODO: investigate current working theory how this diagnostic works
    // Configured with SVRG_Diag by GUI; selected and started with FLM_DIAG_START, 
    // Execution status monitored by FLM_Status2 as other FLM Diags
    SPIReceiveDataNormal data[DIAG_READ_SVRG_DIAG_REGS_COUNT];
    uint16 length = DIAG_READ_SVRG_DIAG_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    static const uint16 diagSVRGdiagRegsAddresses[DIAG_READ_SVRG_DIAG_REGS_COUNT] = {SVRG_SVRG_STATUS};

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode and start diagnostic
        StartFLMDiag(ENUM_FLM_FLM_DIAG_START_FLM_DIAG_MODE_SVRG_TEST);
        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (g_diagExecStatus == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // Read SVRG_STATUS
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, diagSVRGdiagRegsAddresses, &data[0].dw, &length);
    
        // Store results
        svrg_svrg_status_ut diagReadSVRGDiagResTmp;
        diagReadSVRGDiagResTmp.as_uint16 = (data[i].bf.output_data);
        g_resultsValues.resultSVRGdiag.readSVRGcapacityValue = diagReadSVRGDiagResTmp.as_s.SvrgCapValue_u9;
        g_resultsValues.resultSVRGdiag.readSVRGcapacityValid = diagReadSVRGDiagResTmp.as_s.SvrgCapValueValid_u1;

        // Results are stored, get back
        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    }

    return;
}

void DiagSVRGDiag()
{
    return;
}

void FLMSaveDiagResults()
{
    // Short detection results
    g_resultsToSend.data[0] = GetLSB(g_resultsValues.resultShortDiag.readShortCh4_1);
    g_resultsToSend.data[1] = GetMSB(g_resultsValues.resultShortDiag.readShortCh4_1);
    g_resultsToSend.data[2] = GetLSB(g_resultsValues.resultShortDiag.readShortCh8_5);
    g_resultsToSend.data[3] = GetMSB(g_resultsValues.resultShortDiag.readShortCh8_5);
    g_resultsToSend.data[4] = GetLSB(g_resultsValues.resultShortDiag.readShortCh12_9);
    g_resultsToSend.data[5] = GetMSB(g_resultsValues.resultShortDiag.readShortCh12_9);
    g_resultsToSend.data[6] = GetLSB(g_resultsValues.resultShortDiag.readShortCh16_13);
    g_resultsToSend.data[7] = GetMSB(g_resultsValues.resultShortDiag.readShortCh16_13);
    g_resultsToSend.data[8] = GetLSB(g_resultsValues.resultShortDiag.readShortCh20_17);
    g_resultsToSend.data[9] = GetMSB(g_resultsValues.resultShortDiag.readShortCh20_17);

    // VHx diagnostic results resultVHxDiag[]
    // TODO: add reading of FLM_Read_Diag_SVRG_GATE
    g_resultsToSend.data[10] = GetLSB(g_resultsValues.resultVHxDiag[0].readVHxVoltageValue);
    g_resultsToSend.data[11] = GetMSB(g_resultsValues.resultVHxDiag[0].readVHxVoltageValue);
    g_resultsToSend.data[12] = GetLSB(g_resultsValues.resultVHxDiag[1].readVHxVoltageValue);
    g_resultsToSend.data[13] = GetMSB(g_resultsValues.resultVHxDiag[1].readVHxVoltageValue);
    g_resultsToSend.data[14] = GetLSB(g_resultsValues.resultVHxDiag[2].readVHxVoltageValue);
    g_resultsToSend.data[15] = GetMSB(g_resultsValues.resultVHxDiag[2].readVHxVoltageValue);
    g_resultsToSend.data[16] = GetLSB(g_resultsValues.resultVHxDiag[3].readVHxVoltageValue);
    g_resultsToSend.data[17] = GetMSB(g_resultsValues.resultVHxDiag[3].readVHxVoltageValue);
    g_resultsToSend.data[18] = GetLSB(g_resultsValues.resultVHxDiag[4].readVHxVoltageValue);
    g_resultsToSend.data[19] = GetMSB(g_resultsValues.resultVHxDiag[4].readVHxVoltageValue);
    g_resultsToSend.data[20] = GetLSB(g_resultsValues.resultVHxDiag[5].readVHxVoltageValue);
    g_resultsToSend.data[21] = GetMSB(g_resultsValues.resultVHxDiag[5].readVHxVoltageValue);
    g_resultsToSend.data[22] = GetLSB(g_resultsValues.resultVHxDiag[6].readVHxVoltageValue);
    g_resultsToSend.data[23] = GetMSB(g_resultsValues.resultVHxDiag[6].readVHxVoltageValue);
    g_resultsToSend.data[24] = GetLSB(g_resultsValues.resultVHxDiag[7].readVHxVoltageValue);
    g_resultsToSend.data[25] = GetMSB(g_resultsValues.resultVHxDiag[7].readVHxVoltageValue);
    g_resultsToSend.data[26] = GetLSB(g_resultsValues.resultVHxDiag[8].readVHxVoltageValue);
    g_resultsToSend.data[27] = GetMSB(g_resultsValues.resultVHxDiag[8].readVHxVoltageValue);
    g_resultsToSend.data[28] = GetLSB(g_resultsValues.resultVHxDiag[9].readVHxVoltageValue);
    g_resultsToSend.data[29] = GetMSB(g_resultsValues.resultVHxDiag[9].readVHxVoltageValue);
    g_resultsToSend.data[30] = GetLSB(g_resultsValues.resultVHxDiag[10].readVHxVoltageValue);
    g_resultsToSend.data[31] = GetMSB(g_resultsValues.resultVHxDiag[10].readVHxVoltageValue);

    uint8 tmp1VHxVoltageValid, tmp2VHxVoltageValid = 0;
    for (uint8 i = 0; i<11; i++)
    {
        if (i < 8)
        {
            if (g_resultsValues.resultVHxDiag[i].readVHxVoltageValid)
            {
                tmp1VHxVoltageValid |= (1 << i);
            }
        }
        else
        {
            if (g_resultsValues.resultVHxDiag[i].readVHxVoltageValid)
            {
                tmp2VHxVoltageValid |= (1 << (i-8));
            }
        }
    }
    g_resultsToSend.data[32] = tmp1VHxVoltageValid;
    g_resultsToSend.data[33] = tmp2VHxVoltageValid;

    // Squib detection error results
    uint8 tmp1SquibDetErr, tmp2SquibDetErr, tmp3SquibDetErr = 0;
    for (uint8 i = 0; i<20; i++)
    {
        if (i < 8)
        {
            if (g_resultsValues.resultSquibErrorDiag[i])
            {
                tmp1SquibDetErr |= (1 << i);
            }
        }
        else if (i < 16)
        {
            if (g_resultsValues.resultSquibErrorDiag[i])
            {
                tmp2SquibDetErr |= (1 << i);
            }
        }
        else
        {
            if (g_resultsValues.resultSquibErrorDiag[i])
            {
                tmp3SquibDetErr |= (1 << i);
            }
        }
    }
    g_resultsToSend.data[34] = tmp1SquibDetErr;
    g_resultsToSend.data[35] = tmp2SquibDetErr;
    g_resultsToSend.data[36] = tmp3SquibDetErr;

    // Loop resistance diagnostics results
    g_resultsToSend.data[37] = GetLSB(g_resultsValues.resultLoopResDiag[0].readSquibResValue);
    g_resultsToSend.data[38] = GetMSB(g_resultsValues.resultLoopResDiag[0].readSquibResValue);
    g_resultsToSend.data[39] = GetLSB(g_resultsValues.resultLoopResDiag[1].readSquibResValue);
    g_resultsToSend.data[40] = GetMSB(g_resultsValues.resultLoopResDiag[1].readSquibResValue);
    g_resultsToSend.data[41] = GetLSB(g_resultsValues.resultLoopResDiag[2].readSquibResValue);
    g_resultsToSend.data[42] = GetMSB(g_resultsValues.resultLoopResDiag[2].readSquibResValue);
    g_resultsToSend.data[43] = GetLSB(g_resultsValues.resultLoopResDiag[3].readSquibResValue);
    g_resultsToSend.data[44] = GetMSB(g_resultsValues.resultLoopResDiag[3].readSquibResValue);
    g_resultsToSend.data[45] = GetLSB(g_resultsValues.resultLoopResDiag[4].readSquibResValue);
    g_resultsToSend.data[46] = GetMSB(g_resultsValues.resultLoopResDiag[4].readSquibResValue);
    g_resultsToSend.data[47] = GetLSB(g_resultsValues.resultLoopResDiag[5].readSquibResValue);
    g_resultsToSend.data[48] = GetMSB(g_resultsValues.resultLoopResDiag[5].readSquibResValue);
    g_resultsToSend.data[49] = GetLSB(g_resultsValues.resultLoopResDiag[6].readSquibResValue);
    g_resultsToSend.data[50] = GetMSB(g_resultsValues.resultLoopResDiag[6].readSquibResValue);
    g_resultsToSend.data[51] = GetLSB(g_resultsValues.resultLoopResDiag[7].readSquibResValue);
    g_resultsToSend.data[52] = GetMSB(g_resultsValues.resultLoopResDiag[7].readSquibResValue);
    g_resultsToSend.data[53] = GetLSB(g_resultsValues.resultLoopResDiag[8].readSquibResValue);
    g_resultsToSend.data[54] = GetMSB(g_resultsValues.resultLoopResDiag[8].readSquibResValue);
    g_resultsToSend.data[55] = GetLSB(g_resultsValues.resultLoopResDiag[9].readSquibResValue);
    g_resultsToSend.data[56] = GetMSB(g_resultsValues.resultLoopResDiag[9].readSquibResValue);
    g_resultsToSend.data[57] = GetLSB(g_resultsValues.resultLoopResDiag[10].readSquibResValue);
    g_resultsToSend.data[58] = GetMSB(g_resultsValues.resultLoopResDiag[10].readSquibResValue);
    g_resultsToSend.data[59] = GetLSB(g_resultsValues.resultLoopResDiag[11].readSquibResValue);
    g_resultsToSend.data[60] = GetMSB(g_resultsValues.resultLoopResDiag[11].readSquibResValue);
    g_resultsToSend.data[61] = GetLSB(g_resultsValues.resultLoopResDiag[12].readSquibResValue);
    g_resultsToSend.data[62] = GetMSB(g_resultsValues.resultLoopResDiag[12].readSquibResValue);
    g_resultsToSend.data[63] = GetLSB(g_resultsValues.resultLoopResDiag[13].readSquibResValue);
    g_resultsToSend.data[64] = GetMSB(g_resultsValues.resultLoopResDiag[13].readSquibResValue);
    g_resultsToSend.data[65] = GetLSB(g_resultsValues.resultLoopResDiag[14].readSquibResValue);
    g_resultsToSend.data[66] = GetMSB(g_resultsValues.resultLoopResDiag[14].readSquibResValue);
    g_resultsToSend.data[67] = GetLSB(g_resultsValues.resultLoopResDiag[15].readSquibResValue);
    g_resultsToSend.data[68] = GetMSB(g_resultsValues.resultLoopResDiag[15].readSquibResValue);
    g_resultsToSend.data[69] = GetLSB(g_resultsValues.resultLoopResDiag[16].readSquibResValue);
    g_resultsToSend.data[70] = GetMSB(g_resultsValues.resultLoopResDiag[16].readSquibResValue);
    g_resultsToSend.data[71] = GetLSB(g_resultsValues.resultLoopResDiag[17].readSquibResValue);
    g_resultsToSend.data[72] = GetMSB(g_resultsValues.resultLoopResDiag[17].readSquibResValue);
    g_resultsToSend.data[73] = GetLSB(g_resultsValues.resultLoopResDiag[18].readSquibResValue);
    g_resultsToSend.data[74] = GetMSB(g_resultsValues.resultLoopResDiag[18].readSquibResValue);
    g_resultsToSend.data[75] = GetLSB(g_resultsValues.resultLoopResDiag[19].readSquibResValue);
    g_resultsToSend.data[76] = GetMSB(g_resultsValues.resultLoopResDiag[19].readSquibResValue);

    uint8 tmp1SquibResErr, tmp2SquibResErr, tmp3SquibResErr = 0;
    for (uint8 i = 0; i<20; i++)
    {
        if (i < 8)
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResErr)
            {
                tmp1SquibResErr |= (1 << i);
            }
        }
        else if (i < 16)
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResErr)
            {
                tmp2SquibResErr |= (1 << i);
            }
        }
        else
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResErr)
            {
                tmp3SquibResErr |= (1 << i);
            }
        }
    }
    g_resultsToSend.data[77] = tmp1SquibResErr;
    g_resultsToSend.data[78] = tmp2SquibResErr;
    g_resultsToSend.data[79] = tmp3SquibResErr;

    uint8 tmp1SquibResValid, tmp2SquibResValid, tmp3SquibResValid = 0;
    for (uint8 i = 0; i<20; i++)
    {
        if (i < 8)
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResValid)
            {
                tmp1SquibResValid |= (1 << i);
            }
        }
        else if (i < 16)
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResValid)
            {
                tmp2SquibResValid |= (1 << i);
            }
        }
        else
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResValid)
            {
                tmp3SquibResValid |= (1 << i);
            }
        }
    }
    g_resultsToSend.data[80] = tmp1SquibResValid;
    g_resultsToSend.data[81] = tmp2SquibResValid;
    g_resultsToSend.data[82] = tmp3SquibResValid;

    uint8 tmp1SquibResPgndxLoss, tmp2SquibResPgndxLoss, tmp3SquibResPgndxLoss = 0;
    for (uint8 i = 0; i<20; i++)
    {
        if (i < 8)
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResPgndxLoss)
            {
                tmp1SquibResPgndxLoss |= (1 << i);
            }
        }
        else if (i < 16)
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResPgndxLoss)
            {
                tmp2SquibResPgndxLoss |= (1 << i);
            }
        }
        else
        {
            if (g_resultsValues.resultLoopResDiag[i].readSquibResPgndxLoss)
            {
                tmp3SquibResPgndxLoss |= (1 << i);
            }
        }
    }
    g_resultsToSend.data[83] = tmp1SquibResPgndxLoss;
    g_resultsToSend.data[84] = tmp2SquibResPgndxLoss;
    g_resultsToSend.data[85] = tmp3SquibResPgndxLoss;
}

void StartFLMDiag(uint8 diagMode)
{
    flm_flm_diag_start_ut tmpFLMDiagStartfRegister;
    
    // Select Diagnostic to execute
    tmpFLMDiagStartfRegister.as_s.FlmDiagMode_u5 = diagMode;
    // flm_diag_start = 1 starts selected diagnostic
    tmpFLMDiagStartfRegister.as_s.FlmDiagStart_u1 = 1;

    // Write to ASIC
    // TODO: check whether diags can be run on slaves (spiChannel selection)
    QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, tmpFLMDiagStartfRegister.as_uint16);
}

void FLMUpdateDiagExecStatus(void)
{
    SPIReceiveDataNormal data = {.dw = 0};
    boolean isSuccessfulFlag = TRUE;
    flm_flm_status2_ut tmpFLMDiagStatus2fRegister;
    
    // Get value from ASIC
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS2, &data.dw);
    tmpFLMDiagStatus2fRegister.as_uint16 = data.bf.output_data;
    
    // Determine FLM diagnostic execution status
    if ((tmpFLMDiagStatus2fRegister.as_s.FlmDiagActive_u1 == 1) && (tmpFLMDiagStatus2fRegister.as_s.FlmDiagReady_u1 == 0))
    {
        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;
    }
    else if ((g_diagExecStatus != FLM_DIAG_EXEC_STATUS_FINISHED) && (tmpFLMDiagStatus2fRegister.as_s.FlmDiagActive_u1 == 0) && (tmpFLMDiagStatus2fRegister.as_s.FlmDiagReady_u1 == 1))
    {
        g_diagExecStatus = FLM_DIAG_EXEC_STATUS_EVALUATED;
    }
}

boolean CheckBatVoltage(void)
{
    // Mocked for first iteration
    // TODO: evaluate whether to implement
    return TRUE;
}
