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
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Enum for timer types
 */
typedef enum 
{
   WATCHDOG1_TIMER,
   WATCHDOG2_TIMER,
   WATCHDOG_STATUS_CHECK_TIMER,
   FLM_DIAG_TIMER,
   HACKED_TIMER,
   TEST_MODE_TIMER
} TimerTypeEnum;

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

/** \brief Configure periodicity of specified timer
 * Periodicity is defined in number of General timer interrupts
 *
 * \param timerType type of timer
 * \param periodicity periodicity of timer
 * \return Returns nothing
 */
void ConfigureTimerPeriodicity(TimerTypeEnum timerType, uint16 periodicity);

/** \brief Enable specified timer interrupt
 * Periodicity has to be configured first!
 *
 * \param timerType type of timer
 * \return Returns nothing
 */
void EnableTimerInterrupt(TimerTypeEnum timerType);

/** \brief Disable specified timer interrupt
 * 
 * \param timerType type of timer
 * \return Returns nothing
 */
void DisableTimerInterrupt(TimerTypeEnum timerType);

/** \brief Get specified timer interrupt state
 *
 * \param timerType type of timer
 * \return Returns true if irq enabled, false - otherwise
 */
boolean GetTimerState(TimerTypeEnum timerType);

/** \brief Gets duration of specified interrupt period
 *
 * \param timerType type of timer
 * \return Returns value of timer periodicity
 */
uint16 GetTimerPeriodicity(TimerTypeEnum timerType);

#endif /* TIMER_H_ */
