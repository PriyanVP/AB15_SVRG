/**********************************************************************************************************************
 * \file watchdog.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "common/global_defines.h"
#include "common/spi_data_types.h"
#include "common/watchdog_types.h"
#include "common/bit_manipulation.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "periphery/timer.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "watchdog.h"
#include "pwm.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define WD_CFG_PACKAGE_MAX_LEN      (6)         /** \brief Max number of addr+data items (4 bytes) in package  */

#define WD_STATUS_REGS_COUNT        (4)         /** \brief Number of WD status registers for periodic reading */

#define AB12_WD2_ACK_PERIOD         (12)        /** \brief Periodicity of acknowledging WD2 on AB12 platform: 600 us/ 50 us
                                                    where 50 us - timer interrupt periodicity on MCU */

#define AB12_WD3_ACK_PERIOD         (200)       /** \brief Periodicity of acknowledging WD3 on AB12 platform: 10 ms/ 50 us
                                                    where 50 us - timer interrupt periodicity on MCU */

#define WD1_ACK_PERIOD_SCALE_FACTOR (20)        /** \brief Scale factor to convert WD1 ACK period 
                                                    from register units to timer ticks: 1 ms / 50 us = 20 */

#define WD2_ACK_PERIOD_SCALE_FACTOR (1)         /** \brief Scale factor to convert WD2 ACK period 
                                                    from register units to timer ticks: 50 us / 50 us = 1 */

#define WD_STATUS_CHECK_PERIOD      (4000)      /** \brief Periodicity of reading status of WD: 200 ms/ 50 us */

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Defines applicable states for WD state machine
 */
typedef enum
{
    WD_STATE_IDLE               = 0,        /** \brief default state after POR */
    WD_STATE_CONFIGURED         = 1,        /** \brief configuration loaded to ASIC, parametrs for start WD are available */
    WD_STATE_RUNNING_NORMAL     = 2,        /** \brief watchdog is being acknowledged correctly */
    WD_STATE_RUNNING_FAILING    = 3         /** \brief watchdog is being acknowledged incorrectly (intentionally) */
} WatchdogStateEnum;

/** \brief Defines applicable states for Ext clock state
 */
typedef enum
{
    EXT_CLK_STATE_IDLE          = 0,        /** \brief init state after POR */
    EXT_CLK_STATE_2MHZ          = 1,        /** \brief default state eclock is running with 2 Mhz   */
    EXT_CLK_STATE_4MHZ          = 2,        /** \brief eclock is running with 2 Mhz */
    EXT_CLK_STATE_FAIL          = 3         /** \brief not used yet */
} ExtClockStateEnum;

/** \brief Defines applicable options for WD failing (intentional)
 */
typedef enum
{
    WD_FAIL_NONE                = 0,        /** \brief default, no failing */
    WD_FAIL_QA_INCORRECT        = 1,        /** \brief break QA sequesnce: provide incorrect answer */
    WD_FAIL_ACK_TIMING_LOW      = 2,        /** \brief break answer timing: provide answer too soon */
    WD_FAIL_ACK_TIMING_HIGH     = 3         /** \brief break answer timing: provide answer too late */
} WatchdogFailEnum;

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Structure for status reading configuration
 */
typedef struct
{
    boolean enStatusReading;                               /** \brief enable periodic WD status reading flag */
    uint16 wdStatusRegsAddresses[WD_STATUS_REGS_COUNT];    /** \brief array with adresses of WD status registers */
    uint16 lengthOfRegsToRead;                             /** \brief number of registers to read */
    uint8  monitoringMessageID;                            /** \brief message ID for sending back WD status data */
} WatchdogStatusMonitoringStruct;

/** \brief Structure for Watchdog configuration
 */
typedef struct
{
    uint16 ackPeriod;                   /** \brief acknowledge periodicity in timer interrupts */
    WatchdogTypeEnum wdType;            /** \brief type of WD */
} WatchdogConfigStruct;

/** \brief Structure for Watchdog parameters (main structure)
 */
typedef struct
{
    boolean isWDConfigured;             /** \brief flag indicating if WD was configured */
    WatchdogConfigStruct wdConfig;      /** \brief struct with WD configuration */
    uint16 wdQuestion;                  /** \brief current question to WD acknowledgement */
    WatchdogStateEnum state;            /** \brief state of WD (in state machine) */
    WatchdogFailEnum failMode;          /** \brief option for intentional breaking WD ack procedure */
} WatchdogParametersStruct;

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Local static variable to store all parameters for WD1
 */
static WatchdogParametersStruct g_wd1Parameters =
{
    .isWDConfigured = FALSE,            // at startup WD is not configured
    .wdConfig = 
    {
        .wdType = NOT_SET,
        .ackPeriod = 0
    },                                  // default configuration
    .state = WD_STATE_IDLE,             // default state is IDLE
    .failMode = WD_FAIL_NONE            // no failing by default
};

/** \brief Local static variable to store all parameters for WD2
 */
static WatchdogParametersStruct g_wd2Parameters =
{
    .isWDConfigured = FALSE,            // at startup WD is not configured
    .wdConfig = 
    {
        .wdType = NOT_SET,
        .ackPeriod = 0
    },                                  // default configuration
    .state = WD_STATE_IDLE,             // default state is IDLE
    .failMode = WD_FAIL_NONE            // no failing by default
};

/** \brief Local static variable to store all parameters for WD1
 */
static WatchdogStatusMonitoringStruct g_wdStatusMonitoringConfig =
{
    .enStatusReading = FALSE,                                                                           // at startup WD is not configured
    .wdStatusRegsAddresses = {SAFETY_LOGIC_SPI_READ_WDSTATUS1, SAFETY_LOGIC_SPI_READ_WDSTATUS2, 
                              SAFETY_LOGIC_SPI_READ_ENX, SAFETY_LOGIC_SPI_READ_WDQA},                   // addresses for WD status registers are constant
    .lengthOfRegsToRead = WD_STATUS_REGS_COUNT                                                          // number of addresses to read
};


/** \brief Local static variable to store external clock state
 */
static ExtClockStateEnum g_extClState = EXT_CLK_STATE_2MHZ;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Function to provide an Answer word for Question of AB15's Watchdog 1 
 * \param questionValue value of AB15's Watchdog 1 Question word
 * \return Returns Answer word
 */
uint16 GetAnswerWordWD1AB15(uint8 questionValue);

/** \brief Function to provide an Answer word for Question of AB15's Watchdog 2 
 * \param questionValue value of AB15's Watchdog 2 Question word
 * \return Returns Answer word
 */
uint16 GetAnswerWordWD2AB15(uint8 questionValue);

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdConfigureWatchdog(USBReceiveData const * const commandPackage)
{
    // Command should not be executed in certain WD feature states
    if ((g_wd1Parameters.state == WD_STATE_RUNNING_NORMAL) || 
        (g_wd2Parameters.state == WD_STATE_RUNNING_NORMAL) || 
        (g_wd1Parameters.state == WD_STATE_RUNNING_FAILING)||
        (g_wd2Parameters.state == WD_STATE_RUNNING_FAILING))
    {
        // Skip function execution - GUI will see no response
        return;
    }

    // Temporary variables
    USBTransmitData packageToSend;
    boolean isSuccessfulFlag = TRUE;                    //Default true is used to handle write of series of data (AND logic on flags)
    uint8 indexerForPayload = 0;
 
    uint16 address[WD_CFG_PACKAGE_MAX_LEN] = {0};       // Addresses of registers to write
    uint16 data[WD_CFG_PACKAGE_MAX_LEN] = {0};          // Values to write
    uint16 length = (commandPackage->dataLength) >> 2;  // Number of 32bit SPI words to write into ASIC (= number of registers to write)

    // Parse input
    // Layout: (addr_LSB - addr_MSB - data_LSB - data_MSB) - (...)
    for (uint8 i = 0; i < length; i++)
    {
        indexerForPayload = (i << 2); // One item is 4 bytes

        address[i] = ConstructWordFromBytes(commandPackage->data[indexerForPayload+1], commandPackage->data[indexerForPayload]);
        data[i] = ConstructWordFromBytes(commandPackage->data[indexerForPayload+3], commandPackage->data[indexerForPayload+2]);
    }

    // Retrieve values for WD timer configuration
    safety_logic_spi_config_wd1_ut tmpConfigRegisterWD1;
    tmpConfigRegisterWD1.as_uint16 = data[0];
    g_wd1Parameters.wdConfig.ackPeriod = WD1_ACK_PERIOD_SCALE_FACTOR * (tmpConfigRegisterWD1.as_s.SpiSetLocktimeWd1_u6 + (tmpConfigRegisterWD1.as_s.SpiSetResponsetimeWd1_u6 >> 1)); // ACK WD at the middle of response time interval
    g_wd1Parameters.wdConfig.wdType = WD1;
    g_wd1Parameters.wdQuestion = 0;
    g_wd1Parameters.isWDConfigured = TRUE;
    g_wd1Parameters.state = WD_STATE_CONFIGURED;

    safety_logic_spi_config_wd2_ut tmpConfigRegisterWD2;
    tmpConfigRegisterWD2.as_uint16 = data[1];
    g_wd2Parameters.wdConfig.ackPeriod = WD2_ACK_PERIOD_SCALE_FACTOR * (tmpConfigRegisterWD2.as_s.SpiSetLocktimeWd2_u6+ (tmpConfigRegisterWD2.as_s.SpiSetResponsetimeWd2_u6 >> 1)); // ACK WD at the middle of response time interval
    g_wd2Parameters.wdConfig.wdType = WD2;
    g_wd2Parameters.wdQuestion = 0;
    g_wd2Parameters.isWDConfigured = TRUE;
    g_wd2Parameters.state = WD_STATE_CONFIGURED;

    // Code for AB15 implementation
    isSuccessfulFlag = TRUE;
    // Write both WD configs to ASIC to ASIC
    for (uint8 i = 0; i < length; i++)
    {
        isSuccessfulFlag &= QSPIWriteNormal(SPI1_CS1MASTER, address[i], data[i]); // TODO: configuration, not yet implemented; not available for AB12
    }

    // Prepare report for GUI
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;
    }
    else
    {
        packageToSend.status = USB_STATUS_ACK;
        packageToSend.dataLength = 0;
    }

    // Send report to GUI
    SendUSBPackage(&packageToSend);
}

void IntCmdAcknowledgeWatchdog1(void)
{
    uint8 question;
    uint16 answer = 0;
    SPIReceiveDataNormal data;

    // Obtain response word from look-up table
    // AB15 platform
    // Read question from ASIC
    QSPIReadNormal(SPI1_CS1MASTER, SAFETY_LOGIC_SPI_READ_WDQA, &(data.dw));
    question = (data.bf.output_data & SAFETY_LOGIC_SPI_READ_WDQA_QA1_CNT_SPI_MASK) >> SAFETY_LOGIC_SPI_READ_WDQA_QA1_CNT_SPI_SHIFT;

    // Get answer from the table
    answer = GetAnswerWordWD1AB15(question);
    
    // Send answer word to ASIC
    QSPIWriteNormal(SPI1_CS1MASTER, SAFETY_LOGIC_SPI_TRIG_WDQA1, answer);
}

void IntCmdAcknowledgeWatchdog2(void)
{
    uint8 question;
    uint16 answer = 0;
    SPIReceiveDataNormal data;

    // Obtain response word from look-up table
    // AB15 platform
    // Read question from ASIC
    QSPIReadNormal(SPI1_CS1MASTER, SAFETY_LOGIC_SPI_READ_WDQA, &(data.dw));
    question = (data.bf.output_data & SAFETY_LOGIC_SPI_READ_WDQA_QA2_CNT_SPI_MASK) >> SAFETY_LOGIC_SPI_READ_WDQA_QA2_CNT_SPI_SHIFT;

    // Get answer from the table
    answer = GetAnswerWordWD2AB15(question);
    
    // Send answer word to ASIC
    QSPIWriteNormal(SPI1_CS1MASTER, SAFETY_LOGIC_SPI_TRIG_WDQA2, answer);
}

void CmdStartWatchdog(USBReceiveData const * const commandPackage)
{
    /*prepare data package*/
    USBTransmitData packageToSend;
    packageToSend.device_id = 1;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Command should not be executed in certain WD feature states
    if ((g_wd1Parameters.state != WD_STATE_CONFIGURED) || 
        (g_wd2Parameters.state != WD_STATE_CONFIGURED))
    {
        // Skip function execution - GUI will see no response
        return;
    }

    // If any of the watchdogs is not configured do not start
    if ((g_wd1Parameters.isWDConfigured == FALSE) ||
        (g_wd2Parameters.isWDConfigured == FALSE)) 
    {
        packageToSend.status = USB_STATUS_ERROR;
        // Send message to GUI
        SendUSBPackage(&packageToSend);
        return;
    }

    // Lock config for AB15
    // Note: assumed that all other bites in spi_set_wdsettings can be 0
    QSPIWriteNormal(SPI1_CS1MASTER, SAFETY_LOGIC_SPI_SET_WDSETTINGS, SAFETY_LOGIC_SPI_SET_WDSETTINGS_SPI_ON_SL_MASK);

    // Configure periodicity of Watchdog serving MCU interrupt
    ConfigureTimerPeriodicity(WATCHDOG1_TIMER, g_wd1Parameters.wdConfig.ackPeriod);
    ConfigureTimerPeriodicity(WATCHDOG2_TIMER, g_wd2Parameters.wdConfig.ackPeriod);

    // Turn on Watchdog serving interrupt of MCU
    EnableTimerInterrupt(WATCHDOG1_TIMER);
    EnableTimerInterrupt(WATCHDOG2_TIMER);

    // Modify state
    g_wd1Parameters.state = WD_STATE_RUNNING_NORMAL;
    g_wd2Parameters.state = WD_STATE_RUNNING_NORMAL;

    // Akcnowledge success of start WD
    packageToSend.status = USB_STATUS_ACK;

    // Send message to GUI
    SendUSBPackage(&packageToSend);
}

void CmdStopWatchdog(USBReceiveData const * const commandPackage)
{
    // Turn off Watchdog serving interrupt of MCU
    DisableTimerInterrupt(WATCHDOG1_TIMER);
    DisableTimerInterrupt(WATCHDOG2_TIMER);

    // Modify state and flags for WD
    g_wd1Parameters.isWDConfigured = FALSE;
    g_wd1Parameters.state = WD_STATE_IDLE;
    
    g_wd2Parameters.isWDConfigured = FALSE;
    g_wd2Parameters.state = WD_STATE_IDLE;

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void CmdSetExtOsc2Mhz(USBReceiveData const * const commandPackage)
{
    // check if already set to 2 MHZ
    if (g_extClState == EXT_CLK_STATE_2MHZ)
    {
        return;
    }
    g_extClState = EXT_CLK_STATE_2MHZ;

    SetPWMGeneration2MHZ();
    StartPWMGeneration();

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.device_id = 1;
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void CmdSetExtOsc4Mhz(USBReceiveData const * const commandPackage)
{
    // check if already set to 2 MHZ
    if (g_extClState == EXT_CLK_STATE_4MHZ)
    {
        return;
    }
    g_extClState = EXT_CLK_STATE_4MHZ;

    SetPWMGeneration4MHZ();
    StartPWMGeneration();

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.device_id = 1;
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}
void CmdStartMonitoringWatchdog(USBReceiveData const * const commandPackage)
{
    // Save message ID
    g_wdStatusMonitoringConfig.monitoringMessageID = SetResponseBit(commandPackage->msg_id);

    // Enable monitoring procedure
    g_wdStatusMonitoringConfig.enStatusReading = TRUE;

    // Arm timer routine
    ConfigureTimerPeriodicity(WATCHDOG_STATUS_CHECK_TIMER, WD_STATUS_CHECK_PERIOD);
    EnableTimerInterrupt(WATCHDOG_STATUS_CHECK_TIMER);
}

void CmdStopMonitoringWatchdog(USBReceiveData const * const commandPackage)
{
    // Disable monitoring procedure
    g_wdStatusMonitoringConfig.enStatusReading = FALSE;

    // Unarm timer routine
    DisableTimerInterrupt(WATCHDOG_STATUS_CHECK_TIMER);

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void IntCmdMonitorWatchdog(void)
{
    // Local variables
    USBTransmitData packageToSend;
    uint16 length = g_wdStatusMonitoringConfig.lengthOfRegsToRead;
    boolean isSuccessfulFlag = FALSE;

    // Check if configured
    if (g_wdStatusMonitoringConfig.enStatusReading == FALSE)
    {
        // Stop execution as no msgID for response to PC is present
        return;
    }

    // Read WD status from ASIC
    // AB15 platform
    SPIReceiveDataNormal data[WD_STATUS_REGS_COUNT] = {0}; // TODO: may not work as this type of initializer not intended for array of structs

    // Read WD related registers from ASIC
    isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, g_wdStatusMonitoringConfig.wdStatusRegsAddresses, &data[0].dw, &length); // TODO: not implemented

    packageToSend.dataLength = length << 1; // each data item is send as 2 bytes

    for (uint8 i = 0; i < length; i++)
    {
        packageToSend.data[i<<1]     = GetLSB(data[i].bf.output_data);
        packageToSend.data[(i<<1)+1] = GetMSB(data[i].bf.output_data);
    }

    // Send data
    packageToSend.status = (isSuccessfulFlag) ? USB_STATUS_DATA : USB_STATUS_ERROR;
    packageToSend.device_id = SPI1_CS1MASTER;
    packageToSend.msg_id = g_wdStatusMonitoringConfig.monitoringMessageID;

    // Send message to GUI
    SendUSBPackage(&packageToSend);
}

uint16 GetAnswerWordWD1AB15(uint8 questionValue)
{
    static const uint16 answerWordArrayWD1[32] = {0x2027,
                                                  0xE463,
                                                  0x2893,
                                                  0xECD7,
                                                  0x4307,
                                                  0x8743,
                                                  0x4BB3,
                                                  0x8FF7,
                                                  0x32EF,
                                                  0xF6AB,
                                                  0x3A5B,
                                                  0xFE1F,
                                                  0x51CF,
                                                  0x958B,
                                                  0x597B,
                                                  0x9D3F,
                                                  0x0C2D,
                                                  0xC869,
                                                  0x0499,
                                                  0xC0DD,
                                                  0x6F0D,
                                                  0xAB49,
                                                  0x67B9,
                                                  0xA3FD,
                                                  0x1EE5,
                                                  0xDAA1,
                                                  0x1651,
                                                  0xD215,
                                                  0x7DC5,
                                                  0xB981,
                                                  0x7571,
                                                  0xB135};
    return (answerWordArrayWD1[questionValue]);
}

uint16 GetAnswerWordWD2AB15(uint8 questionValue)
{
    static const uint16 answerWordArrayWD2[8] = {0x35CF,
                                                 0x9867,
                                                 0x68B3,
                                                 0xC51B,
                                                 0x5789,
                                                 0xFA21,
                                                 0x0AF5,
                                                 0xA75D};
    return (answerWordArrayWD2[questionValue]);
}
