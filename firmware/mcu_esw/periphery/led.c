/**********************************************************************************************************************
 * \file led.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "IfxPort.h"
#include "led.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/* onboard LED -> overlaps with SPI function (share pin with SCK) we dont use it.*/
//#define LED                          &MODULE_P10,2                           /* LED Port Pin                         */
/* added 4 LEDs on breadboard
 * 1= red;  2= yel; 3= gn; 4 = blue
 * */
#define LED1                         &MODULE_P02,0                           /* LED Port Pin                         */
#define LED2                         &MODULE_P02,1                           /* LED Port Pin                         */
#define LED3                         &MODULE_P02,3                           /* LED Port Pin                         */
#define LED4                         &MODULE_P02,4                           /* LED Port Pin                         */


/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void InitLEDs(void)
{
    /* Initialize GPIO pins for the LED */
    IfxPort_setPinMode(LED1, IfxPort_Mode_outputPushPullGeneral);
    IfxPort_setPinMode(LED2, IfxPort_Mode_outputPushPullGeneral);
    IfxPort_setPinMode(LED3, IfxPort_Mode_outputPushPullGeneral);
    IfxPort_setPinMode(LED4, IfxPort_Mode_outputPushPullGeneral);

    /* Turn off the LED */
    IfxPort_setPinState(LED1, IfxPort_State_low);
    IfxPort_setPinState(LED2, IfxPort_State_low);
    IfxPort_setPinState(LED3, IfxPort_State_low);
    IfxPort_setPinState(LED4, IfxPort_State_low);

}

void OnLED1(void)
{
    IfxPort_setPinState(LED1, IfxPort_State_high);
}

void OffLED1(void)
{
    IfxPort_setPinState(LED1, IfxPort_State_low);
}

void ToggleLED1(void)
{
    IfxPort_setPinState(LED1, IfxPort_State_toggled);
}


void OnLED2(void)
{
    IfxPort_setPinState(LED2, IfxPort_State_high);
}

void OffLED2(void)
{
    IfxPort_setPinState(LED2, IfxPort_State_low);
}

void ToggleLED2(void)
{
    IfxPort_setPinState(LED2, IfxPort_State_toggled);
}


void OnLED3(void)
{
    IfxPort_setPinState(LED3, IfxPort_State_high);
}

void OffLED3(void)
{
    IfxPort_setPinState(LED3, IfxPort_State_low);
}

void ToggleLED3(void)
{
    IfxPort_setPinState(LED3, IfxPort_State_toggled);
}


void OnLED4(void)
{
    IfxPort_setPinState(LED4, IfxPort_State_high);
}

void OffLED4(void)
{
    IfxPort_setPinState(LED4, IfxPort_State_low);
}

void ToggleLED4(void)
{
    IfxPort_setPinState(LED4, IfxPort_State_toggled);
}

/* create some blink funktion
 * if triggered in 2ms cycle, led  will blink in 1Hz rate */
void Blink_LED1_1Hz(void)
{
    static uint16 cntr;

    cntr++;
    if (cntr >=500){
        cntr = 0;
    }

    if (cntr < 250)
    {
        OnLED1();
    }
    else{
        OffLED1();
    }
}



