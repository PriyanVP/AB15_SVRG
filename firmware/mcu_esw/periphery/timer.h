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
#include "common/watchdog_types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define ISR_PROVIDER_GPT12_TIMER    IfxSrc_Tos_cpu0          /* Interrupt provider                                   */

#define GENERAL_TIMER_PERIODICITY      625u                  /* Reload value to have an interrupt each  50us*/
                                                             /* fGPT = 100 Mhz
                                                                (a) IfxGpt12_Gpt1BlockPrescaler_8
                                                                (b) IfxGpt12_TimerInputPrescaler_1
                                                                Reload_Value = fGPT / (block Prescaler * input prescaler * irQfreq)
                                                                Reload_Value = (fGPT  * IRQ_duration) *  / (block Prescaler * input prescaler)
                                                                Reload_Value = (100000000*0,00005)/(8*1)=625
                                                                Response time in factor of 50us ( value 2 = 100us) max val of 65535 will result of timer of ~3,27s */

// Calculation formula:
// T = desired IRQ period in seconds
// Reload Value = f_GPT * T / (Gpt1BlockPrescaler ∗ TimerInputPrescaler)
// where f_GPT = 100 MHz --> GPT12 module base frequency,
// T, s - interrupt periodicity
// Gpt1BlockPrescaler - GPT1 block prescaler = 8
// TimerInputPrescaler - timer prescaler = 2
// Reload Value should fit in 16 bit unsigned int!

// T desired = 0,01 --> 10ms
// reload value = (100000000*0,01)/(16*2) = 31250

// T desired = 0,0001 --> 50us
// reload value = (100000000*0,00005)/(8*1) = 625

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
 * \param wdType type of WD (valid options WD1 and WD2)
 * \param watchdogPeriodicity periodicity of acknowledging WD
 * \return Returns nothing
 */
void ConfigureWatchdogPeriodicity(WatchdogTypeEnum wdType, uint16 watchdogPeriodicity);

/** \brief Configure ASIC watchdog status check periodicity
 * Periodicity is defined in number of General timer interrupts
 *
 * \param watchdogStatusCheckPeriodicity periodicity of reading chunks of data
 * \return Returns nothing
 */
void ConfigureWatchdogStatusCheckPeriodicity(uint16 watchdogStatusCheckPeriodicity);

/** \brief Configure Test mode 1/2 check periodicity
 * Periodicity is defined in number of General timer interrupts
 *
 * \param testModePeriodicity periodicity of checking test mode results
 * \return Returns nothing
 */
void ConfigureTestModePeriodicity(uint16 testModePeriodicity);

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
 * \param wdType type of WD (valid options WD1 and WD2)
 * \return Returns nothing
 */
void EnableWatchdogInterrupt(WatchdogTypeEnum wdType);

/** \brief Enable Watchdog status check interrupt
 * Periodicity has to be configured first!
 *
 * \return Returns nothing
 */
void EnableWatchdogStatusCheckInterrupt(void);

/** \brief Enable Test mode 1/2 check interrupt
 * Periodicity has to be configured first!
 *
 * \return Returns nothing
 */
void EnableTestModeInterrupt(void);

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
 * \param wdType type of WD (valid options WD1 and WD2)
 * \return Returns nothing
 */
void DisableWatchdogInterrupt(WatchdogTypeEnum wdType);

/** \brief Disable Watchdog status check interrupt
 *
 * \return Returns nothing
 */
void DisableWatchdogStatusCheckInterrupt(void);

/** \brief Disable Test mode 1/2 check interrupt
 *
 * \return Returns nothing
 */
void DisableTestModeInterrupt(void);

/** \brief Disable Fast interrupt
 *
 * \return Returns nothing
 */
void DisableFastInterrupt(void); // TODO: for removal

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

/** \brief Get watchdog acknowledgement interrupt state
 *
 * \param wdType type of WD (valid options WD1 and WD2)
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetStateWatchdogInterrupt(WatchdogTypeEnum wdType);

/** \brief Get watchdog status check interrupt state
 *
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetStateWatchdogStatusCheckInterrupt(void);

/** \brief Get error check interrupt state
 *
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetStateErrorCheckInterrupt(void);

/** \brief Get continuous reading interrupt state
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
 * \param wdType type of WD (valid options WD1 and WD2)
 * \return Returns value of g_watchdogReload variable
 */
uint16 GetWatchdogPeriodicity(WatchdogTypeEnum wdType);

#endif /* TIMER_H_ */
