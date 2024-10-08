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
 * cyclic test, performed automatically, no need to check FLM_DIAG_START
 */ 
void FLMShortDiag();

/** \brief 
 */
void FLMVHxDiag();

/** \brief 
 */
void FLMSquibDetErrDiag();

/** \brief
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
void SetFLMDiagStart();

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

FLMCycDiagFaults FLMCycDiagFaultsStatus;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void InitFLMDiag()
{
    static FLMVHxMeasStatus = FLM_DIAG_STATUS_VHX_MEAS_SKIPPED;
    static flm_LoopResMeasStatusEnum FLMLoopResMeasStatus = FLM_DIAG_STATUS_LOOP_RES_MEAS_SKIPPED;
    static flm_SquibDetStatusEnum FLMSquibDetStatus = FLM_DIAG_STATUS_SQUIB_DET_SKIPPED;

    bool FLMDiagActive = 0;

    // TODO: do we need to know which FLM channels have squibs from the start (get it from GUI?)?
    // I've assumed 'Squibs on all channels' for this implementation 
}

void FLMShortDiag()
{
    static FLMShortDiagResults FLMShortDiagResultsValues;

    // read FLM_Read_Short_ch4_1 through FLM_Read_Short_ch20_17, store in FLMShortDiagResults
    // if any active, FLMCycDiagFaultsValues.FLM_SC2G_SC2B_fault = TRUE
    
}

void FLMVHxDiag()
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