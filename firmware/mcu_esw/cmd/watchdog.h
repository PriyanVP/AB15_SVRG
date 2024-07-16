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
#include "common/spi_data_types.h"
#include "common/command_queue.h"
#include "common/bit_manipulation.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "periphery/timer.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Defines available WD
 */
typedef enum
{
    NOT_SET                     = 0,        /** \brief WD type was not yet defined */
    WD1                         = 1,        /** \brief Watchdog 1 */
    WD2                         = 2,        /** \brief Watchdog 2 */
    WD3                         = 3,        /** \brief Watchdog 3 */
    WD12                        = 4         /** \brief Watchdog 1 and 2 */
} WatchdogTypeEnum;

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

#endif /* WATCHDOG_H_ */

