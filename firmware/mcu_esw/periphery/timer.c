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
//extern void ErrorCheckInterruptRoutine(void);
//extern void ContinuousReadInterruptRoutine(void);
//extern void GPIOInterruptRoutine(void);
//
///** \brief General timer interrupt routine
// * Implements timers for ASIC watchdog, error check and continuous read
// *
// * \return Returns nothing
// */
void UpdateTimersRoutine(void);


/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static uint16 g_watchdog1Reload;                  /** \brief Watchdog acknowledge periodicity in T3 timer interrupts */
static uint16 g_watchdog2Reload;                  /** \brief Watchdog acknowledge periodicity in T3 timer interrupts */
static uint16 g_errorCheckReload;                 /** \brief Error check periodicity in T2 timer interrupts          */
static uint16 g_continuousReadReload;             /** \brief Continuous read periodicity in T2 timer interrupts      */
static uint16 g_GPIOReload;                       /** \brief GPIO handling periodicity in T2 timer interrupts        */

static boolean g_watchdog1Enable      = FALSE;    /** \brief Watchdog 1 acknowledge interrupts enable                */
static boolean g_watchdog2Enable      = FALSE;    /** \brief Watchdog 2 acknowledge interrupts enable                */
static boolean g_errorCheckEnable     = FALSE;    /** \brief Error check interrupts enable                           */
static boolean g_continuousReadEnable = FALSE;    /** \brief Continuous read interrupts enable                       */
static boolean g_GPIOEnable           = FALSE;    /** \brief GPIO interrupts enable                                  */

/*********************************************************************************************************************/
/*--------------------------------------------Function Implementations-----------------------------------------------*/
/*********************************************************************************************************************/
/* Macro defining the Interrupt Service Routines */
IFX_INTERRUPT(UpdateTimersRoutine, 0, ISR_PRIORITY_GPT1_T3_TIMER);
//IFX_INTERRUPT(ServiceTimerRoutineWrapper, 0, ISR_PRIORITY_GPT1_T3_TIMER);


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

void ConfigureWatchdogPeriodicity(WatchdogTypeEnum wdType, uint16 watchdogPeriodicity)
{
    if (wdType == WD1)
    {
        g_watchdog1Reload = watchdogPeriodicity;
    }
    else if (wdType == WD2)
    {
        g_watchdog2Reload = watchdogPeriodicity;
    } 
}

void ConfigureErrorCheckPeriodicity(uint16 errorCheckPeriodicity)
{
    g_errorCheckReload = errorCheckPeriodicity;
}

void ConfigureContinuousReadPeriodicity(uint16 continuousReadPeriodicity)
{
    g_continuousReadReload = continuousReadPeriodicity;
}

void ConfigureGPIOPeriodicity(uint16 gpioPeriodicity)
{
    g_GPIOReload = gpioPeriodicity;
}


void EnableWatchdogInterrupt(WatchdogTypeEnum wdType)
{  
    if (wdType == WD1)
    {
        g_watchdog1Enable = TRUE;
    }
    else if (wdType == WD2)
    {
        g_watchdog2Enable = TRUE;
    } 
}

void EnableErrorCheckInterrupt(void)
{
    g_errorCheckEnable = TRUE;
}

void EnableContinuousReadInterrupt(void)
{
    g_continuousReadEnable = TRUE;
}

void EnableGPIOInterrupt(void)
{
    g_GPIOEnable = TRUE;
}

void DisableWatchdogInterrupt(WatchdogTypeEnum wdType)
{  
    if (wdType == WD1)
    {
        g_watchdog1Enable = FALSE;
    }
    else if (wdType == WD2)
    {
        g_watchdog2Enable = FALSE;
    } 
}

void DisableErrorCheckInterrupt(void)
{
    g_errorCheckEnable = FALSE;
}

void DisableContinuousReadInterrupt(void)
{
    g_continuousReadEnable = FALSE;
}

void DisableGPIOInterrupt(void)
{
    g_GPIOEnable = FALSE;
}

boolean GetStateWatchdogInterrupt(WatchdogTypeEnum wdType)
{
    boolean isEnabled;

    if (wdType == WD1)
    {
        isEnabled = g_watchdog1Enable;
    }
    else if (wdType == WD2)
    {
        isEnabled = g_watchdog2Enable;
    } 

    return isEnabled;
}

boolean GetStateErrorCheckInterrupt(void)
{
    return g_errorCheckEnable;
}

boolean GetStateContinuousReadInterrupt(void)
{
    return g_continuousReadEnable;
}

boolean GetStateGPIOInterrupt(void)
{
    return g_GPIOEnable;
}

uint16 GetWatchdogPeriodicity(WatchdogTypeEnum wdType)
{
    uint16 watchdogReload = 0;

    if (wdType == WD1)
    {
        watchdogReload = g_watchdog1Reload;
    }
    else if (wdType == WD2)
    {
        watchdogReload = g_watchdog2Reload;
    } 

    return watchdogReload;
}

void UpdateTimersRoutine(void)
{
    // Static variables to simulate separate timers
    static uint16 watchdog1Counter          = 0;
    static uint16 watchdog2Counter          = 0;
    static uint16 errorCheckCounter         = 0;
    static uint16 continuousReadCounter     = 0;
    static uint16 GPIOCounter               = 0;

    // Increment variables if entered interrupt routine
    // TODO:  Prescale changed from 2 to 1, so all couters need to be doulbed! --> IfxGpt12_TimerInputPrescaler_1

    watchdog1Counter++;
    watchdog2Counter++;
    errorCheckCounter++;
    continuousReadCounter++;
    GPIOCounter++;

    // Call corresponding functions if enabled and counter reached reload value
    if ((g_watchdog1Enable == TRUE) && (watchdog1Counter >= g_watchdog1Reload))
    {
        // Watchdog acknowledge
        watchdog1Counter = 0;
        Watchdog1InterruptRoutine();
    }

    // Call corresponding functions if enabled and counter reached reload value
    if ((g_watchdog2Enable == TRUE) && (watchdog2Counter >= g_watchdog2Reload))
    {
        // Watchdog acknowledge
        watchdog2Counter = 0;
        Watchdog2InterruptRoutine();
    }

//    if ((g_errorCheckEnable == TRUE) && (errorCheckCounter >= g_errorCheckReload))
//    {
//        // Continuous ASIC error check
//        errorCheckCounter = 0;
//        //ErrorCheckInterruptRoutine();
//    }
//
//    if ((g_continuousReadEnable == TRUE) && (continuousReadCounter >= g_continuousReadReload))
//    {
//        // Continuous registers reading
//        continuousReadCounter = 0;
//        //ContinuousReadInterruptRoutine();
//    }
//
//    if ((g_GPIOEnable == TRUE) && (GPIOCounter >= g_GPIOReload))
//    {
//        // GPIO handling
//        GPIOCounter = 0;
//        //GPIOInterruptRoutine();
//    }
}


void ServiceTimerRoutineWrapper(void)
{
    // Call interrupt routine function from other file
    //TODO. Inhibited
    //ServiceInterruptRoutine();
    ToggleLED2();
}
