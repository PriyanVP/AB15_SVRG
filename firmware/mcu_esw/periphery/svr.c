/**********************************************************************************************************************
 * \file svr.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "IfxPort.h"
#include "svr.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define SVR1_PIN    &MODULE_P00,8
#define SVR2_PIN    &MODULE_P00,9

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void InitSVRPins(void)
{
    /* Initialize GPIO pins for the SVR */
    IfxPort_setPinMode(SVR1_PIN, IfxPort_Mode_outputPushPullGeneral);
    IfxPort_setPinMode(SVR2_PIN, IfxPort_Mode_outputPushPullGeneral);

    /* Turn off the SVR pins */
    ClearSVRPin(SVR1);
    ClearSVRPin(SVR2);
}

void SetSVRPin(SVRPinsEnum pinIdx)
{
    switch (pinIdx)
    {
        case SVR1:
            IfxPort_setPinState(SVR1_PIN, IfxPort_State_high);
            break;
        case SVR2:
            IfxPort_setPinState(SVR2_PIN, IfxPort_State_high);
            break;
        default:
            break;
    }

}

void ClearSVRPin(SVRPinsEnum pinIdx)
{
    switch (pinIdx)
    {
        case SVR1:
            IfxPort_setPinState(SVR1_PIN, IfxPort_State_low);
            break;
        case SVR2:
            IfxPort_setPinState(SVR2_PIN, IfxPort_State_low);
            break;
        default:
            break;
    }
}

//Changes for GPIO control transfer to MCU are made below     // Function to handle firing command

//Option 1: Blocks the MCU execution for 500ms

// void HandleFiringCommand(void)
// {
//     //Activating the GPIO
//     SetSVRPin(SVR1);                                                            //Changes for transfer GPIO control from GUI to MCU
//     SetSVRPin(SVR2);

//     //Executing the ASIC firing
//     ExecuteASICFiring();

//     //Waiting for a desired/required duration. In my case I have considered that to be 500ms
//     delay(500);

//     //Deactivating the GPIO
//     ClearSVRPin(SVR1);
//     ClearSVRPin(SVR2);
// }                                        //Changes

//Option 2: Non-blocking Timer (preferable)

// void GPIODeactivationCallback(void) {
//     ClearSVRPin(SVR1);
//     ClearSVRPin(SVR2);     // Callback to deactivate GPIO
// }

// void HandleFiringCommand(void) {
//     SetSVRPin(SVR1);
//     SetSVRPin(SVR2);       // Activate GPIO pins
//     ExecuteASICFiring();    // Trigger ASIC firing sequence
//     StartTimer(500, GPIODeactivationCallback); // Schedule deactivation after 500ms
// }

//Command Processing Logic
void ProcessCommand(uint8_t* commandData)         //Read incoming commands and trigger firing when required
{
    if (IsFiringCommand(commandData)) // Check if the received command is a firing command
    {
        HandleFiringCommand(); // Call the new function to handle firing
    }
}

// Callback function for timer expiration
void GPIODeactivationCallback(void)
{
    ClearSVRPin(SVR1);
    ClearSVRPin(SVR2);
}

// Modified HandleFiringCommand with timer-based deactivation            //Changes
void HandleFiringCommand(void)
{
    SetSVRPin(SVR1);
    SetSVRPin(SVR2);

    ExecuteASICFiring();

    StartTimer(500, GPIODeactivationCallback); // Start a non-blocking timer for 500ms
}
