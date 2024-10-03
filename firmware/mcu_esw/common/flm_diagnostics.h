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
    FLM_DIAG_STATUS_VHX_MEAS_SKIPPED    = 0,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_INITIATED  = 1,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_ONGOING    = 2,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_EVALUATED  = 3,    /** \brief  */
    FLM_DIAG_STATUS_VHX_MEAS_FINISHED   = 4    /** \brief  */

} flm_VHxMeasStatusEnum;

/** \brief
 */
typedef enum 
{
    FLM_DIAG_STATUS_LOOP_RES_MEAS_SKIPPED    = 0,    /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_INITIATED  = 1,    /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_ONGOING    = 2,    /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_EVALUATED  = 3,    /** \brief  */
    FLM_DIAG_STATUS_LOOP_RES_MEAS_FINISHED   = 4    /** \brief  */

} flm_LoopResMeasStatusEnum;

/** \brief
 */
typedef enum 
{
    FLM_DIAG_STATUS_SQUIB_DET_SKIPPED    = 0,    /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_INITIATED  = 1,    /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_ONGOING    = 2,    /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_EVALUATED  = 3,    /** \brief  */
    FLM_DIAG_STATUS_SQUIB_DET_FINISHED   = 4    /** \brief  */

} flm_SquibDetStatusEnum;

/*************************************************************************************************************************/
/*-------------------------------------------------Data Structures-------------------------------------------------------*/
/*************************************************************************************************************************/

/** \brief Structure to store FLM cyclic diagnostics fault flags  
 */
typedef struct
{
    bool    FLM_SC2G_SC2B_fault;                            /** \brief  */
    bool    FLM_VHxMeasErr_fault;                           /** \brief  */
    bool    FLM_LoopResErr_fault;                           /** \brief  */
    bool    FLM_SquibDetErr_fault;                          /** \brief  */
    
} FLMCycDiagFaults;

/** \brief Structure to store results of cyclic tests  
 */
typedef struct
{
    struct          FLMShortDiagResults;                    /** \brief  */
    struct          FLMVHxDiagResults;                      /** \brief  */
    struct          FLMSquibDetErrDiagResults;              /** \brief  */
    struct          FLMLoopResDiagResults;                  /** \brief  */
    
} FLMCycDiagResults;

/** \brief Structure to store results of FLM channel short detection (IGH/IGL short to ground/battery)
 */
typedef struct
{
    uint16          FLM_Read_Short_ch4_1;                   /** \brief  */
    uint16          FLM_Read_Short_ch8_5;                   /** \brief  */
    uint16          FLM_Read_Short_ch12_9;                  /** \brief  */
    uint16          FLM_Read_Short_ch16_13;                 /** \brief  */
    uint16          FLM_Read_Short_ch20_17;                 /** \brief  */

} FLMShortDiagResults;

/** \brief Structure to store results of FLM channel VH voltages diagnostic
 */
typedef struct
{
    uint16 FLM_Read_Diag_VH1a_voltage_value;          /** \brief  */
    uint16 FLM_Read_Diag_VH1b_voltage_value;          /** \brief  */
    uint16 FLM_Read_Diag_VH2_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH3_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH4_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH5_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH6_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH7_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH8_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH9_voltage_value;           /** \brief  */
    uint16 FLM_Read_Diag_VH10_voltage_value;          /** \brief  */
    bool FLM_Read_Diag_VH1a_voltage_valid;            /** \brief  */
    bool FLM_Read_Diag_VH1b_voltage_valid;            /** \brief  */
    bool FLM_Read_Diag_VH2_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH3_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH4_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH5_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH6_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH7_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH8_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH9_voltage_valid;             /** \brief  */
    bool FLM_Read_Diag_VH10_voltage_valid;            /** \brief  */

} FLMVHxDiagResults;

/** \brief Structure to store results of FLM Squib detection (Squib detection error)
 */
typedef struct
{
    bool          FLM_Squib_det_err_ch1;                    /** \brief  */
    bool          FLM_Squib_det_err_ch2;                    /** \brief  */
    bool          FLM_Squib_det_err_ch3;                    /** \brief  */
    bool          FLM_Squib_det_err_ch4;                    /** \brief  */
    bool          FLM_Squib_det_err_ch5;                    /** \brief  */
    bool          FLM_Squib_det_err_ch6;                    /** \brief  */
    bool          FLM_Squib_det_err_ch7;                    /** \brief  */
    bool          FLM_Squib_det_err_ch8;                    /** \brief  */
    bool          FLM_Squib_det_err_ch9;                    /** \brief  */
    bool          FLM_Squib_det_err_ch10;                   /** \brief  */
    bool          FLM_Squib_det_err_ch11;                   /** \brief  */
    bool          FLM_Squib_det_err_ch12;                   /** \brief  */
    bool          FLM_Squib_det_err_ch13;                   /** \brief  */
    bool          FLM_Squib_det_err_ch14;                   /** \brief  */
    bool          FLM_Squib_det_err_ch15;                   /** \brief  */
    bool          FLM_Squib_det_err_ch16;                   /** \brief  */
    bool          FLM_Squib_det_err_ch17;                   /** \brief  */
    bool          FLM_Squib_det_err_ch18;                   /** \brief  */
    bool          FLM_Squib_det_err_ch19;                   /** \brief  */
    bool          FLM_Squib_det_err_ch20;                   /** \brief  */

} FLMSquibErrorDiagResults;

/** \brief Structure to store results of FLM all Loops resistanse diagnostic
 */
typedef struct 
{
    struct FLMReadSquibRes FLM_Squib_Res_ch1;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch2;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch3;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch4;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch5;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch6;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch7;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch8;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch9;               /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch10;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch11;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch12;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch13;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch14;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch15;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch16;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch17;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch18;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch19;              /** \brief  */
    struct FLMReadSquibRes FLM_Squib_Res_ch20;              /** \brief  */    

} FLMLoopResDiagResults;

/** \brief Structure to store results of FLM Loop resistanse diagnostic
 */
typedef struct
{
    uint16 flm_squib_res_value;                             /** \brief  */
    bool flm_squib_res_err;                                 /** \brief  */
    bool flm_squib_res_valid;                               /** \brief  */
    bool flm_squib_res_pgndx_loss;                          /** \brief  */

} FLMReadSquibRes;

/*************************************************************************************************************************/
/*------------------------------------------------Function Prototypes----------------------------------------------------*/
/*************************************************************************************************************************/

/*************************************************************************************************************************/
/*-------------------------------------------------Global variables------------------------------------------------------*/
/*************************************************************************************************************************/

#endif /* FLM_DIAGNOSTICS_H_ */