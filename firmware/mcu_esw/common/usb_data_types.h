/**************************************************************************************************************************
 * \file usb_data_types.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *************************************************************************************************************************/

#ifndef USB_DATA_TYPES_H_
#define USB_DATA_TYPES_H_

/*************************************************************************************************************************/
/*-----------------------------------------------------Includes----------------------------------------------------------*/
/*************************************************************************************************************************/

#include "Ifx_Types.h"

/*************************************************************************************************************************/
/*------------------------------------------------------Macros-----------------------------------------------------------*/
/*************************************************************************************************************************/

#define USB_STARTBYTE_LENGTH            (1)                          /** \brief USB startbyte length in bytes           */
#define USB_MSG_ID_LENGTH               (1)                          /** \brief msg id length in bytes                  */
#define USB_ASIC_ID_LENGTH              (1)                          /** \brief ASIC id length in bytes                 */
#define USB_CMD_LENGTH                  (1)                          /** \brief USB command length in bytes             */
#define USB_PAYLOADLEN_LENGTH           (1)                          /** \brief lenghth of the "payload lenght" information itself in bytes          */
#define USB_CRC_LENGTH                  (1)                          /** \brief USB crc length in bytes                 */
#define USB_STOPBYTE_LENGTH             (1)                          /** \brief USB Endbyte length in bytes             */

#define MAX_USB_RECIEVE_PAYLOAD_LENGTH  (64)                         /** \brief Max USB payload length in bytes          */
#define MAX_USB_TRANSMIT_PAYLOAD_LENGTH (128)                        /** \brief Max USB payload length in bytes          */
#define MIN_USB_TRANSMIT_PAYLOAD_LENGTH (0)                          /** \brief Min USB payload length in bytes          */

#define MAX_USB_PACKAGE_LENGTH          (USB_STARTBYTE_LENGTH + \
                                         USB_MSG_ID_LENGTH + \
                                         USB_ASIC_ID_LENGTH + \
                                         USB_CMD_LENGTH + \
                                         USB_PAYLOADLEN_LENGTH + \
                                         MAX_USB_TRANSMIT_PAYLOAD_LENGTH + \
                                         USB_CRC_LENGTH + \
                                         USB_STOPBYTE_LENGTH)        /** \brief Max USB package length in bytes */


#define MIN_USB_MSG_LENGTH              (USB_STARTBYTE_LENGTH + \
                                         USB_MSG_ID_LENGTH + \
                                         USB_ASIC_ID_LENGTH + \
                                         USB_CMD_LENGTH + \
                                         USB_PAYLOADLEN_LENGTH + \
                                         MIN_USB_TRANSMIT_PAYLOAD_LENGTH + \
                                         USB_CRC_LENGTH + \
                                         USB_STOPBYTE_LENGTH)        /** \brief Min USB message length in bytes          */

#define USB_STARTBYTE_POS               (0)                          /** \brief USB Start Byte field position in package     */
#define USB_MSG_ID_POS                  (1)                          /** \brief USB MSG_ID field position in package     */
#define USB_ASIC_ID_POS                 (2)                          /** \brief USB CMD ASIC ID  position in package */
#define USB_CMD_STAT_POS                (3)                          /** \brief USB CMD/STATUS field position in package */
#define USB_PAYLOAD_LEN_POS             (4)                          /** \brief USB PAYLOAD LEN field position in package*/
#define USB_PAYLOAD_POS                 (5)                          /** \brief USB PAYLOAD field position in package    */

#define USB_RESERVED_ID_1               (0x20)                       /** \brief Reserved ID for MCU messages             */
#define USB_RESERVED_ID_2               (0x21)                       /** \brief Reserved ID for MCU messages             */

/*************************************************************************************************************************/
/*--------------------------------------------------Enumerations---------------------------------------------------------*/
/*************************************************************************************************************************/

/** \brief Defines applicable commands for received USB data TODO: needs clean up, removel of unused commands, updating comments
 * Includes min and max command
 */
typedef enum
{
    _USB_CMD_MIN                        = 0,                  /** \brief minimal value of command (not included) */

    USB_CMD_IS_ALIVE                    = 1,                  /** \brief transaction to check if USB communication works */
    USB_CMD_SPI_INSTRUCTION             = 2,                  /** \brief execute SPI instruction with 16bit input data (send via payload) */
    USB_CMD_READ_DEV_ID                 = 3,                  /** \brief reads ASIC Device ID, reports error if unsuccessful */
    USB_CMD_READ_REG                    = 4,                  /** \brief read single register, output data in raw format, reports error if unsuccessful */
    USB_CMD_WRITE_REG                   = 5,                  /** \brief write single register, output data in raw format, reports error if unsuccessful */
    USB_CMD_CONFIGURE_READING           = 6,                  /** \brief reconfigure continuos reading (table, periodicity, etc.) */
    USB_CMD_START_READING_SEQ           = 7,                  /** \brief start reading defined set of registers */
    USB_CMD_STOP_READING_SEQ            = 8,                  /** \brief stop reading sequence */
    USB_CMD_STOP_WATCHDOG               = 9,                  /** \brief stop to perform periodic Watchdog Request acknowledgement by MCU */
    USB_CMD_EXECUTE_RW_SEQUENCE         = 10,                 /** \brief perform single read/write series of operation */
    USB_CMD_EXECUTE_READ_SEQUENCE       = 11,                 /** \brief perform single read series operation */
    USB_CMD_EXECUTE_WRITE_SEQUENCE      = 12,                 /** \brief perform single write series operation */
    USB_CMD_CONFIGURE_WATCHDOG          = 13,                 /** \brief configure watchdog (Note: initiates read/write seq, configs timer, arms watchdog response) */
    USB_CMD_START_WATCHDOG              = 14,                 /** \brief start to perform periodic Watchdog Request acknowledgement by MCU  */
    USB_CMD_RESET_TO_IDLE               = 15,                 /** \brief stop all continuous operations, set all pins to default state, wait for new command */
    USB_CMD_START_MONITORING_WATCHDOG   = 16,                 /** \brief enables MCU functionality that sends values of Watchdog status registers and time durations between the polls to GUI; continuous command */
    USB_CMD_STOP_MONITORING_WATCHDOG    = 17,                 /** \brief disables MCU functionality that sends values of Watchdog status registers and time durations between the polls to GUI */
    USB_CMD_GET_MCU_VERSION             = 18,                 /** \brief returns MCU SW version */
    USB_CMD_GET_MCU_BUILD_DATE          = 19,                 /** \brief returns MCU SW build date */
    USB_CMD_GET_MCU_BUILD_TIME          = 20,                 /** \brief returns MCU SW build time */
    USB_CMD_SET_EXT_OSC_2MHZ            = 21,                 /** \brief sets external clock output to 2Mhz */
    USB_CMD_SET_EXT_OSC_4MHZ            = 22,                 /** \brief sets external clock output to 4Mhz  */
    USB_CMD_WRITE_RAW_DATA_SPI          = 26,                 /** \brief 0x1A write 32bit raw data to spi */
    USB_CMD_START_TEST_MODE1            = 29,                 /** \brief configure and start Test mode 1  */
    USB_CMD_START_TEST_MODE2            = 30,                 /** \brief configure and start Test mode 2  */
    USB_CMD_STOP_TEST_MODE12            = 31,                 /** \brief stop Test mode (currently running)  */

    USB_CMD_START_HACKED_TIMER          = 32,                 /** \brief  */
    USB_CMD_STOP_HACKED_TIMER           = 33,                 /** \brief   */
    
    
    _USB_EXT_CMD_MAX                    = 63,                 /** \brief maximal value of command send via USB, start of internal commands (generated by MCU) */

    INT_CMD_SYS_TIMER_UPD               = 64,                 /** \brief execute system timer sequence */
    INT_CMD_ACK_WATCHDOG1               = 65,                 /** \brief acknowledge watchdog 1 */
    INT_CMD_ACK_WATCHDOG2               = 66,                 /** \brief acknowledge watchdog 2 */
    INT_CMD_READ_WD_STATUS              = 67,                 /** \brief read watchdogs status registers */
    INT_CMD_EXECUTE_TEST_MODE           = 68,                 /** \brief execute 1 iteration of test mode 1/2  */
    INT_CMD_EXECUTE_HACKED_TIMER        = 69,                 /** \brief   */

    _USB_CMD_MAX                        = 127                 /** \brief maximal value of command (not included) */
} USBCommandsEnum;

/** \brief Defines applicable statuses for transmitted USB data
 * Includes min and max status
 */
typedef enum
{
    _USB_STATUS_MIN                    = 127,             /** \brief minimal value of status (not included) */

    USB_STATUS_ACK                     = 128,             /** \brief response to is alive transaction */
    USB_STATUS_ERROR                   = 129,             /** \brief error at MCU or CS600, data holds error info */
    USB_STATUS_ERROR_TELEMETRY         = 130,             /** \brief error telemetry data of CS600 and MCU */
    USB_STATUS_STATUS                  = 131,             /** \brief status of MCU, data holds status info/log */
    USB_STATUS_BUSY                    = 132,             /** \brief MCU is busy processing previous command TODO: check if required */
    USB_STATUS_DATA                    = 133,             /** \brief response to read or read/write transaction (also works for transactions including such behaviour) */
    USB_STATUS_RESPONSE_ABSENT         = 134,             /** \brief response from MCU hasn't been received in expected timeframe */
    //TODO: Implement
    USB_STATUS_CMD_NO_SUPPORTED        = 135,             /** \brief response if cmd is not supported*/
    USB_STATUS_CMD_WRONG_PAYLOAD_LEN   = 136,             /** \brief response if cmd has wrong payload len*/
    // TODO: end
    _USB_STATUS_MAX                    = 255              /** \brief maximal value of command (not included) */
} USBStatusesEnum;

/*************************************************************************************************************************/
/*-------------------------------------------------Data Structures-------------------------------------------------------*/
/*************************************************************************************************************************/

/** \brief Structure for received USB data
 */
typedef struct
{
    uint8           msg_id;                                 /** \brief message ID */
    uint8           device_id;                              /** \brief Device ID (ASICs + sensors) */
    USBCommandsEnum command;                                /** \brief command from PC */
    uint8           dataLength;                             /** \brief data length in bytes */
    uint8           data[MAX_USB_RECIEVE_PAYLOAD_LENGTH];   /** \brief array with payload */
} USBReceiveData;

/** \brief Structure for transmit USB data
 */
typedef struct
{
    uint8           msg_id;                                 /** \brief message ID */
    uint8           device_id;                              /** \brief Device ID (ASICs + sensors) */
    USBStatusesEnum status;                                 /** \brief MCU status */
    uint8           dataLength;                             /** \brief data length in bytes */
    uint8           data[MAX_USB_TRANSMIT_PAYLOAD_LENGTH];  /** \brief array with payload */
} USBTransmitData;

/*************************************************************************************************************************/
/*------------------------------------------------Function Prototypes----------------------------------------------------*/
/*************************************************************************************************************************/

/*************************************************************************************************************************/
/*-------------------------------------------------Global variables------------------------------------------------------*/
/*************************************************************************************************************************/

#endif /* USB_DATA_TYPES_H_ */
