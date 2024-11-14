/**********************************************************************************************************************
 * \file general_cmd.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef GENERAL_CMD_H_
#define GENERAL_CMD_H_

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

/** \brief Handling IS_ALIVE command from PC
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdIsAlive(USBReceiveData const * const commandPackage);

/** \brief Handling GET MCU VERSION  command from PC
 *
 * \param commandPackage package with command, command not used
 * \returns 3 bytes for current Version VERSION_MAJOR VERSION_MINOR VERSION_PATCH
 */
void CmdGetMcuVersion(USBReceiveData const * const commandPackage);

/** \brief Handling GET MCU DATE  command from PC
 *
 * \param commandPackage package with command, command not used
 * \return 8 byte build date yyyymmdd
 */
void CmdGetMcuBuildDate(USBReceiveData const * const commandPackage);

/** \brief Handling GET MCU TIME  command from PC
 *
 * \param commandPackage package with command, command not used
 * \return 4 byte build time hhmm
 */
void CmdGetMcuBuildTime(USBReceiveData const * const commandPackage);

/** \brief Handling of command USB_CMD_READ_DEV_ID
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdGetDeviceId(USBReceiveData const * const commandPackage);

/** \brief Handling WRITE_REG command from PC
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdWriteReg(USBReceiveData const * const commandPackage);

/** \brief Handling READ_REG_RAW command from PC
 *
 * \param commandPackage package with command
 * \return Nothing
 */
void CmdReadReg(USBReceiveData const * const commandPackage);

void CmdSendRawData(USBReceiveData const * const commandPackage);

#endif /* GENERAL_CMD_H_ */
