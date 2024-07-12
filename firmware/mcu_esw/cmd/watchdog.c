/**********************************************************************************************************************
 * \file watchdog.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "watchdog.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define WD_STATUS_REGS_COUNT    (3)             /** \brief Number of WD status registers for periodic reading */

#define SPI_READ_WDSTATUS1      (0x03E)         /** \brief Watchdog status register 1 */
#define SPI_READ_WDSTATUS2      (0x03F)         /** \brief Watchdog status register 2 */
#define SPI_READ_ENX            (0x040)         /** \brief 	Status of the enx_read_in feedback signals */

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

/** \brief Defines applicable options for WD failing (intentional)
 */
typedef enum
{
    WD_FAIL_NONE                = 0,        /** \brief default, no failing */
    WD_FAIL_QA_INCORRECT        = 1,        /** \brief break QA sequesnce: provide incorrect answer */
    WD_FAIL_ACK_TIMING_LOW      = 2,        /** \brief break answer timing: provide answer too soon */
    WD_FAIL_ACK_TIMING_HIGH     = 3         /** \brief break answer timing: provide answer too late */
} WatchdogFailEnum;

/** \brief Defines available WD
 */
typedef enum
{
    NOT_SET                     = 0,        /** \brief WD type was not yet defined */
    WD1                         = 1,        /** \brief Watchdog 1 */
    WD2                         = 2,        /** \brief Watchdog 2 */
    WD3                         = 3         /** \brief Watchdog 3 */
} WatchdogTypeEnum;

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Structure for status reading configuration
 */
typedef struct
{
    boolean enStatusReading;            /** \brief enable periodic WD status reading flag */
    uint16 wdStatusRegsAddresses[3];    /** \brief array with adresses of WD status registers */
    uint16 monitoringMessageID;         /** \brief message ID for sending back WD status data */
} WatchdogStatusMonitoringStruct;

/** \brief Structure for Watchdog configuration
 */
typedef struct
{
    uint32 ackPeriod;                   /** \brief acknowledge periodicity in timer interrupts */
    WatchdogTypeEnum wdType;            /** \brief type of WD */
} WatchdogConfigStruct;

/** \brief Structure for Watchdog parameters (main structure)
 */
typedef struct
{
    boolean isWDConfigured;             /** \brief flag indicating if WD was configured */
    WatchdogConfigStruct wdConfig;      /** \brief struct with WD configuration */
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
    .enStatusReading = FALSE,                                                           // at startup WD is not configured
    .wdStatusRegsAddresses = {SPI_READ_WDSTATUS1, SPI_READ_WDSTATUS2, SPI_READ_ENX},    // addresses for WD status registers are constant TODO: replace by defines from regmap
};

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdConfigureWatchdog(USBReceiveData const * const commandPackage)
{
    // TODO: implement
    boolean isSuccessfulFlag = FALSE;

    // // Prepare package for writing into ASIC
    // uint16 address[WD_CFG_PACKAGE_LEN] = {WD_CONFIG0_ADDRESS, WD_CONFIG1_ADDRESS, WD_TIME_WIN_ADDRESS, WD_RESPTIME_ADDRESS}; // Addresses of registers to write
    // uint16 data[WD_CFG_PACKAGE_LEN] = {0, 0, 0, 0};  // Values to write
    // uint16 length = WD_CFG_PACKAGE_LEN; // Number of 32bit SPI words to write into ASIC

    // // Unpack received message from GUI to obtain register values
    // data[0] = ConstructWordFromBytes(0, commandPackage->data[0]); // WD_CONFIG0 register value (WDG_ENABLE field)
    // data[1] = ConstructWordFromBytes(0, commandPackage->data[1]); // WD_CONFIG1 register value (WDG_EC_TH1, WDG_EN_LOCK, T_WIN_INITIAL fields)
    // data[2] = ConstructWordFromBytes(commandPackage->data[3], commandPackage->data[2]); // WD_TIME_WIN register value (T_WIN field)
    // data[3] = ConstructWordFromBytes(commandPackage->data[5], commandPackage->data[4]); // WD_RESPTIME register value (RESPTIME field)

    // // Store RESPTIME value for configuration of Watchdog serving periodicity
    // respTimeValue = (uint16)data[3];

    // // Write to ASIC

    // //TODO: JS: 17.6. bruteforce set to true to allow enable watchdog for test purpose +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    // isSuccessfulFlag = TRUE;
    // //isSuccessfulFlag = QSPIWriteSequence(&(address), &(data), &length); // TODO: configuration, not yet implemented; not available for AB12

    // // Prepare report for GUI
    // USBTransmitData packageToSend;
    // packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // if (isSuccessfulFlag == FALSE)
    // {
    //     // Common error frame setup
    //     packageToSend.status = USB_STATUS_ERROR;
    //     packageToSend.dataLength = 0;
    // }
    // else
    // {
    //     WatchdogManagementFlags.isConfigured = 1;
    //     packageToSend.status = USB_STATUS_ACK;
    //     packageToSend.dataLength = 0;
    // }

    // // Send report to GUI
    // SendUSBPackage(&packageToSend);
}

void CmdStartMonitoringWatchdog(USBReceiveData const * const commandPackage)
{
    // Save message ID
    g_wdStatusMonitoringConfig.monitoringMessageID = commandPackage->msg_id;

    // Enable monitoring procedure
    g_wdStatusMonitoringConfig.enStatusReading = TRUE;
}

void CmdStopMonitoringWatchdog(USBReceiveData const * const commandPackage)
{
    // Disable monitoring procedure
    g_wdStatusMonitoringConfig.enStatusReading = FALSE;

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void IntCmdAcknowledgeWatchdog1(void)
{
    // TODO: implement
    // uint8 requValue = 0;
    // uint16 responseWord0, responseWord1 = 0;
    // uint16 address, length = 0;

    // // Get request from ASIC's Watchdog
    // // No checks to not serve same Watchdog request more than once (expected behavior in case of failed previous serving)
    // requValue = GetREQUValue();

    // // Obtain response word from look-up table
    // responseWord1 = GetResponseWordAB12(requValue, 1);
    // responseWord0 = GetResponseWordAB12(requValue, 0);

    // // Send response words to ASIC
    // address = WD_RESP_ADDRESS;
    // length = 1;
    // QSPIWriteSequence(&address, &responseWord1, &length);
    // QSPIWriteSequence(&address, &responseWord0, &length);
}

void IntCmdAcknowledgeWatchdog2(void)
{
    // TODO: implement
    // uint8 requValue = 0;
    // uint16 responseWord0, responseWord1 = 0;
    // uint16 address, length = 0;

    // // Get request from ASIC's Watchdog
    // // No checks to not serve same Watchdog request more than once (expected behavior in case of failed previous serving)
    // requValue = GetREQUValue();

    // // Obtain response word from look-up table
    // responseWord1 = GetResponseWordAB12(requValue, 1);
    // responseWord0 = GetResponseWordAB12(requValue, 0);

    // // Send response words to ASIC
    // address = WD_RESP_ADDRESS;
    // length = 1;
    // QSPIWriteSequence(&address, &responseWord1, &length);
    // QSPIWriteSequence(&address, &responseWord0, &length);
}

void CmdStartWatchdog(USBReceiveData const * const commandPackage)
{
    // TODO implement
    // Plan:
    // 1. extract WD type flag
    // 2. check based on flag value if WD is configured
    // 3. start WD
    // 4. report to UI


    // USBTransmitData packageToSend;
    // packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    // packageToSend.dataLength = 0;
    // uint16 address, length, data;

    // if (WatchdogManagementFlags.isConfigured == TRUE)
    // {
    //     // Sync event
    //     // Write Response Word 0 twice to cause start of new monitoring cycle
    //     address = WD_RESP_ADDRESS;
    //     length = 1;
    //     data = GetResponseWordAB12(0, 0);
    //     //QSPIWriteSequence(&address, &data, &length);  // TODO: handling ASIC communication for watchdog, not implemented
    //     //QSPIWriteSequence(&address, &data, &length);

    //     // Configure periodicity of Watchdog serving MCU interrupt
    //     ConfigureWatchdogPeriodicity(CalculateWatchdogAckPeriodicity());
    //     // Turn on Watchdog serving interrupt of MCU
    //     EnableWatchdogInterrupt();

    //     // Responses to Watchdog requests will be now generated by MCU starting with next Watchdog acknowledgement interrupt
    //     WatchdogManagementFlags.isEnabledAckWatchdog = 1;
    //     packageToSend.status = USB_STATUS_ACK;
    // }
    // else
    // {
    //     // Watchdog serving can't be enabled if no successful WD configuration was done beforehand
    //     packageToSend.status = USB_STATUS_ERROR;
    // }

    // // Send message to GUI
    // SendUSBPackage(&packageToSend);
}

void CmdStopWatchdog(USBReceiveData const * const commandPackage)
{
    // TODO: implement
    // // Turn off Watchdog serving interrupt of MCU
    // DisableWatchdogInterrupt();

    // // No responses to Watchdog requests will be now generated by MCU
    // WatchdogManagementFlags.isEnabledAckWatchdog = 0;

    // // Prepare acknowledge message
    // USBTransmitData packageToSend;
    // packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    // packageToSend.status = USB_STATUS_ACK;
    // packageToSend.dataLength = 0;
    // // Send acknowledge message to GUI
    // SendUSBPackage(&packageToSend);
}

uint16 GetResponseWordAB12(uint8 requValue, boolean respWrdNumber)
{
    // TODO: same tables for AB15 for WD1/2
    // Table of RESP_WRD values (according to datasheet AB12)
    uint16 responseWordArray[8][2] = {{0x2020, 0xE106},
                                       {0xFDFD, 0x9671},
                                       {0x8A8A, 0x4BAC},
                                       {0x5757, 0x3CDB},
                                       {0xECEC, 0xD235},
                                       {0x3131, 0xA542},
                                       {0x4646, 0x789F},
                                       {0x9B9B, 0x0FE8}};
    // Provide value of requested RESP_WRD
    return (responseWordArray[requValue][respWrdNumber]);
}


// // TODO: check approach for implementation
// uint8 GetRespCnt(void)
// {
//     // Read ASIC's WD_REQU register
//     uint16 address = WD_REQU_ADDRESS;
//     SPIReceiveData data;
//     uint16 length = 1;
//     // Read from ASIC
//     QSPIReadSequence(&address, &(data.dw), &length);
//     // Obtain RESP_CNT field value from register's content
//     uint8 respCntValue = (data.bf.output_data & RESP_CNT_READMASK) >> RESP_CNT_OFFSET;
//     return respCntValue;
// }

// uint8 GetREQUValue(void)
// {
//     // Read ASIC's WD_REQU register
//     uint16 address = WD_REQU_ADDRESS;
//     SPIReceiveData data;
//     uint16 length = 1;
//     // Read from ASIC
//     QSPIReadSequence(&address, &(data.dw), &length);
//     // Obtain REQU field value from register's content
//     uint8 requValue = (data.bf.output_data & REQU_READMASK) >> REQU_OFFSET;
//     return requValue;
// }

// uint16 CalculateWatchdogAckPeriodicity(void)
// {
//     //TODO: can be optimized using IfxGpt12_T3_getFrequency (base freq of GPT12 + prescaler). Check

//     // Should be equal to or longer than RESP_TIME and shorter than RESP_TIME + TIME_WINDOW durations
//     // T_timer_itr(s) = (GENERAL_TIMER_PERIODICITY * Gpt1BlockPrescaler ∗ TimerInputPrescaler) / (FREQ_GPT12_HZ)
//     // TODO: find a vay to use defines for Gpt1BlockPrescaler and TimerInputPrescaler (enum values of those constants aren't suited for straightforward translation, find a solution)
//     durationOfTimerInterruptUs = (GENERAL_TIMER_PERIODICITY * 16 * 2) / (100); // Configured duration of General Timer interrupt, microseconds; 1/100 = (s->us constant)/(FREQ_GPT12_HZ)

//     // Configured duration of ASIC's Watchdog Response Time, microseconds (datasheet translation: Response Time = RESPTIME_CFG * 0.1, ms)
//     volatile uint32 respTimeValueUs = respTimeValue*100;
//     volatile uint16 watchdogAckPeriodicity = respTimeValueUs/durationOfTimerInterruptUs;
//     return watchdogAckPeriodicity;
// }
