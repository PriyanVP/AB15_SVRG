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

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

flm_VHxMeasStatusEnum FLMVHxMeasStatus = FLM_DIAG_STATUS_VHX_MEAS_SKIPPED;
flm_LoopResMeasStatusEnum FLMLoopResMeasStatus = FLM_DIAG_STATUS_LOOP_RES_MEAS_SKIPPED;
flm_SquibDetStatusEnum FLMSquibDetStatus = FLM_DIAG_STATUS_SQUIB_DET_SKIPPED;

FLMCycDiagFaults FLMCycDiagFaultsValues;

bool FLMDiagActive = 0;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void FLMShortDiag() // cyclic test, performed automatically, no need to check FLM_DIAG_START
{
    static FLMShortDiagResults FLMShortDiagResultsValues;

    // read FLM_Read_Short_ch4_1 through FLM_Read_Short_ch20_17, store in FLMShortDiagResults
    // if any active, FLMCycDiagFaultsValues.FLM_SC2G_SC2B_fault = TRUE
    
}

void FLMVHxDiag() //
{
    static FLMVHxDiagResults FLMVHxDiagResultsValues;

    // 
    
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

void SetFLMDiagStart(void)
{
    // 
}

void SetFLMDiagMode(void)
{
    // 
}