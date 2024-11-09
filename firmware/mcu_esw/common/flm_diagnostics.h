/**************************************************************************************************************************
 * \file flm_diagnostics.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *************************************************************************************************************************/

#ifndef FLM_DIAGNOSTICS_H_
#define FLM_DIAGNOSTICS_H_

/*************************************************************************************************************************/
/*-----------------------------------------------------Includes----------------------------------------------------------*/
/*************************************************************************************************************************/

#include "Ifx_Types.h"

/*************************************************************************************************************************/
/*------------------------------------------------------Macros-----------------------------------------------------------*/
/*************************************************************************************************************************/


/*************************************************************************************************************************/
/*--------------------------------------------------Enumerations---------------------------------------------------------*/
/*************************************************************************************************************************/

/** \brief
 */
typedef enum 
{
    FLM_DIAG_ORDER_SHORT_DET        = 1,        /** \brief  */
    FLM_DIAG_ORDER_VHX_MEAS         = 2,        /** \brief  */
    FLM_DIAG_ORDER_LOOP_RES_MEAS    = 3,        /** \brief  */
    FLM_DIAG_ORDER_SQUIB_DET        = 4         /** \brief  */

} flm_DiagExecOrderEnum;

/** \brief
 */
typedef enum
{
    FLM_DIAG_MODE_VHX_MEAS          = 9,    /** \brief 01001 */
    FLM_DIAG_MODE_LOOP_RES_MEAS     = 3,    /** \brief 00011 */
    FLM_DIAG_MODE_SQUIB_DET         = 5,    /** \brief 00101 */
} FLMDiagModeEnum;

/** \brief
 */
typedef enum 
{
    FLM_DIAG_EXEC_STATUS_IDLE           = 0,   /** \brief  */
    FLM_DIAG_EXEC_STATUS_ONGOING        = 1,   /** \brief  */
    FLM_DIAG_EXEC_STATUS_EVALUATED      = 2,   /** \brief  */
    FLM_DIAG_EXEC_STATUS_FINISHED       = 3    /** \brief  */

} flm_DiagExecStatusEnum;

/** \brief
 */
typedef enum 
{
    FLM_DIAG_STATUS_VHX_MEAS_SKIPPED    = 0,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_INITIATED  = 1,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_ONGOING    = 2,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_EVALUATED  = 3,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_FINISHED   = 4     /** \brief  */

} flm_VHxMeasStatusEnum;

/** \brief
 */
typedef enum 
{
    FLM_DIAG_STATUS_LOOP_RES_MEAS_SKIPPED    = 0,       /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_INITIATED  = 1,       /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_ONGOING    = 2,       /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_EVALUATED  = 3,       /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_FINISHED   = 4        /** \brief  */

} flm_LoopResMeasStatusEnum;

/** \brief
 */
typedef enum 
{
    FLM_DIAG_STATUS_SQUIB_DET_SKIPPED    = 0,       /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_INITIATED  = 1,       /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_ONGOING    = 2,       /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_EVALUATED  = 3,       /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_FINISHED   = 4        /** \brief  */

} flm_SquibDetStatusEnum;

/*************************************************************************************************************************/
/*-------------------------------------------------Data Structures-------------------------------------------------------*/
/*************************************************************************************************************************/

// TODO: transfer FLM Diag - specific type declarations to .c file to reduce visibility scope

/** \brief Structure to store fault flags of FLM cyclic diagnostics  
 */
typedef struct
{
    bool    FLM_SC2G_SC2B_fault;                            /** \brief  */
    bool    FLM_VHxMeasErr_fault;                           /** \brief  */
    bool    FLM_LoopResErr_fault;                           /** \brief  */
    bool    FLM_SquibDetErr_fault;                          /** \brief  */
    
} FLMCycDiagFaults;

/** \brief Structure to store execution status of FLM cyclic diagnostic
*/
typedef struct
{
    flm_VHxMeasStatusEnum       flm_VHxMeasStatus;      /** \brief  */
    flm_LoopResMeasStatusEnum   flm_LoopResMeasStatus;  /** \brief  */
    flm_SquibDetStatusEnum      flm_SquibDetStatus;     /** \brief  */

} FLMCycDiagStatus;

/** \brief Structure to store results of cyclic tests  
 */
// TODO: check and fix naming to comply with naming convention
typedef struct
{
    FLM_Read_Diag_VHx   flmVHxDiagResults[11];              /** \brief  */
    bool                flmSquibErrorDiagResults[20];       /** \brief  */
    FLMReadSquibRes     flmLoopResDiagResults[20];          /** \brief  */
    FLMShortDiagStruct  flmShortDiagResults;                /** \brief  */

} FLMCycDiagResults;

/** \brief Structure to store results of FLM channel short detection (IGH/IGL short to ground/battery)
 * 10 bytes
 */
typedef struct
{
    uint16          FLM_Read_Short_ch4_1;                   /** \brief  */
    uint16          FLM_Read_Short_ch8_5;                   /** \brief  */
    uint16          FLM_Read_Short_ch12_9;                  /** \brief  */
    uint16          FLM_Read_Short_ch16_13;                 /** \brief  */
    uint16          FLM_Read_Short_ch20_17;                 /** \brief  */

} FLMShortDiagStruct;

/** \brief Structure to store results of one FLM channel VH voltage diagnostic
 */ 
typedef struct 
{
    uint16 FLM_Read_Diag_VHx_voltage_value;          /** \brief  */
    bool FLM_Read_Diag_VHx_voltage_valid;            /** \brief  */

}FLM_Read_Diag_VHx;

/** \brief Array to store results of all FLM channel VH voltages diagnostic
 * 24 bytes
 */ 
FLM_Read_Diag_VHx FLMVHxDiagResults[11];

/** \brief Structure to store results of FLM Squib detection (Squib detection error, FLM_Squib_det_err_ch1...ch20)
 * 3 bytes
 */
bool FLMSquibErrorDiagResults[20];

/** \brief Structure to store results of FLM Loop resistanse diagnostic
 */
typedef struct
{
    uint16 flm_squib_res_value;                             /** \brief  */
    bool flm_squib_res_err;                                 /** \brief  */
    bool flm_squib_res_valid;                               /** \brief  */
    bool flm_squib_res_pgndx_loss;                          /** \brief  */

} FLMReadSquibRes;

/** \brief Structure to store results of FLM all Loops resistanse diagnostic
 * 48 bytes
 */
struct FLMReadSquibRes FLMLoopResDiagResults[20];

/*************************************************************************************************************************/
/*------------------------------------------------Function Prototypes----------------------------------------------------*/
/*************************************************************************************************************************/

/*************************************************************************************************************************/
/*-------------------------------------------------Global variables------------------------------------------------------*/
/*************************************************************************************************************************/

#endif /* FLM_DIAGNOSTICS_H_ */