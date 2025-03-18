/**********************************************************************************************************************
 * \file timer.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "IfxGpt12.h"
#include "IfxPort.h"
#include "periphery/usb.h"
#include "periphery/led.h"
#include "common/Ifx_IntPrioDef.h"
#include "common/watchdog_types.h"
#include "Bsp.h"

#include "timer.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/


//extern void FastInterruptRoutine(void);
extern void Watchdog1InterruptRoutine(void);
extern void Watchdog2InterruptRoutine(void);
extern void WatchdogStatusReadingInterruptRoutine(void);
extern void TestModeInterruptRoutine(void);
extern void HackedTimerInterruptRoutine(void);
extern void FLMDiagInterruptRoutine(void);

///** \brief General timer interrupt routine
// * Implements timers for ASIC watchdogs, watchdog status, error check and other virtual timers
// * with GENERAL_TIMER_PERIODICITY = 625u MCU will trigger an interrupt each  50us*/
// * Response time in factor of 50us ( value 2 = 100us) max val of 65535 will result of timer of ~3,27s */
// *
// * \return Returns nothing
// */
void UpdateTimersRoutine(void);


/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static uint16 g_watchdog1Reload;                  /** \brief Watchdog acknowledge periodicity in T3 timer interrupts */
static uint16 g_watchdog2Reload;                  /** \brief Watchdog acknowledge periodicity in T3 timer interrupts */
static uint16 g_watchdogStatusCheckReload;        /** \brief Watchdog status check periodicity in T3 timer interrupts*/
static uint16 g_testModeReload;                   /** \brief Test mode check periodicity in T3 timer interrupts      */
static uint16 g_hackedTimerReload;                /** \brief       */
static uint16 g_flmDiagReload;                    /** \brief FLM cyclic diagnostic interrupts                        */

static boolean g_watchdog1Enable            = FALSE;    /** \brief Watchdog 1 acknowledge interrupts enable          */
static boolean g_watchdog2Enable            = FALSE;    /** \brief Watchdog 2 acknowledge interrupts enable          */
static boolean g_watchdogStatusCheckEnable  = FALSE;    /** \brief Watchdog status reading interrupts enable         */
static boolean g_testModeEnable             = FALSE;    /** \brief Test mode check interrupts enable                 */
static boolean g_hackedTimerEnable          = FALSE;    /** \brief                  */
static boolean g_flmDiagEnable              = FALSE;    /** \brief FLM cyclic diagnostic interrupts enable           */

/*********************************************************************************************************************/
/*--------------------------------------------Function Implementations-----------------------------------------------*/
/*********************************************************************************************************************/
/* Macro defining the Interrupt Service Routines */
IFX_INTERRUPT(UpdateTimersRoutine, 0, ISR_PRIORITY_GPT1_T3_TIMER);



void InitGpt12Timer(void)
{
    /* Initialize the GPT12 module */
    IfxGpt12_enableModule(&MODULE_GPT120);                                          /* Enable the GPT12 module      */
    IfxGpt12_setGpt1BlockPrescaler(&MODULE_GPT120, IfxGpt12_Gpt1BlockPrescaler_8); /* Set GPT1 block prescaler     */

    /* Initialize the Timer T3 - General timer*/
    IfxGpt12_T3_setTimerPrescaler(&MODULE_GPT120, IfxGpt12_TimerInputPrescaler_1); /* Set T3 input prescaler        */
    IfxGpt12_T3_setMode(&MODULE_GPT120, IfxGpt12_Mode_timer);                       /* Set T3 to timer mode         */
    IfxGpt12_T3_setTimerDirection(&MODULE_GPT120, IfxGpt12_TimerDirection_down);    /* Set T3 count direction       */
    IfxGpt12_T3_setTimerValue(&MODULE_GPT120, 0x0001);           /* Set T3 start value, does not affect periodicity  */

    /* Initialize the Timer T2 to provide reload value  */
    IfxGpt12_T2_setMode(&MODULE_GPT120, IfxGpt12_Mode_reload);                       /* Set T2 to timer mode         */
    IfxGpt12_T2_setReloadInputMode(&MODULE_GPT120, IfxGpt12_ReloadInputMode_bothEdgesTxOTL);
    //IfxGpt12_T2_setTimerDirection(&MODULE_GPT120, IfxGpt12_TimerDirection_down);    /* Set T2 count direction       */
    //IfxGpt12_T2_setTimerPrescaler(&MODULE_GPT120, IfxGpt12_TimerInputPrescaler_2);  /* Set T2 input prescaler       */
    IfxGpt12_T2_setTimerValue(&MODULE_GPT120, GENERAL_TIMER_PERIODICITY);           /* Set T2 reload value           */

    /* Initialize the main tick interrupt */
    volatile Ifx_SRC_SRCR *src3 = IfxGpt12_T3_getSrc(&MODULE_GPT120);                /* Get the interrupt address    */
    IfxSrc_init(src3, ISR_PROVIDER_GPT12_TIMER, ISR_PRIORITY_GPT1_T3_TIMER);         /* Initialize service request   */
    IfxSrc_enable(src3);                                                             /* Enable GPT12 interrupt       */
}

void StartGeneralTimer(void)
{
    IfxGpt12_T3_run(&MODULE_GPT120, IfxGpt12_TimerRun_start);
}

void StopGeneralTimer(void)
{
    IfxGpt12_T3_run(&MODULE_GPT120, IfxGpt12_TimerRun_stop);
}

void ConfigureTimerPeriodicity(TimerTypeEnum timerType, uint16 periodicity)
{
    switch (timerType)
    {
        case WATCHDOG1_TIMER:
            g_watchdog1Reload = periodicity;
            break;
        case WATCHDOG2_TIMER:
            g_watchdog2Reload = periodicity;
            break;
        case WATCHDOG_STATUS_CHECK_TIMER:
            g_watchdogStatusCheckReload = periodicity;
            break;
        case FLM_DIAG_TIMER:
            g_flmDiagReload = periodicity;
            break;
        case HACKED_TIMER:
            g_hackedTimerReload = periodicity;
            break;
        case TEST_MODE_TIMER:
            g_testModeReload = periodicity;
            break;
        default:
            break;
    }
}

void EnableTimerInterrupt(TimerTypeEnum timerType)
{
    switch (timerType)
    {
        case WATCHDOG1_TIMER:
            g_watchdog1Enable = TRUE;
            break;
        case WATCHDOG2_TIMER:
            g_watchdog2Enable = TRUE;
            break;
        case WATCHDOG_STATUS_CHECK_TIMER:
            g_watchdogStatusCheckEnable = TRUE;
            break;
        case FLM_DIAG_TIMER:
            g_flmDiagEnable = TRUE;
            break;
        case HACKED_TIMER:
            g_hackedTimerEnable = TRUE;
            break;
        case TEST_MODE_TIMER:
            g_testModeEnable = TRUE;
            break;
        default:
            break;
    }
}

void DisableTimerInterrupt(TimerTypeEnum timerType)
{
    switch (timerType)
    {
        case WATCHDOG1_TIMER:
            g_watchdog1Enable = FALSE;
            break;
        case WATCHDOG2_TIMER:
            g_watchdog2Enable = FALSE;
            break;
        case WATCHDOG_STATUS_CHECK_TIMER:
            g_watchdogStatusCheckEnable = FALSE;
            break;
        case FLM_DIAG_TIMER:
            g_flmDiagEnable = FALSE;
            break;
        case HACKED_TIMER:
            g_hackedTimerEnable = FALSE;
            break;
        case TEST_MODE_TIMER:
            g_testModeEnable = FALSE;
            break;
        default:
            break;
    }
}

boolean GetTimerState(TimerTypeEnum timerType)
{
    boolean isEnabled;

    switch (timerType)
    {
        case WATCHDOG1_TIMER:
            isEnabled = g_watchdog1Enable;
            break;
        case WATCHDOG2_TIMER:
            isEnabled = g_watchdog2Enable;
            break;
        case WATCHDOG_STATUS_CHECK_TIMER:
            isEnabled = g_watchdogStatusCheckEnable;
            break;
        case FLM_DIAG_TIMER:
            isEnabled = g_flmDiagEnable;
            break;
        case HACKED_TIMER:
            isEnabled = g_hackedTimerEnable;
            break;
        case TEST_MODE_TIMER:
            isEnabled = g_testModeEnable;
            break;
        default:
            isEnabled = FALSE;
            break;
    }

    return isEnabled;
}

uint16 GetTimerPeriodicity(TimerTypeEnum timerType)
{
    uint16 periodicity;

    switch (timerType)
    {
        case WATCHDOG1_TIMER:
            periodicity = g_watchdog1Reload;
            break;
        case WATCHDOG2_TIMER:
            periodicity = g_watchdog2Reload;
            break;
        case WATCHDOG_STATUS_CHECK_TIMER:
            periodicity = g_watchdogStatusCheckReload;
            break;
        case FLM_DIAG_TIMER:
            periodicity = g_flmDiagReload;
            break;
        case HACKED_TIMER:
            periodicity = g_hackedTimerReload;
            break;
        case TEST_MODE_TIMER:
            periodicity = g_testModeReload;
            break;
        default:
            periodicity = 0;
            break;
    }

    return periodicity;
}

void UpdateTimersRoutine(void)
{
    // Static variables to simulate separate timers
    static uint16 watchdog1Counter              = 0;
    static uint16 watchdog2Counter              = 0;
    static uint16 watchdogStatusCheckCounter    = 0;
    static uint16 testModeCounter               = 0;
    static uint16 hackedTimerCounter            = 0;
    static uint16 FLMDiagCounter                = 0;

    // Call corresponding functions if enabled and counter reached reload value
    if (g_watchdog2Enable == TRUE)
    {
        watchdog2Counter++;

        if (watchdog2Counter >= g_watchdog2Reload)
        {
            // Watchdog acknowledge
            watchdog2Counter = 0;
            Watchdog2InterruptRoutine();
        }
    }

    // Call corresponding functions if enabled and counter reached reload value
    if (g_watchdog1Enable == TRUE)
    {
        watchdog1Counter++;

        if (watchdog1Counter >= g_watchdog1Reload)
        {
            // Watchdog acknowledge
            watchdog1Counter = 0;
            Watchdog1InterruptRoutine();
        }
    }

    // Call corresponding functions if enabled and counter reached reload value
    if (g_watchdogStatusCheckEnable == TRUE)
    {
        watchdogStatusCheckCounter++;

        if (watchdogStatusCheckCounter >= g_watchdogStatusCheckReload)
        {
            // Watchdog acknowledge
            watchdogStatusCheckCounter = 0;
            WatchdogStatusReadingInterruptRoutine();
        }
    }

    // Call corresponding functions if enabled and counter reached reload value
    if (g_testModeEnable == TRUE)
    {
        testModeCounter++;

        if (testModeCounter >= g_testModeReload)
        {
            // Test mode periodic check
            testModeCounter = 0;
            TestModeInterruptRoutine();
        }
    }

    // Call corresponding functions if enabled and counter reached reload value
    if (g_hackedTimerEnable == TRUE)
    {
        hackedTimerCounter++;

        if (hackedTimerCounter >= g_hackedTimerReload)
        {
            // Test mode periodic check
            hackedTimerCounter = 0;
            HackedTimerInterruptRoutine();
        }
    }

    // Call corresponding functions if enabled and counter reached reload value
    if (g_flmDiagEnable == TRUE)
    {
        FLMDiagCounter++;

        if (FLMDiagCounter >= g_flmDiagReload)
        {
            // Check FLM diagnostics execution results
            FLMDiagCounter = 0;
            FLMDiagInterruptRoutine();
        }
    }

}
