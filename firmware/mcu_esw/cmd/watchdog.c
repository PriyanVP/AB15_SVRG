/**********************************************************************************************************************
 * \file watchdog.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "common/global_defines.h"
#include "common/watchdog_types.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "watchdog.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define WD_CFG_PACKAGE_LEN      (1)             /** \brief Number of addr+data items (4 bytes) in package  */

#define WD_STATUS_REGS_COUNT    (3)             /** \brief Number of WD status registers for periodic reading */

#define AB12_WD2_ACK_PERIOD     (12)            /** \brief Periodicity of acknowledging WD2 on AB12 platform: 600 us/ 50 us
                                                    where 50 us - timer interrupt periodicity on MCU */

#define AB12_WD3_ACK_PERIOD     (200)           /** \brief Periodicity of acknowledging WD3 on AB12 platform: 10 ms/ 50 us
                                                    where 50 us - timer interrupt periodicity on MCU */

#define WD_STATUS_CHECK_PERIOD  (4000)          /** \brief Periodicity of reading status of WD: 200 ms/ 50 us */

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
    boolean enStatusReading;                               /** \brief enable periodic WD status reading flag */
    uint16 wdStatusRegsAddresses[WD_STATUS_REGS_COUNT];    /** \brief array with adresses of WD status registers */
    uint16 lengthOfRegsToRead;                             /** \brief number of registers to read */
    uint16 monitoringMessageID;                            /** \brief message ID for sending back WD status data */
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
    .lengthOfRegsToRead = WD_STATUS_REGS_COUNT                                                                                 // number of addresses to read
};

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdConfigureWatchdog(USBReceiveData const * const commandPackage)
{
    // Temporary variables
    boolean isSuccessfulFlag = TRUE;            //Default true is used to handle write of series of data (AND logic on flags)
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
        g_wd1Parameters.wdConfig.ackPeriod = tmpConfigRegister.as_s.SpiSetResponsetimeWd1_u6 + (tmpConfigRegister.as_s.SpiSetResponsetimeWd1_u6 >> 1); // ACK WD at the middle of responce time interval
        g_wd1Parameters.wdConfig.wdType = selectedWD;
        g_wd1Parameters.isWDConfigured = TRUE;
        g_wd1Parameters.state = WD_STATE_CONFIGURED;
    }
    else
    {
        safety_logic_spi_config_wd2_ut tmpConfigRegister;
        tmpConfigRegister.as_uint16 = data[0];
        g_wd2Parameters.wdConfig.ackPeriod = tmpConfigRegister.as_s.SpiSetResponsetimeWd2_u6 + (tmpConfigRegister.as_s.SpiSetResponsetimeWd2_u6 >> 1); // ACK WD at the middle of responce time interval
        g_wd2Parameters.wdConfig.wdType = selectedWD;
        g_wd2Parameters.isWDConfigured = TRUE;
        g_wd2Parameters.state = WD_STATE_CONFIGURED;
    }
     

    #ifdef AB12_PLATFORM

    // Code for AB12 implementation
    
    if (selectedWD == WD1)
    {
        // WD3 in AB12
        g_wd1Parameters.wdConfig.ackPeriod = AB12_WD3_ACK_PERIOD; // Period: 10 ms
    }
    else
    {
        // Actual WD2 of Ab12
        g_wd2Parameters.wdConfig.ackPeriod = AB12_WD2_ACK_PERIOD; // Period: 600 us
    }

    isSuccessfulFlag = TRUE; // no configuration exists fro WD in AB12

    #else

    // Code for AB15 implementation

    // Write to ASIC
    for (uint8 i = 0; i < length; i++)
    {
        isSuccessfulFlag = QSPIWriteSequence(&(address[i]), &(data[i]), &length); // TODO: configuration, not yet implemented; not available for AB12
    }

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

    // Arm timer routine
    ConfigureWatchdogStatusCheckPeriodicity(WD_STATUS_CHECK_PERIOD);
    EnableWatchdogStatusCheckInterrupt();
}

void CmdStopMonitoringWatchdog(USBReceiveData const * const commandPackage)
{
    // Disable monitoring procedure
    g_wdStatusMonitoringConfig.enStatusReading = FALSE;

    // Unarm timer routine
    DisableWatchdogStatusCheckInterrupt();

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
    // AB12 platform
    responseWord = GetResponseWordAB12(requValue);

    // Send response word to ASIC
    length = 1;
    QSPIWriteSequence(&addressAnswer, &responseWord, &length);  // Use corresponding SPI instruction
    #else
    responseWord = GetResponseWordWD1AB15(requValue, 0);

    // Send response word to ASIC
    length = 1;
    QSPIWriteSequence(&addressAnswer, &responseWord, &length);
    #endif
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
    // AB12 platform
    responseWord = GetResponseWordAB12(requValue);
    // Send response word to ASIC
    length = 1;
    QSPIWriteSequence(&addressAnswer, &responseWord, &length);  // Use corresponding SPI instruction
    #else
    responseWord = GetResponseWordWD2AB15(requValue, 0);

    // Send response word to ASIC
    length = 1;
    QSPIWriteSequence(&addressAnswer, &responseWord, &length);
    #endif
}

void CmdStartWatchdog(USBReceiveData const * const commandPackage)
{
    // Local variables
    USBTransmitData packageToSend;

    // If any of the watchdogs is not configured do not start
    if ((g_wd1Parameters.isWDConfigured == FALSE) ||
        (g_wd2Parameters.isWDConfigured == FALSE)) 
    {
        packageToSend.status = USB_STATUS_ERROR;
        // Send message to GUI
        SendUSBPackage(&packageToSend);
        return;
    }

    // Configure periodicity of Watchdog serving MCU interrupt
    ConfigureWatchdogPeriodicity(WD1, g_wd1Parameters.wdConfig.ackPeriod);
    ConfigureWatchdogPeriodicity(WD2, g_wd2Parameters.wdConfig.ackPeriod);

    // Turn on Watchdog serving interrupt of MCU
    EnableWatchdog1Interrupt();
    EnableWatchdog2Interrupt();

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
    DisableWatchdog1Interrupt();
    DisableWatchdog2Interrupt();

    // Modify state and flags for WD
    g_wd1Parameters.isWDConfigured = FALSE;
    g_wd1Parameters.state = WD_STATE_IDLE;
    
    g_wd2Parameters.isWDConfigured = FALSE;
    g_wd2Parameters.state = WD_STATE_IDLE;

    // Prepare acknowledge message
    USBTransmitData packageToSend;
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
    SPIReceiveData data;
    uint16 length = g_wdStatusMonitoringConfig.lengthOfRegsToRead;

    // Read WD status from ASIC
    #ifdef AB12_PLATFORM
    // AB12 platform
    // responseWord = GetResponseWordAB12(requValue);   // TODO: use corresponding table
    #else
    // AB15 platform
    // Read WD related registers from ASIC
    QSPIReadSequence(g_wdStatusMonitoringConfig.wdStatusRegsAddresses, &data, &length);

    packageToSend.dataLength = length << 1; // each data item is send as 2 bytes

    for (uint8 i = 0; i < length; i++)
    {
        // Store SPI response frame to temporary variable for extracting data
        dataRecived.dw = data[i];

        packageToSend.data[i*2]     = GetLSB(dataRecived.bf.output_data);
        packageToSend.data[(i*2)+1] = GetMSB(dataRecived.bf.output_data);
    }
    #endif

    // Akcnowledge success of start WD
    packageToSend.status = USB_STATUS_DATA;
    packageToSend.asic_id = 1;
    packageToSend.msg_id = g_wdStatusMonitoringConfig.monitoringMessageID;

    // Send message to GUI
    SendUSBPackage(&packageToSend);
}

uint16 GetResponseWordAB12(uint8 challengeValue)
{
    // Table of Challenge-Response values (according to datasheet AB12)
    static uint16 responseWordArray[8] =  {0xE106,
                                            0x9671,
                                            0x4BAC,
                                            0x3CDB,
                                            0xD235,
                                            0xA542,
                                            0x789F,
                                            0x0FE8};
    static uint16 challengeWordArray[8] =  {0x2020,
                                            0xFDFD,
                                            0x8A8A,
                                            0x5757,
                                            0xECEC,
                                            0x3131,
                                            0x4646,
                                            0x9B9B};
    // Using if condition instead of simple array indexing
    // because it's unclear if order of coming Challenge words
    // will always be numerical
    for (uint8 i = 0; i<8; i++)
    {
        if (challengeValue == challengeWordArray[i])
        {
            // Provide value of requested Response word
            return (responseWordArray[i]);
        }
    }
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
