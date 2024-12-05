/**********************************************************************************************************************
 * \file gpio.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "IfxPort.h"
#include "IfxPort_PinMap.h"
#include "gpio.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define GPIO_MASK   (0x7F)                                   /** \brief Mask for group modification of all GPIO pins */

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

// GPIO definitions
static Ifx_P *GPIOPort;
static IfxPort_Pin GPIO0;
static IfxPort_Pin GPIO1;
static IfxPort_Pin GPIO2;
static IfxPort_Pin GPIO3;
static IfxPort_Pin GPIO4;
static IfxPort_Pin GPIO5;
static IfxPort_Pin GPIO6;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void InitGPIOPins(void)
{
    // Init references
    GPIOPort = &MODULE_P00;
    GPIO0 = IfxPort_P00_0;
    GPIO1 = IfxPort_P00_1;
    GPIO2 = IfxPort_P00_2;
    GPIO3 = IfxPort_P00_3;
    GPIO4 = IfxPort_P00_4;
    GPIO5 = IfxPort_P00_5;
    GPIO6 = IfxPort_P00_6;

    // Set all pins as input
    IfxPort_setGroupModeInput(GPIOPort, 0, GPIO_MASK, IfxPort_InputMode_noPullDevice);
}

void ConfigureGPIOPin(GPIOPinsEnum pinIdx, IfxPort_Mode mode)
{
    // Configure mode
    IfxPort_setPinMode(GPIOPort, pinIdx, mode);

    // Set pad driver as hardcoded value
    IfxPort_setPinPadDriver(GPIOPort, pinIdx, IfxPort_PadDriver_cmosAutomotiveSpeed1);
}

void SetGPIOPin(GPIOPinsEnum pinIdx)
{
    // Activate pin
    IfxPort_setPinState(GPIOPort, pinIdx, IfxPort_State_high);
}

void ClearGPIOPin(GPIOPinsEnum pinIdx)
{
    // Deactivate pin
    IfxPort_setPinState(GPIOPort, pinIdx, IfxPort_State_low);
}

void ToggleGPIOPin(GPIOPinsEnum pinIdx)
{
    // Activate pin
    IfxPort_setPinState(GPIOPort, pinIdx, IfxPort_State_toggled);
}

boolean GetGPIOPin(GPIOPinsEnum pinIdx)
{
    // Get pin state
    return IfxPort_getPinState(GPIOPort, pinIdx);
}

uint8 GetGPIOPort(void)
{
    // Get all pins states. Read pin states on port from 0 to 6
    return ((uint8) IfxPort_getGroupState(GPIOPort, 0, GPIO_MASK));
}

void SetGPIOPort(uint8 portState)
{
    // Set all pins states. Read pin states on port from 0 to 6
    (IfxPort_setGroupState(GPIOPort, 0, GPIO_MASK, portState));
}
