using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Models;

/// <summary>
/// Constants for receive/transmit package formats
/// Note: should match with package format on MCU
/// </summary>
public static class SerialPackageConstants
{
    // Fields position in package (both send and receive)
    // NOTE: CRC, END byte positions are defined from end of the package
    public const int StartBytePosition      = 0;
    public const int MsgIDPosition          = 1;
    public const int ASICIDPosition         = 2;
    public const int CmdStatusPosition      = 3;
    public const int PayloadLengthPosition  = 4;
    public const int PayloadPosition        = 5;
    public const int CRCPosition            = -2;   /** \brief prelast byte */
    public const int EndBytePosition        = -1;   /** \brief last byte */

    // Fields length in bytes in package (both send and receive)
    // NOTE: for variable length field (payload) max length is provided
    public const int StartByteLength        = 1;
    public const int MsgIDLength            = 1;
    public const int ASICIDLength           = 1;
    public const int CmdStatusLength        = 1;
    public const int PayloadLengthLength    = 1;
    public const int PayloadLength          = 255;  /** \brief maximum length */
    public const int CRCLength              = 1;
    public const int EndByteLength          = 1;

    // Minimum length of full package in bytes
    public const int MinPackageLength       = 7;

    // Constant values
    public const byte StartByteValue        = 0xAB;
    public const byte EndByteValue          = 0xBA;
}

/// <summary>
/// Constants for certain commands
/// Note: should match with package format on MCU
/// </summary>
public static class CommandSpecificConstants
{
    public const int readSequenceMaxItems = 32;
    public const int writeSequenceMaxItems = 16;
    public const int rwSequenceMaxItems = 10;
}

/// <summary>
/// Command values for MCU. Occupy cmd field of package
/// Note: should match commands declared in MCU; formatting inherited from MCU code
/// </summary>
public enum MCUCommand
{
    _CMD_MIN                    = 0,                  /** \brief minimal value of command (not included) */

    IS_ALIVE                    = 1,                  /** \brief transaction to check if USB communication works */
    SPI_INSTRUCTION             = 2,                  /** \brief execute SPI instruction with 16bit input data (send via payload) */
    READ_DEV_ID                 = 3,                  /** \brief reads ASIC Device ID, reports error if unsuccessful */
    READ_REG                    = 4,                  /** \brief read single register, otput data in raw format, reports error if unsuccessful */
    WRITE_REG                   = 5,                  /** \brief write single register, otput data in raw format, reports error if unsuccessful */
    CONFIGURE_READING           = 6,                  /** \brief reconfigure continuos reading (table, periodiciyr, etc.) */
    START_READING_SEQ           = 7,                  /** \brief start reading defined set of registers */
    STOP_READING_SEQ            = 8,                  /** \brief stop reading sequence */
    STOP_WATCHDOG               = 9,                  /** \brief stop to perform periodic Watchdog Request acknowledgement by MCU */
    EXECUTE_RW_SEQUENCE         = 10,                 /** \brief perform single read/write series of operation */
    EXECUTE_READ_SEQUENCE       = 11,                 /** \brief perform single read series operation */
    EXECUTE_WRITE_SEQUENCE      = 12,                 /** \brief perform single write series operation */
    CONFIGURE_WATCHDOG          = 13,                 /** \brief configure watchdog (Note: initiates read/write seq, configs timer, arms watchdog response) */
    START_WATCHDOG              = 14,                 /** \brief start to perform periodic Watchdog Request acknowledgement by MCU  */
    RESET_TO_IDLE               = 15,                 /** \brief stop all continuous operations, set all pins to default state, wait for new command */
    START_MONITORING_WATCHDOG   = 16,                 /** \brief enables MCU functionality that sends values of Watchdog status registers and time durations between the polls to GUI; continuous command */
    STOP_MONITORING_WATCHDOG    = 17,                 /** \brief disables MCU functionality that sends values of Watchdog status registers and time durations between the polls to GUI */
    GET_MCU_VERSION             = 18,                 /** \brief returns MCU SW version */
    GET_MCU_BUILD_DATE          = 19,                 /** \brief returns MCU SW build date */
    GET_MCU_BUILD_TIME          = 20,                 /** \brief returns MCU SW build time */
    SET_EXT_OSC_2MHZ            = 21,                 /** \brief sets external clock output to 2Mhz */
    SET_EXT_OSC_4MHZ            = 22,                 /** \brief sets external clock output to 4Mhz  */
    FLM_DIAG_ENABLE             = 23,                 /** \brief enables Firing Loop Module cyclic diagnostics TODO: should be verified after feature merging */
    FLM_DIAG_READ_RESULTS       = 24,                 /** \brief returns results of Firing Loop Module cyclic diagnostics TODO: should be verified after feature merging */
    INIT_SYNC                   = 25,                 /** \brief syncs ASIC and GUI: SPI_COLDSTART_1, response to GUI when ASIC in INIT state TODO: should be verified after feature merging */
    WRITE_RAW_DATA_SPI          = 26,                 /** \brief 0x1A write 32bit raw data to spi TODO: should be verified after feature merging */
    FLM_DIAG_DISABLE            = 27,                 /** \brief enables Firing Loop Module cyclic diagnostics TODO: should be verified after feature merging */
    START_HACKED_TIMER          = 32,                 /** \brief  */
    STOP_HACKED_TIMER           = 33,                 /** \brief   */
    
    START_TEST_MODE1            = 29,                 /** \brief configure and start Test mode 1  */
    START_TEST_MODE2            = 30,                 /** \brief configure and start Test mode 1  */
    STOP_TEST_MODE12            = 31,                 /** \brief stop Test mode (currently running)  */

    _EXT_CMD_MAX                = 63                  /** \brief maximal value of command send via USB, start of internal commands (generated by MCU) */
}

/// <summary>
/// Status values of MCU. Occupy status field of package
/// Note: should match statuses declared in MCU; formatting inherited from MCU code
/// </summary>
public enum MCUStatus
{
    _STATUS_MIN             = 127,             /** \brief minimal value of status (not included) */

    ACK                     = 128,             /** \brief response to is alive transaction */
    ERROR                   = 129,             /** \brief error at MCU or CS600, data holds error info */
    ERROR_TELEMETRY         = 130,             /** \brief error telemetry data of CS600 and MCU */
    STATUS                  = 131,             /** \brief status of MCU, data holds status info/log */
    BUSY                    = 132,             /** \brief MCU is busy processing previous command TODO: check if required */
    DATA                    = 133,             /** \brief response to read or read/write transaction (also works for transactions including such behaviour) */

    RESPONSE_ABSENT         = 134,             /** \brief response from MCU hasn't been received in expected timeframe */

    _STATUS_MAX             = 255              /** \brief maximal value of command (not included) */
}

/// <summary>
/// Read masks to work with message ID values
/// </summary>
public enum MsgIDMasks
{
    ResponseBit             = 0x80,               /** bit indicating message direction, 0: PC -> MCU, 1: MCU -> PC */
    MCUOnlyBits             = 0x60,               /** bits reserved for messages initiated by MCU (error, status) */
    GeneralPurposeBits      = 0x1F,               /** bits for typical MCU messages in response for PC messages */
}