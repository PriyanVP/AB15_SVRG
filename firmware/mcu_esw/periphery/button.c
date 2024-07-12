/**********************************************************************************************************************
 * \file GPIO_LED_Button.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "IfxPort.h"
#include "button.h"

/*********************************************************************************************************************/
/*-----------------------------------------------------Macros--------------------------------------------------------*/
/*********************************************************************************************************************/
#define BUTTON  &MODULE_P02,5   /* Port pin for the button  */

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void InitButton(void)
{

    /* Setup the port pin connected to the push button to input mode. This function can be used to initialize any
     * port to input mode by just specifying the port number as illustrated.
     */
    IfxPort_setPinMode(BUTTON, IfxPort_Mode_inputPullUp);
}

uint8 GetButtonState(void)
{
    uint8 retval = 0;
    /* With the routine getPinState() the value of a particular pin can be retrieved. This
     * function can be used to retrieve any port state by just specifying the port number
     * as illustrated.
     */
    if(IfxPort_getPinState(BUTTON) == 0)
    {
        /* With the routine setPinState() the state of the port can be set to drive either
         * LOW or HIGH. This function can be used to retrieve any port state by just
         * specifying the port number as illustrated.
         */
        retval = 1;
    }
    else
    {
        retval = 0;
    }
    return(retval);
}
