/**********************************************************************************************************************
 * \file gpio_cmd.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef GPIO_CMD_H_
#define GPIO_CMD_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/usb_data_types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

// /** \brief Handling USB_CMD_START_GPIO_READING command from PC
//  * Populates read sequence for GPIO feature. Configure reading periodicity
//  * Msg_id of this command will be used in responses of read iterations
//  *
//  * \param commandPackage package with command
//  * \return Nothing
//  */
// void CmdStartGPIOReading(USBReceiveData const * const commandPackage);

// /** \brief Handling USB_CMD_STOP_GPIO_READING command from PC
//  * Stops GPIO reading if it was configured. Disables GPIO read interrupts
//  *
//  * \param commandPackage package with command
//  * \return Nothing
//  */
// void CmdStopGPIOReading(USBReceiveData const * const commandPackage);

// /** \brief Handling INT_CMD_GPIO_READ command
//  * Performs GPIO read iteration (single read of set of registers) if it was configured
//  * Msg_id of response is set by start reading.
//  *
//  * \return Nothing
//  */
// void IntCmdGPIORead(void);

#endif /* GPIO_CMD_H_ */
