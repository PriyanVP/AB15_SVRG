/**********************************************************************************************************************
 * \file status.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef STATUS_H_
#define STATUS_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Ifx_Types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/


/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Defines applicable states for state field in global status
 */
typedef enum
{
    MCU_STATE_IDLE                  = 0,   /** \brief idle state, entered immediately after POR */
    MCU_STATE_CONFIGURED            = 1,   /** \brief configured state, initial configuration passed, in dispatcher loop */
    MCU_STATE_ERROR                 = 2    /** \brief error occurred */
} MCUStatesEnum;

/** \brief Defines applicable states for continuously running features
 */
typedef enum
{
    FEATURE_DISABLED    = 0,           /** \brief disabled state */
    FEATURE_CONFIGURED  = 1,           /** \brief configured state, feature is still disabled, but config received */
    FEATURE_RUNNING     = 2,           /** \brief execution of feature ongoing */
    FEATURE_ERROR       = 3            /** \brief error during feature execution occured */
} ContinuousFeaturesStatesEnum;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Initializes MCU status. All fields set to 0
 *
 * \return Returns nothing
 */
void InitStatus(void);

/** \brief Gets MCU status
 *
 * \return Returns MCU status
 */
uint32 GetStatus(void);

/** \brief Sets state of MCU SW execution
 *
 * \param newState new state of state field in status
 * \return Nothing
 */
void SetState(MCUStatesEnum newState);

/** \brief Gets USB communication error flag
 *
 * \return Returns TRUE is error flag set, FALSE - otherwise
 */
boolean GetUSBError(void);

/** \brief Gets SPI communication error flag
 *
 * \return Returns TRUE is error flag set, FALSE - otherwise
 */
boolean GetSPIError(void);

/** \brief Sets USB communication error flag
 *
 * \return Returns nothing
 */
void SetUSBError(void);

/** \brief Sets SPI communication error flag
 *
 * \return Returns nothing
 */
void SetSPIError(void);

/** \brief Clears USB communication error flag
 *
 * \return Returns nothing
 */
void ClearUSBError(void);

/** \brief Clears SPI communication error flag
 *
 * \return Returns nothing
 */
void ClearSPIError(void);

/** \brief Gets state of continuous read feature
 *
 * \return State
 */
ContinuousFeaturesStatesEnum GetContinuousReadState(void);

/** \brief Gets state of error check feature
 *
 * \return State
 */
ContinuousFeaturesStatesEnum GetErrorCheckState(void);

/** \brief Gets state of watchdog feature
 *
 * \return State
 */
ContinuousFeaturesStatesEnum GetWatchdogState(void);

/** \brief Gets state of GPIO feature
 *
 * \return State
 */
ContinuousFeaturesStatesEnum GetGPIOState(void);

/** \brief Sets state of continuous read feature
 *
 * \param newState new state of continuous read field in status
 * \return Nothing
 */
void SetContinuousReadState(ContinuousFeaturesStatesEnum newState);

/** \brief Sets state of error check feature
 *
 * \param newState new state of error check field in status
 * \return Nothing
 */
void SetErrorCheckState(ContinuousFeaturesStatesEnum newState);

/** \brief Sets state of watchdog feature
 *
 * \param newState new state of watchdog field in status
 * \return Nothing
 */
void SetWatchdogState(ContinuousFeaturesStatesEnum newState);

/** \brief Sets state of GPIO feature
 *
 * \param newState new state of watchdog field in status
 * \return Nothing
 */
void SetGPIOState(ContinuousFeaturesStatesEnum newState);

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

#endif /* STATUS_H_ */
