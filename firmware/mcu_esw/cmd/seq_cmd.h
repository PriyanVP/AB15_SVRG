/**********************************************************************************************************************
 * \file seq_cmd.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef SEQ_CMD_H_
#define SEQ_CMD_H_

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

/** \brief Handling USB_CMD_EXECUTE_RW_SEQUENCE command from PC
 * Executes read and write operation sequence
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdExecuteRWSequence(USBReceiveData const * const commandPackage);

/** \brief Handling USB_CMD_EXECUTE_READ_SEQUENCE command from PC
 * Executes read operation sequence
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdExecuteReadSequence(USBReceiveData const * const commandPackage);

/** \brief Handling USB_CMD_EXECUTE_WRITE_SEQUENCE command from PC
 * Executes write operation sequence
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdExecuteWriteSequence(USBReceiveData const * const commandPackage);

#endif /* SEQ_CMD_H_ */
