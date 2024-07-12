/**********************************************************************************************************************
 * \file watchdog.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef WATCHDOG_H_
#define WATCHDOG_H_
#define WD_RESP_ADDRESS 0x013
#define WD_CONFIG0_ADDRESS 0x010
#define WD_CONFIG1_ADDRESS 0x011
#define WD_TIME_WIN_ADDRESS 0x014
#define WD_REQU_ADDRESS 0x016
#define WD_RESPTIME_ADDRESS 0x012
#define WD_CFG_PACKAGE_LEN 4

#define REQU_OFFSET 0
#define REQU_READMASK 0xF //0b0000000000001111
#define REQU_WRITEMASK 0xFFF0 //0b1111111111110000
#define RESP_CNT_OFFSET 14
#define RESP_CNT_READMASK 0x4000 //0b0100000000000000

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
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Function to configure CS600 Watchdog with config values from GUI's Watchdog tab and enable Watchdog
 * Corresponds to USB command USB_CMD_CONFIGURE_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdConfigureWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to get values of Watchdog config registers from CS600, and send them to PC to populate corresponding GUI fields
 * Corresponds to USB command USB_CMD_GET_CONFIG_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdGetConfigWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to start serving CS600 Watchdog's requests by MCU
 * Corresponds to USB command USB_CMD_START_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStartWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to disable CS600 Watchdog's serving by MCU
 * Corresponds to USB command USB_CMD_STOP_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStopWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to enable MCU functionality that sends values of Watchdog status registers and time durations between the polls to GUI
 * Corresponds to USB command USB_CMD_START_OBSERVATION_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStartMonitoringWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to disable MCU functionality that sends values of Watchdog status registers and time durations between the polls to GUI
 * Corresponds to USB command USB_CMD_STOP_MONITORING_WATCHDOG
 *
 * \param commandPackage package with command
 * \return Returns nothing
 */
void CmdStopMonitoringWatchdog(USBReceiveData const * const commandPackage);

/** \brief Function to serve CS600 Watchdog's request
 * Corresponds to Internal command INT_CMD_ACK_WATCHDOG
 * \return Returns nothing
 */
void IntCmdServeWatchdog(void);

/** \brief Function that provides a Response word corresponding to Watchdog Request value
 * \param requValue Watchdog Request value
 * \param respWrdNumber Requested Response word (WORD_1 or WORD_0)
 * \return Response word value
 */
uint16 GetResponseWordAb12(uint8 requValue, boolean respWrdNumber);

/** \brief Function that reads value of RESP_CNT field from CS600 WD_REQU register by SPI
 * \return Value of RESP_CNT field
 */
uint8 GetRespCnt(void);

/** \brief Function that reads value of REQU field from CS60 WD_REQU register by SPI
 * \return Value of REQU field
 */
uint8 GetREQUValue(void);

/** \brief Function that calculates periodicity of Watchdog serving procedure
 * \return Period duration of Watchdog serving procedure, in General Timer's interrupts
 */
uint16 CalculateWatchdogAckPeriodicity(void);

#endif /* WATCHDOG_H_ */

