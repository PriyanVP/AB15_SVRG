/**********************************************************************************************************************
 * \file timer.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef TIMER_H_
#define TIMER_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define ISR_PROVIDER_GPT12_TIMER    IfxSrc_Tos_cpu0          /* Interrupt provider                                   */

#define SERVICE_TIMER_PERIODICITY   39062u                   /* Reload value to have an interrupt each 200ms         */
#define GENERAL_TIMER_PERIODICITY   31250u                   /* Reload value to have an interrupt each 10ms          */

// Calculation formula:
// Reload Value = f_GPT * T / (Gpt1BlockPrescaler ∗ TimerInputPrescaler)
// where f_GPT = 100 MHz - GPT12 module base frequency,
// T, s - interrupt periodicity
// Gpt1BlockPrescaler - GPT1 block prescaler
// TimerInputPrescaler - timer prescaler
// Reload Value should fit in 16 bit unsigned int!


/*from users manual:
 * Timer Block GPT1 contains three timers/counters: The core timer T3 and the two auxiliary timers T2 and T4. The
    maximum resolution is fGPT/4.
*/


#define FREQ_GPT12_HZ 100000000                             /* GPT12 module base frequency                           */

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Function to initialize the GPT12 Module and configure timers
 * Should be called before starting timers!
 *
 * \return Returns nothing
 */
void InitGpt12Timer(void);

/** \brief Start service timer interrupts
 * Has to be configured first!
 *
 * \return Returns nothing
 */
void StartServiceTimer(void);

/** \brief Start general timer interrupts
 * Has to be configured first!
 *
 * \return Returns nothing
 */
void StartGeneralTimer(void);

/** \brief Stop service timer interrupts
 *
 * \return Returns nothing
 */
void StopServiceTimer(void);

/** \brief Stop general timer interrupts
 *
 * \return Returns nothing
 */
void StopGeneralTimer(void);

/** \brief Configure ASIC watchdog acknowledge periodicity
 * Periodicity is defined in number of General timer interrupts
 *
 * \param watchdogPeriodicity periodicity of reading chunks of data
 * \return Returns nothing
 */
void ConfigureWatchdogPeriodicity(uint16 watchdogPeriodicity);

/** \brief Configure error check periodicity
 * Periodicity is defined in number of General timer interrupts
 *
 * \param errorCheckPeriodicity periodicity of reading error registers in ASIC
 * \return Returns nothing
 */
void ConfigureErrorCheckPeriodicity(uint16 errorCheckPeriodicity);

/** \brief Configure continuous read periodicity
 * Periodicity is defined in number of General timer interrupts
 *
 * \param continuousReadPeriodicity periodicity of reading chunks of data
 * \return Returns nothing
 */
void ConfigureContinuousReadPeriodicity(uint16 continuousReadPeriodicity);

/** \brief Configure GPIO handling periodicity
 * Periodicity is defined in number of General timer interrupts
 *
 * \param gpioPeriodicity periodicity of reading chunks of data
 * \return Returns nothing
 */
void ConfigureGPIOPeriodicity(uint16 gpioPeriodicity);

/** \brief Enable Watchdog interrupt
 * Periodicity has to be configured first!
 *
 * \return Returns nothing
 */
void EnableWatchdogInterrupt(void);

/** \brief Enable error check interrupt
 * Periodicity has to be configured first!
 *
 * \return Returns nothing
 */
void EnableErrorCheckInterrupt(void);

/** \brief Enable continuous read interrupt
 * Periodicity has to be configured first!
 *
 * \return Returns nothing
 */
void EnableContinuousReadInterrupt(void);

/** \brief Enable GPIO handling interrupt
 * Periodicity has to be configured first!
 *
 * \return Returns nothing
 */
void EnableGPIOInterrupt(void);

/** \brief Disable Watchdog interrupt
 *
 * \return Returns nothing
 */
void DisableWatchdogInterrupt(void);

/** \brief Disable error check interrupt
 *
 * \return Returns nothing
 */
void DisableErrorCheckInterrupt(void);

/** \brief Disable continuous read interrupt
 *
 * \return Returns nothing
 */
void DisableContinuousReadInterrupt(void);

/** \brief Disable GPIO handling interrupt
 *
 * \return Returns nothing
 */
void DisableGPIOInterrupt(void);

/** \brief Get error check interrupt state
 *
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetStateWatchdogInterrupt(void);

/** \brief Get error check interrupt state
 *
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetStateErrorCheckInterrupt(void);

/** \brief Get error check interrupt state
 *
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetStateContinuousReadInterrupt(void);

/** \brief Get GPIO interrupt state
 *
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetStateGPIOInterrupt(void);

/** \brief Gets duration of Watchdog interrupt period
 *
 * \return Returns value of g_watchdogReload variable
 */
uint16 GetWatchdogPeriodicity(void);

#endif /* TIMER_H_ */
