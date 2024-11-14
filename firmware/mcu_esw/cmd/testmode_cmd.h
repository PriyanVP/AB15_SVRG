/**********************************************************************************************************************
 * \file testmode_cmd.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef TESTMODE_CMD_H_
#define TESTMODE_CMD_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Platform_Types.h"
#include "common/usb_data_types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Function to start Test mode 1 sequence. When finished will send data back
 * 
 * Corresponds to USB command USB_CMD_START_TEST_MODE1
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStartTestMode1(USBReceiveData const * const commandPackage);

/** \brief Function to start Test mode 2 sequence. When finished will send data back
 * 
 * Corresponds to USB command USB_CMD_START_TEST_MODE2
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStartTestMode2(USBReceiveData const * const commandPackage);


/** \brief Function to stop Test mode 1/2 sequences. Intended for special cases, not needed in typical flow
 * 
 * Corresponds to USB command USB_CMD_STOP_TEST_MODE12
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStopTestMode12(USBReceiveData const * const commandPackage);

/** \brief Function to read execute 1 iteration of Test mode sequence
 * 
 * Corresponds to Internal command INT_CMD_EXECUTE_TEST_MODE
 * 
 * \return Returns nothing
 */
void IntCmdExecutePowerstageTest(void);

#endif /* TESTMODE_CMD_H_ */

