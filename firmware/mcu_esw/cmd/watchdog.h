/**********************************************************************************************************************
 * \file watchdog.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef WATCHDOG_H_
#define WATCHDOG_H_

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

/** \brief Function to configure AB watchdog. Will write configuration to ASIC. 
 * Will save configuration to internal config struct
 * 
 * Corresponds to USB command USB_CMD_CONFIGURE_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdConfigureWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to start serving AB Watchdog's requests by MCU (can start WD1, WD2 or both)
 * 
 * Corresponds to USB command USB_CMD_START_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStartWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to disable AB Watchdog's serving by MCU
 * 
 * Corresponds to USB command USB_CMD_STOP_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStopWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to set MCU External clock output to 2MHz
 *
 * \param commandPackage package without command
 * \return Returns nothing
 */
void CmdSetExtOsc2Mhz(USBReceiveData const * const commandPackage);

/** \brief Function to set MCU External clock output to 4MHz
 *
 * \param commandPackage package without command
 * \return Returns nothing
 */
void CmdSetExtOsc4Mhz(USBReceiveData const * const commandPackage);

/** \brief Function to enable MCU functionality that sends values of Watchdog status registers to PC
 * 
 * Corresponds to USB command USB_CMD_START_MONITORING_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStartMonitoringWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to disable MCU functionality that sends values of Watchdog status registers to PC
 * 
 * Corresponds to USB command USB_CMD_STOP_MONITORING_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStopMonitoringWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to serve AB Watchdog 1 request
 * 
 * Corresponds to Internal command INT_CMD_ACK_WATCHDOG1
 * 
 * \return Returns nothing
 */
void IntCmdAcknowledgeWatchdog1(void);

/** \brief Function to serve AB Watchdog 2 request
 * 
 * Corresponds to Internal command INT_CMD_ACK_WATCHDOG1
 * 
 * \return Returns nothing
 */
void IntCmdAcknowledgeWatchdog2(void);

/** \brief Function to read WD status. Will be periodically called from timer interrupt
 * 
 * Corresponds to Internal command INT_CMD_READ_WD_STATUS
 * 
 * \return Returns nothing
 */
void IntCmdMonitorWatchdog(void);

#endif /* WATCHDOG_H_ */

