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

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define FLM_DIAG_READ_SHORT_REGS_COUNT      (5)     /** \brief Number of registers with results of IGL/IGH short and 
                                                    leakage to battery or ground detection */

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

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

/** \brief
 */
void SetFLMDiagMode();

/** \brief
 */
void StartFLMDiag();

// TODO: implement
/** \brief get FLM diagnostic execution status from ASIC (ongoing/evaluated)
 */
flm_cycDiagExecStatusEnum FLMReadDiagExecStatus(void);


/** \brief
 * Measure Battery voltage, normal range to perform diagnostics
 * is 6...18V
 */
bool CheckBatVoltage();

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static FLMCycDiagFaults g_FLMCycDiagFaultsValues;
static flm_cycDiagExecStatusEnum g_FLMCycDiagExecStatus;
static FLMCycDiagResults g_flmCycDiagResultsValues;
//bool static g_FLMDiagActive = 0; // similar should be available at top level to see if MCU is busy with FLM diag
//bool static g_FLMDiagReady = 0;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

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
    isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagShortsRegsAddresses, &data[0].dw, &length);
    
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

    if (GetFLMDiagExecStatus() != FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        g_flmDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;
        // TODO: start diagnostic and get out
        // code
        return;
    }
    else
    {
        // TODO: diagnostic was performed, store results
        // code
        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_FINISHED);
    }

    // TODO: update this portion
    g_FLMCycDiagFaultsValues.FLM_VHxMeasErr_fault = 0; // as per Vasant's diagrams; clearing fault here doesn't seem right for me
    g_FLMCycDiagStatus.flm_VHxMeasStatus = FLM_DIAG_STATUS_VHX_MEAS_INITIATED;
    return;
}

void FLMSquibDetErrDiag() //
{
    static FLMSquibDetErrDiagResults FLMSquibDetErrDiagResultsValues;

        if (GetFLMDiagExecStatus() != FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        g_flmDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;
        // TODO: start diagnostic and get out
        // code
        return;
    }
    else
    {
        // TODO: diagnostic was performed, store results
        // code
        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_FINISHED);
    }

    return;
    
}

void FLMLoopResDiag() //
{
    static FLMLoopResDiagResults FLMLoopResDiagResultsValues;

    if (GetFLMDiagExecStatus() != FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        g_flmDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;
        // TODO: start diagnostic and get out
        // code
        return;
    }
    else
    {
        // TODO: diagnostic was performed, store results
        // code
        SetFLMDiagExecStatus(FLM_DIAG_EXEC_STATUS_FINISHED);
    }

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
    tmpFLMDiagStartfRegister.as_uint16 = data.output_data
    
    // flm_diag_start = 1 starts selected diagnostic
    tmpFLMDiagStartfRegister.as_s.FlmDiagStart_u1 = 1;

    // Write
    QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, tmpFLMDiagStartfRegister.as_uint16);
}

flm_cycDiagExecStatusEnum FLMReadDiagExecStatus(void)
{
    // TODO
    // Read flm_diag_active and FLM_diag_ready from ASIC
    // (flm_diag_active ==1 && FLM_diag_ready == 0) -> SPI triggered diag is running)
    // (flm_diag_active ==0 && flm_diag_ready ==1) -> SPI triggered diag is evaluated, read results)
}

void SetFLMDiagExecStatus(flm_cycDiagExecStatusEnum FLMCycDiagExecStatus)
{
    g_FLMDiagExecStatus = FLMCycDiagExecStatus;
}

flm_cycDiagExecStatusEnum GetFLMDiagExecStatus(void)
{
    return g_FLMDiagExecStatus;
}

void SetFLMDiagMode(void)
{
    // 
}

bool CheckBatVoltage(void)
{
    // TODO
    // Mock for first iteration
    return TRUE;
}