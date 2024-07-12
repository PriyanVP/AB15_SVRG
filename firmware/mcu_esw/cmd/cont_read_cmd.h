/**********************************************************************************************************************
 * \file cont_read.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef CONT_READ_CMD_H_
#define CONT_READ_CMD_H_

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

/** \brief Handling USB_CMD_CONFIGURE_READING command from PC
 * Populates read sequence for continuous read feature. Allows enable of continuous read
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdConfigureReading(USBReceiveData const * const commandPackage);

/** \brief Handling USB_CMD_START_READING command from PC
 * Starts continuous reading if it was configured. Enables continuous read interrupts
 * Msg_id of this command will be used in responses of read iterations
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdStartReading(USBReceiveData const * const commandPackage);

/** \brief Handling USB_CMD_STOP_READING command from PC
 * Stops continuous reading. Disables continuous read interrupts
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdStopReading(USBReceiveData const * const commandPackage);

/** \brief Handling INT_CMD_CONT_READ command
 * Performs continuous read iteration (single read of set of registers) if it was configured
 * Msg_id of response is set by start reading.
 *
 * \return Nothing
 */
void IntCmdContRead(void);

#endif /* CONT_READ_CMD_H_ */
