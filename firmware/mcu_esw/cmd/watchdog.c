/**********************************************************************************************************************
 * \file watchdog.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "common/global_defines.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "watchdog.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define WD_CFG_PACKAGE_LEN      (1)             /** \brief Number of addr+data items (4 bytes) in package  */

#define WD_STATUS_REGS_COUNT    (3)             /** \brief Number of WD status registers for periodic reading */

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
    .enStatusReading = FALSE,                                                                                                  // at startup WD is not configured
    .wdStatusRegsAddresses = {SAFETY_LOGIC_SPI_READ_WDSTATUS1, SAFETY_LOGIC_SPI_READ_WDSTATUS2, SAFETY_LOGIC_SPI_READ_ENX},    // addresses for WD status registers are constant
};

// Variable to store WD periodicity [n*20us] TODO: check usage, may not be required
static uint16 g_wdPeriodicity20UsTicks;

// Variable to store General Timer's interrupt duration calculated in microseconds TODO: check usage, may not be required
static uint32 durationOfTimerInterruptUs;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdConfigureWatchdog(USBReceiveData const * const commandPackage)
{
    // Temporary variables
    boolean isSuccessfulFlag = FALSE;
    uint8 indexerForPayload = 0;
 
    WatchdogTypeEnum selectedWD = NOT_SET;      // Selected WD
    uint16 address[WD_CFG_PACKAGE_LEN] = {0};   // Addresses of registers to write
    uint16 data[WD_CFG_PACKAGE_LEN] = {0};      // Values to write
    uint16 length = WD_CFG_PACKAGE_LEN;         // Number of 32bit SPI words to write into ASIC

    // Parse input
    // Layout: selectedWD - (addr_MSB - addr_LSB - data_MSB - data_LSB) - (...)
    selectedWD = commandPackage->data[0];
    for (uint8 i = 0; i < length; i++)
    {
        indexerForPayload = 1 + (i << 2); // One item is 4 bytes

        address[i] = ConstructWordFromBytes(commandPackage->data[indexerForPayload], commandPackage->data[indexerForPayload+1]); // TODO: incorrect, start from 1, not 0
        data[i] = ConstructWordFromBytes(commandPackage->data[indexerForPayload+2], commandPackage->data[indexerForPayload+3]);
    }

    // Retrieve values for WD timer configuration
    if (selectedWD == WD1)
    {
        safety_logic_spi_config_wd1_ut tmpConfigRegister;
        tmpConfigRegister.as_uint16 = data[0];
        g_wd1Parameters.wdConfig.ackPeriod = 10; // TODO: implement actual calculation based on spi_set_responsetime_wd1 and spi_set_locktime_wd1
        g_wd1Parameters.wdConfig.wdType = selectedWD;
        g_wd1Parameters.isWDConfigured = TRUE;
        g_wd1Parameters.state = WD_STATE_CONFIGURED;
    }
    else
    {
        safety_logic_spi_config_wd2_ut tmpConfigRegister;
        tmpConfigRegister.as_uint16 = data[0];
        g_wd2Parameters.wdConfig.ackPeriod = 10; // TODO: implement actual calculation based on spi_set_responsetime_wd2 and spi_set_locktime_wd2
        g_wd2Parameters.wdConfig.wdType = selectedWD;
        g_wd2Parameters.isWDConfigured = TRUE;
        g_wd2Parameters.state = WD_STATE_CONFIGURED;
    }
     

    #ifdef AB12_PLATFORM

    // Code for AB12 implementation
    
    if (selectedWD == WD1)
    {
        g_wd1Parameters.wdConfig.ackPeriod = 10; // TODO: hardcode value for AB12 implementation
    }
    else
    {
        g_wd1Parameters.wdConfig.ackPeriod = 10; // TODO: hardcode value for AB12 implementation
    }

    isSuccessfulFlag = TRUE; // no configuration exists fro WD in AB12

    #else

    // Code for AB15 implementation

    // Write to ASIC
    isSuccessfulFlag = QSPIWriteSequence(&(address), &(data), &length); // TODO: configuration, not yet implemented; not available for AB12

    #endif

    // Prepare report for GUI
    USBTransmitData packageToSend;
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
    uint8 requValue = 0;
    uint16 responseWord = 0;
    uint16 addressQuestion = SAFETY_LOGIC_SPI_READ_WDQA;
    uint16 addressAnswer = SAFETY_LOGIC_SPI_TRIG_WDQA1;
    SPIReceiveData data;
    uint16 length = 1;

    // Read question from ASIC
    QSPIReadSequence(&addressQuestion, &(data.dw), &length);
    requValue = (data.bf.output_data & SAFETY_LOGIC_SPI_READ_WDQA_QA1_CNT_SPI_MASK) >> SAFETY_LOGIC_SPI_READ_WDQA_QA1_CNT_SPI_SHIFT;

    // Obtain response word from look-up table
    #ifdef AB12_PLATFORM
    responseWord = GetResponseWordAB12(requValue, 0);   // TODO: use corresponding table
    #else
    responseWord = GetResponseWordAB15(requValue, 0);
    #endif

    // Send response word to ASIC
    length = 1;
    QSPIWriteSequence(&addressAnswer, &responseWord, &length);
}

void IntCmdAcknowledgeWatchdog2(void)
{
    uint8 requValue = 0;
    uint16 responseWord = 0;
    uint16 addressQuestion = SAFETY_LOGIC_SPI_READ_WDQA;
    uint16 addressAnswer = SAFETY_LOGIC_SPI_TRIG_WDQA2;
    SPIReceiveData data;
    uint16 length = 1;

    // Read question from ASIC
    QSPIReadSequence(&addressQuestion, &(data.dw), &length);
    requValue = (data.bf.output_data & SAFETY_LOGIC_SPI_READ_WDQA_QA2_CNT_SPI_MASK) >> SAFETY_LOGIC_SPI_READ_WDQA_QA2_CNT_SPI_SHIFT;

    // Obtain response word from look-up table
    #ifdef AB12_PLATFORM
    responseWord = GetResponseWordAB12(requValue, 0);   // TODO: use corresponding table
    #else
    responseWord = GetResponseWordAB15(requValue, 0);
    #endif

    // Send response word to ASIC
    length = 1;
    QSPIWriteSequence(&addressAnswer, &responseWord, &length);
}

void CmdStartWatchdog(USBReceiveData const * const commandPackage)
{
    // TODO implement
    // Plan:
    // 1. extract WD type flag
    // 2. check based on flag value if WD is configured
    // 3. start WD
    // 4. report to UI

    WatchdogTypeEnum selectedWD = NOT_SET;      // Selected WD

    selectedWD = commandPackage->data[0]; // TODO: how to handle WD1+WD2

    WatchdogParametersStruct *currentWDConfig = (selectedWD == WD1) ? (&g_wd1Parameters) : (&g_wd2Parameters);

    // If watchdog is not configured do not start
    if (currentWDConfig->isWDConfigured == FALSE) return;





    //     // Configure periodicity of Watchdog serving MCU interrupt
    //     ConfigureWatchdogPeriodicity(CalculateWatchdogAckPeriodicity());
    //     // Turn on Watchdog serving interrupt of MCU
    //     EnableWatchdogInterrupt();
    //     // JS: for testing timers:
    //     // Responses to Watchdog requests will be now generated by MCU starting with next Watchdog acknowledgement interrupt
    //     WatchdogManagementFlags.isEnabledAckWatchdog = 1;
    //     packageToSend.status = USB_STATUS_ACK;
    // }
    // else
    // {
    //     // Watchdog serving can't be enabled if no successful WD configuration was done beforehand
    //     packageToSend.status = USB_STATUS_ERROR;
    // }

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
    // Get watchdog type
    WatchdogTypeEnum selectedWD = NOT_SET;      // Selected WD
    selectedWD = commandPackage->data[0]; 

    // Turn off Watchdog serving interrupt of MCU
    if (selectedWD == WD1)
    {
        DisableWatchdog1Interrupt();
        g_wd1Parameters.isWDConfigured = FALSE;
        g_wd1Parameters.state = WD_STATE_IDLE;
    }
    else if (selectedWD == WD2)
    {
        DisableWatchdog2Interrupt();
        g_wd2Parameters.isWDConfigured = FALSE;
        g_wd2Parameters.state = WD_STATE_IDLE;
    } 

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;
    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
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
//     // TODO: find a vay to use defines for Gpt1BlockPrescaler and TimerInputPrescaler
//     //(enum values of those constants aren't suited for straightforward translation, find a solution)
//     // maybe  something like    uint16 GetGpt1BlockPrescaler(void) {  prescaler = &MODULE_GPT120.T3CON.B.BPS1;}

//     durationOfTimerInterruptUs = (GENERAL_TIMER_PERIODICITY * 8 * 1) / (100); // Configured duration of General Timer interrupt, microseconds; 1/100 = (s->us constant)/(FREQ_GPT12_HZ)

//     // Response time in factor of 20us ( value 5 = 100us) max val of 65535 will result of timer of ~1.3s
//     volatile uint16 watchdogAckPeriodicity = g_wdPeriodicity20UsTicks;
//     return watchdogAckPeriodicity;
// }
