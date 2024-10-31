/**********************************************************************************************************************
 * \file flm_diagnostics.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

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

/** \brief
 * Measure Battery voltage, normal range to perform diagnostics
 * is 6...18V
 */
bool CheckBatVoltage();

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static FLMCycDiagFaults g_FLMCycDiagFaultsValues;
static flm_DiagExecStatusEnum g_FLMCycDiagStatus;
static FLMCycDiagResults g_FLMCycDiagResultsValues;
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

// read FLM_Read_Short_ch4_1 through FLM_Read_Short_ch20_17, store in FLMShortDiagResults
void FLMShortDiag()
{
    // TODO: research for QSPIReadSequenceNormal()
    
    // if any active, FLMCycDiagFaultsValues.FLM_SC2G_SC2B_fault = TRUE
}

void FLMVHxDiag()
{
    static FLMVHxDiagResults FLMVHxDiagResultsValues;

    if (CheckBatVoltage()==FALSE) 
    {
        g_FLMCycDiagStatus.flm_VHxMeasStatus = FLM_DIAG_STATUS_VHX_MEAS_SKIPPED;
    }

    if (g_FLMDiagActive != 0) && (g_FLMDiagReady != 1)
    {   
        // Wait for any previous diagnostic to end
        // TODO: implement getters for g_FLMDiagActive and g_FLMDiagReady
        // TODO: implement timeout for 1.5-2 duration of diagnostic -> if timeout then feature error to PC 
    }

    g_FLMCycDiagFaultsValues.FLM_VHxMeasErr_fault = 0; // as per Vasant's diagrams; clearing fault here doesn't seem right for me
    g_FLMCycDiagStatus.flm_VHxMeasStatus = FLM_DIAG_STATUS_VHX_MEAS_INITIATED;


}

void FLMSquibDetErrDiag() //
{
    static FLMSquibDetErrDiagResults FLMSquibDetErrDiagResultsValues;

    //
    
}

void FLMLoopResDiag() //
{
    static FLMLoopResDiagResults FLMLoopResDiagResultsValues;

    //
    
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