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
#include "common/flm_diagnostics.h"
#include "top/spi_wrapper.h"

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

#define FLM_DIAG_READ_VHX_REGS_COUNT        (11)    /** \brief Number of registers with results of VHx measurement 
                                                    results*/ 

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief 
 */
void CmdEnableFLMDiag();

/** \brief 
 */
void InitFLMDiag();

/** \brief 
 * cyclic test performed automatically, no need to set FLM_DIAG_START
 */ 
void FLMShortDiag();

/** \brief 
 * SPI-triggered test, must be set by FLM_diag_mode = VHx_volt_meas_all and started FLM_DIAG_START = 1
 */
void FLMVHxDiag();

/** \brief 
 * SPI-triggered test, must be set by FLM_diag_mode = Squib_pres_test_all and started FLM_DIAG_START = 1
 */
void FLMSquibDetErrDiag();

/** \brief
 * SPI-triggered test, must be set by FLM_diag_mode = Loop_res_meas_all_ch and started FLM_DIAG_START = 1
 */
void FLMLoopResDiag();

/** \brief
 */
void GetFLMDiagMode();

/** \brief Select diagnostic to be run by FLM diag module
 * write diagMode to flm_diag_mode field of FLM_DIAG_START register
 */
void SetFLMDiagMode(FLMDiagModeEnum diagMode);

/** \brief
 */
void StartFLMDiag();

/** \brief
 */
void CmdDisableFLMDiag();

// TODO: implement
/** \brief get FLM diagnostic execution status from ASIC (ongoing/evaluated)
 */
flm_cycDiagExecStatusEnum FLMReadDiagExecStatus(void);

/** \brief
 */
flm_DiagExecOrderEnum GetFLMDiagExecOrder (void);

/** \brief
 */
void SetFLMDiagExecOrder(flm_DiagExecOrderEnum execNumber);

/** \brief
 * Measure Battery voltage, normal range to perform diagnostics
 * is 6...18V
 */
bool CheckBatVoltage();

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static FLMCycDiagFaults g_FLMCycDiagFaultsValues;
static FLMCycDiagResults g_flmCycDiagResultsValues;
static flm_DiagExecStatusEnum g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_IDLE;
static flm_DiagExecOrderEnum g_flmDiagExecNumber = FLM_DIAG_ORDER_SHORT_DET;
//bool static g_FLMDiagActive = 0; // TODO similar (maybe more like status) should be available at top level to see if MCU is busy with FLM diag
//bool static g_FLMDiagReady = 0;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdEnableFLMDiag()
{
    // Configure periodicity of FLM diagnoscics MCU interrupt
    ConfigureFLMDiagPeriodicity();
    // Turn on FLM diagnostics performing interrupt of MCU
    EnableFLMDiagInterrupt();
}

void CmdDisableFLMDiag()
{
    // Set status and number of diag to init values for proper start
    // of diagnostics on next enable
    SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_IDLE);
    SetFLMDiagExecOrder(FLM_DIAG_ORDER_SHORT_DET);

    // Turn off FLM diagnostics performing interrupt of MCU
    DisableFLMDiagInterrupt();
}

void InitFLMDiag()
{
    g_FLMCycDiagStatus.flm_VHxMeasStatus     = FLM_DIAG_STATUS_VHX_MEAS_SKIPPED;
    g_FLMCycDiagStatus.flm_LoopResMeasStatus = FLM_DIAG_STATUS_LOOP_RES_MEAS_SKIPPED;
    g_FLMCycDiagStatus.flm_SquibDetStatus    = FLM_DIAG_STATUS_SQUIB_DET_SKIPPED;

    // 'Squibs on all channels' for this implementation 
}

// Diagnostic is running automatically by default, just read results
void FLMShortDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_SHORT_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_SHORT_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagShortsRegsAddresses = {FLM_FLM_READ_SHORT_CH4_1, FLM_FLM_READ_SHORT_CH8_5, 
                                         FLM_FLM_READ_SHORT_CH12_9, FLM_FLM_READ_SHORT_CH16_13, 
                                         FLM_FLM_READ_SHORT_CH20_17};
    
    // read FLM_Read_Short_ch4_1 through FLM_Read_Short_ch20_17, store in FLMShortDiagResults
    SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_ONGOING);

    // Read related registers from ASIC
    isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagShortsRegsAddresses, &data[0].dw, &length); //TODO: question: should data argument be &data[0].dw not &data[].dw ?

    // Store results //TODO: check order of data
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch4_1 = data[0].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch8_5 = data[1].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch12_9 = data[2].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch16_13 = data[3].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch20_17 = data[4].bf.output_data;

    // results are stored, get back
    SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_FINISHED);
    
    // TODO: quick result analisys to check for faults
    // if any active, FLMCycDiagFaultsValues.FLM_SC2G_SC2B_fault = TRUE
    return;
}

void FLMVHxDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_VHX_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_VHX_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagVHxRegsAddresses[FLM_DIAG_READ_VHX_REGS_COUNT] =  { FLM_FLM_READ_DIAG_VH1A, FLM_FLM_READ_DIAG_VH2, 
                                                                        FLM_FLM_READ_DIAG_VH3, FLM_FLM_READ_DIAG_VH4, 
                                                                        FLM_FLM_READ_DIAG_VH5, FLM_FLM_READ_DIAG_VH6, 
                                                                        FLM_FLM_READ_DIAG_VH7, FLM_FLM_READ_DIAG_VH8, 
                                                                        FLM_FLM_READ_DIAG_VH9, FLM_FLM_READ_DIAG_VH10,
                                                                        FLM_FLM_READ_DIAG_VH1B };
    static FLMVHxDiagResults FLMVHxDiagResultsValues;

    if (CheckBatVoltage()==FALSE) 
    {
        // TODO: re-do to support current logic
        //g_FLMCycDiagStatus.flm_VHxMeasStatus = FLM_DIAG_STATUS_VHX_MEAS_SKIPPED;
    }

    // TODO: clean up
    //if (g_FLMDiagActive != 0) && (g_FLMDiagReady != 1)
    //{   
    //    // Wait for any previous diagnostic to end
    //    // TODO: implement getters for g_FLMDiagActive and g_FLMDiagReady
    //    // TODO: implement timeout for 1.5-2 duration of diagnostic -> if timeout then feature error to PC 
    //}

    if (GetFLMDiagExecStatus() == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode
        SetFLMDiagMode(FLM_DIAG_MODE_VHX_MEAS);

        // Start diagnostic
        StartFLMDiag();
        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (GetFLMDiagExecStatus() == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // TODO: diagnostic was performed, store results
        // Read FLM_FLM_READ_DIAG_VH1A...FLM_FLM_READ_DIAG_VH10, FLM_FLM_READ_DIAG_VH1B
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagVHxRegsAddresses, &data[0].dw, &length);
        // Store results TODO: check order of data
        for (uint8 i = 0; i < FLM_DIAG_READ_VHX_REGS_COUNT ; i++)
        {
            flm_flm_read_diag_vh1a_ut flmReadDiagVHxTmp = data[i].bf.output_data;
            g_flmCycDiagResultsValues.flmVHxDiagResults[i].FLM_Read_Diag_VHx_voltage_valid = flmReadDiagVHxTmp.as_s.FlmVhVoltageValid_u1;
            g_flmCycDiagResultsValues.flmVHxDiagResults[i].FLM_Read_Diag_VHx_voltage_value = flmReadDiagVHxTmp.as_s.FlmVhVoltageValue_u12;
        }

        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_FINISHED);
    }

    // TODO: evaluate wether to implement error status handling
    // g_FLMCycDiagFaultsValues.FLM_VHxMeasErr_fault = 0; // as per Vasant's diagrams; clearing fault here doesn't seem right for me
    return;
}

void FLMSquibDetErrDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_SQUIB_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_SQUIB_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagSquibRegsAddresses[FLM_DIAG_READ_SQUIB_REGS_COUNT] = {FLM_FLM_READ_SQUIB_CH16_1, FLM_FLM_READ_SQUIB_CH20_17};
    
    static FLMSquibDetErrDiagResults FLMSquibDetErrDiagResultsValues;

    if (GetFLMDiagExecStatus() == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode
        SetFLMDiagMode(FLM_DIAG_MODE_SQUIB_DET);

        // Start diagnostic
        StartFLMDiag();
        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (GetFLMDiagExecStatus() == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // TODO: diagnostic was performed, store results
        // Read FLM_READ_SQUIB_CH16_1, FLM_READ_SQUIB_CH20_17
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagSquibRegsAddresses, &data[0].dw, &length);
        // Store results TODO: check order of data
        for (uint8 i = 0; i < 20 ; i++)
        {
            if (i < 16)
            {
                // Load results of channels 1-16
                g_flmCycDiagResultsValues.flmSquibErrorDiagResults[i] = ((data[0].bf.output_data)&(1<<i));
            }
            else
            {
                // Load results of channels 17-20
                g_flmCycDiagResultsValues.flmSquibErrorDiagResults[i] = ((data[1].bf.output_data)&(1<<(i-16)));
            }
        }

        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_FINISHED);
    }

    // TODO: evaluate wether to implement error status handling
    return;
}

void FLMLoopResDiag() //
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_RES_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_RES_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagResRegsAddresses[FLM_DIAG_READ_RES_REGS_COUNT] = {FLM_FLM_READ_SQUIB_RES_CH1, FLM_FLM_READ_SQUIB_RES_CH2, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH3, FLM_FLM_READ_SQUIB_RES_CH4, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH5, FLM_FLM_READ_SQUIB_RES_CH6, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH7, FLM_FLM_READ_SQUIB_RES_CH8, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH9, FLM_FLM_READ_SQUIB_RES_CH10,
                                                                    FLM_FLM_READ_SQUIB_RES_CH11,FLM_FLM_READ_SQUIB_RES_CH12,
                                                                    FLM_FLM_READ_SQUIB_RES_CH13,FLM_FLM_READ_SQUIB_RES_CH14,
                                                                    FLM_FLM_READ_SQUIB_RES_CH15,FLM_FLM_READ_SQUIB_RES_CH16,
                                                                    FLM_FLM_READ_SQUIB_RES_CH17,FLM_FLM_READ_SQUIB_RES_CH18,
                                                                    FLM_FLM_READ_SQUIB_RES_CH19,FLM_FLM_READ_SQUIB_RES_CH20,
                                                                    FLM_FLM_READ_SQUIB_RES_SQREF};

    if (GetFLMDiagExecStatus() == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode
        SetFLMDiagMode(FLM_DIAG_MODE_LOOP_RES_MEAS);

        // Start diagnostic
        StartFLMDiag();
        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (GetFLMDiagExecStatus() == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // TODO: diagnostic was performed, store results
        // Read FLM_READ_SQUIB_RES_CH1...FLM_READ_SQUIB_RES_CH20
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagResRegsAddresses, &data[0].dw, &length);
        // Store results TODO: check order of data
        for (uint8 i = 0; i < FLM_DIAG_READ_RES_REGS_COUNT ; i++)
        {
            flm_flm_read_squib_res_ch1_ut flmReadSquibResChxTmp = data[i].bf.output_data;
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_value = flmReadSquibResChxTmp.as_s.FlmSquibResValue_u13;
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_err = flmReadSquibResChxTmp.as_s.FlmSquibResErr_u1;
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_valid = flmReadSquibResChxTmp.as_s.FlmSquibResValid_u1;     
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_pgndx_loss = flmReadSquibResChxTmp.as_s.FlmSquibResPgndxLoss_u1;
        }

        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_FINISHED);
    }

    // TODO: evaluate wether to implement error status handling
    return;
}

void GetFLMDiagMode(void)
{
    // 
}

void StartFLMDiag(void)
{
    SPIReceiveDataNormal data;
    boolean isSuccessfulFlag = TRUE;
    flm_flm_diag_start_ut tmpFLMDiagStartfRegister;
    
    // Get value from ASIC
    // TODO: check whether diags can be run on slaves (spiChannel selection)
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, &data.dw);
    tmpFLMDiagStartfRegister.as_uint16 = data.output_data;
    
    // flm_diag_start = 1 starts selected diagnostic
    tmpFLMDiagStartfRegister.as_s.FlmDiagStart_u1 = 1;

    // Write
    QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, tmpFLMDiagStartfRegister.as_uint16);
}

flm_cycDiagExecStatusEnum FLMReadDiagExecStatus(void)
{
    // TODO FLM_FLM_STATUS2
    SPIReceiveDataNormal data;
    boolean isSuccessfulFlag = TRUE;
    flm_flm_status2_ut tmpFLMDiagStatus2fRegister;
    
    // Get value from ASIC
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS2, &data.dw);
    tmpFLMDiagStatus2fRegister.as_uint16 = data.output_data;
    
    // Determine FLM diagnostic execution status
    if ((tmpFLMDiagStatus2fRegister.as_s.FlmDiagActive_u1 == 1) && (tmpFLMDiagStatus2fRegister.as_s.FlmDiagReady_u1 == 0))
    {
        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_ONGOING);
    }

    if ((tmpFLMDiagStatus2fRegister.as_s.FlmDiagActive_u1 == 0) && (tmpFLMDiagStatus2fRegister.as_s.FlmDiagReady_u1 == 1))
    {
        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_EVALUATED);
    }
}

void SetFLMDiagExecStatus(flm_cycDiagExecStatusEnum FLMCycDiagExecStatus)
{
    g_FLMDiagExecStatus = FLMCycDiagExecStatus;
}

flm_cycDiagExecStatusEnum GetFLMDiagExecStatus(void)
{
    return g_FLMDiagExecStatus;
}

void SetFLMDiagExecOrder (flm_DiagExecOrderEnum execNumber)
    {
        g_flmDiagExecNumber = execNumber;
    }

flm_DiagExecOrderEnum GetFLMDiagExecOrder (void)
    {
        return g_flmDiagExecNumber;
    }

void SetFLMDiagMode(FLMDiagModeEnum diagMode)
{
    SPIReceiveDataNormal data;
    boolean isSuccessfulFlag = TRUE;
    flm_flm_diag_start_ut tmpFLMDiagStartfRegister;
    
    // Get value from ASIC
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, &data.dw);
    tmpFLMDiagStartfRegister.as_uint16 = data.output_data;
    
    // flm_diag_start = 1 starts selected diagnostic
    tmpFLMDiagStartfRegister.as_s.FlmDiagMode_u5 = diagMode;

    // Write
    QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, tmpFLMDiagStartfRegister.as_uint16);
}

bool CheckBatVoltage(void)
{
    // TODO
    // Mock for first iteration
    return TRUE;
}