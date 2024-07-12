/**********************************************************************************************************************
 * \file button.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *
 *********************************************************************************************************************/
#ifndef BUTTON_H_
#define BUTTON_H_

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Function to configure the port pins for the push button and the LED
 *
 * \return Nothing
 */
void InitButton(void);

/** \brief Depending on the the state of the "BUTTON" port pin, the LED is turned on or off
 *
 * \return TODO: add info about output meaning
 */
uint8 GetButtonState(void);

#endif /* BUTTON_H_ */
