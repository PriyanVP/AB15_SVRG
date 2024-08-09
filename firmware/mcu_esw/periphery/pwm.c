/**********************************************************************************************************************
 * \file pwm.c
 * \copyright Copyright (C) RobertBosch GmbH
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "IfxCcu6_PwmHl.h"
#include "pwm.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/
#define CCU6_BASE_FREQUENCY	    100000000                             /* CCU6 base frequency, in Hertz               */
#define PWM_FREQUENCY_4MHZ        4000000                             /* PWM signal frequency, in Hertz              */
#define PWM_FREQUENCY_2MHZ        2000000                             /* PWM signal frequency, in Hertz              */


#define NUMBER_OF_CHANNELS      3                                     /* Number of channels for capture-compare unit */

//#define PWM_PERIOD              (CCU6_BASE_FREQUENCY / PWM_FREQUENCY)      /* PWM signal period, in ticks          */
//#define DUTY_CYCLE     50                                          /* PWM Signal 1 Duty cycle, in percent  */
//#define COMPARE_VALUE   ((PWM_PERIOD / 100) * (100 - DUTY_CYCLE))

#define COMPARE_VALUE_4MHZ   12                                              /* by using formular above, the PWM_PERIOD
                                                                                will be 25 for 4MHZ, a compare value of
                                                                                12.5 would be necessary, only full
                                                                                numbers are possible -->
                                                                                duty cycle will be 52%               */
#define COMPARE_VALUE_2MHZ   25                                             /*  set duty cycle to 50%                */

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/
IfxCcu6_TimerWithTrigger g_timer;
IfxCcu6_PwmHl g_driver;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void SetDefaultPWMFrequency(void)
{
    SetPWMGeneration2MHZ();
}

void StartPWMGeneration(void)
{
    IfxCcu6_TimerWithTrigger_run(&g_timer);
}

void SetPWMGeneration2MHZ(void)
{
    // TODO: may be refactored into same function - both 2 and 4 MHz
    boolean interruptState = IfxCpu_disableInterrupts();            /* Disable global interrupts                    */

    /* Timer configuration: timer used as counter */
    IfxCcu6_TimerWithTrigger_Config timerConf;
    IfxCcu6_TimerWithTrigger_initConfig(&timerConf, &MODULE_CCU60); /* Initialize the timer configuration with
                                                                     * default values                               */
    /* User timer configuration */
    timerConf.base.frequency = PWM_FREQUENCY_2MHZ;                       /* Set the desired frequency for the PWM signal */
    timerConf.base.countDir = IfxStdIf_Timer_CountDir_down;          /* Configure the timer to count down   */
    /* Initialize the timer driver */
    IfxCcu6_TimerWithTrigger_init(&g_timer, &timerConf);

    /* PWM High/Low driver configuration */
    IfxCcu6_PwmHl_Config pwmHlConf;
    IfxCcu6_PwmHl_initConfig(&pwmHlConf);                           /* Initialize the PwmHl configuration with
                                                                     * default values                               */
    /* User PWM High/Low driver configuration */
    pwmHlConf.timer = &g_timer;                                     /* Use the already configured timer             */
    pwmHlConf.base.channelCount = NUMBER_OF_CHANNELS;               /* Configure the driver to use use all three
                                                                     * compare modules available in T12             */

    /* Assign output pins: generation of (complementary) signals */
    pwmHlConf.cout2 = &IfxCcu60_COUT62_P02_5_OUT;

    /* Initialize the PwmHl driver */
    IfxCcu6_PwmHl_init(&g_driver, &pwmHlConf);

    /* Instruct the driver to generate center aligned PWM signals */
    IfxCcu6_PwmHl_setMode(&g_driver, Ifx_Pwm_Mode_leftAligned);

    /* Set the duty cycles for the three channels */
    Ifx_TimerValue cmpValues[NUMBER_OF_CHANNELS];
    cmpValues[0] = COMPARE_VALUE_2MHZ;                          /* Set the compare value for channel 1          */
    cmpValues[1] = COMPARE_VALUE_2MHZ;                          /* Set the compare value for channel 2          */
    cmpValues[2] = COMPARE_VALUE_2MHZ;                          /* Set the compare value for channel 3          */

    g_driver.update(&g_driver, cmpValues);                 /* Apply the compare values                     */

    /* Update the timer.
     * This instruction enables the shadow transfer of the compare values, copying the compare values to the
     * compare registers */
    IfxCcu6_TimerWithTrigger_applyUpdate(g_driver.timer);

    /* Restore interrupts to their initial state */
    IfxCpu_restoreInterrupts(interruptState);
}

void SetPWMGeneration4MHZ(void)
{
    boolean interruptState = IfxCpu_disableInterrupts();            /* Disable global interrupts                    */

    /* Timer configuration: timer used as counter */
    IfxCcu6_TimerWithTrigger_Config timerConf;
    IfxCcu6_TimerWithTrigger_initConfig(&timerConf, &MODULE_CCU60); /* Initialize the timer configuration with
                                                                     * default values                               */
    /* User timer configuration */
    timerConf.base.frequency = PWM_FREQUENCY_4MHZ;                       /* Set the desired frequency for the PWM signal */
    //timerConf.base.countDir = IfxStdIf_Timer_CountDir_upAndDown;    /* Configure the timer to count up and down, in
    timerConf.base.countDir = IfxStdIf_Timer_CountDir_down;          /* Configure the timer to count down   */
    /* Initialize the timer driver */
    IfxCcu6_TimerWithTrigger_init(&g_timer, &timerConf);

    /* PWM High/Low driver configuration */
    IfxCcu6_PwmHl_Config pwmHlConf;
    IfxCcu6_PwmHl_initConfig(&pwmHlConf);                           /* Initialize the PwmHl configuration with
                                                                     * default values                               */
    /* User PWM High/Low driver configuration */
    pwmHlConf.timer = &g_timer;                                     /* Use the already configured timer             */
    pwmHlConf.base.channelCount = NUMBER_OF_CHANNELS;               /* Configure the driver to use use all three
                                                                     * compare modules available in T12             */


    /* Assign output pins: generation of (complementary) signals */
    pwmHlConf.cout2 = &IfxCcu60_COUT62_P02_5_OUT;

    /* Initialize the PwmHl driver */
    IfxCcu6_PwmHl_init(&g_driver, &pwmHlConf);

    /* Instruct the driver to generate center aligned PWM signals */
    IfxCcu6_PwmHl_setMode(&g_driver, Ifx_Pwm_Mode_leftAligned);

    /* Set the duty cycles for the three channels */
    Ifx_TimerValue cmpValues[NUMBER_OF_CHANNELS];
    cmpValues[0] = COMPARE_VALUE_4MHZ;                          /* Set the compare value for channel 1          */
    cmpValues[1] = COMPARE_VALUE_4MHZ;                          /* Set the compare value for channel 2          */
    cmpValues[2] = COMPARE_VALUE_4MHZ;                          /* Set the compare value for channel 3          */

    g_driver.update(&g_driver, cmpValues);                 /* Apply the compare values                     */

    /* Update the timer.
     * This instruction enables the shadow transfer of the compare values, copying the compare values to the
     * compare registers */
    IfxCcu6_TimerWithTrigger_applyUpdate(g_driver.timer);

    /* Restore interrupts to their initial state */
    IfxCpu_restoreInterrupts(interruptState);
}