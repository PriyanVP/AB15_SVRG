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

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Structure for Watchdog serving MCU flags
 */
typedef struct
{
    boolean isEnabledAckWatchdog;
    boolean isEnabledMonitoringWatchdog;
    boolean isConfigured;
} WatchdogManagementFlagsStruct;

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

WatchdogManagementFlagsStruct WatchdogManagementFlags =
{
    .isEnabledAckWatchdog = 1, // MCU serves Watchdog requests from ASIC by default
    .isEnabledMonitoringWatchdog = 0, // No monitoring by default
    .isConfigured = 0 // Watchdog presumed to be unconfigured until GUI explicitly performs configuration
};

// Variable to store message ID of USB_CMD_START_MONITORING_WATCHDOG command (to properly construct MCU->PC messages of continuous command)
static uint8 monitoringMessageID;

// Variable to store RESPTIME value for configuration of Watchdog serving periodicity
static uint16 respTimeValue;

// Variable to store General Timer's interrupt duration calculated in microseconds
static uint32 durationOfTimerInterruptUs;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdConfigureWatchdog(USBReceiveData const * const commandPackage)
{
    boolean isSuccessfulFlag = FALSE;

    // Prepare package for writing into ASIC
    uint16 address[WD_CFG_PACKAGE_LEN] = {WD_CONFIG0_ADDRESS, WD_CONFIG1_ADDRESS, WD_TIME_WIN_ADDRESS, WD_RESPTIME_ADDRESS}; // Addresses of registers to write
    uint16 data[WD_CFG_PACKAGE_LEN] = {0, 0, 0, 0};  // Values to write
    uint16 length = WD_CFG_PACKAGE_LEN; // Number of 32bit SPI words to write into ASIC

    // Unpack received message from GUI to obtain register values
    data[0] = ConstructWordFromBytes(0, commandPackage->data[0]); // WD_CONFIG0 register value (WDG_ENABLE field)
    data[1] = ConstructWordFromBytes(0, commandPackage->data[1]); // WD_CONFIG1 register value (WDG_EC_TH1, WDG_EN_LOCK, T_WIN_INITIAL fields)
    data[2] = ConstructWordFromBytes(commandPackage->data[3], commandPackage->data[2]); // WD_TIME_WIN register value (T_WIN field)
    data[3] = ConstructWordFromBytes(commandPackage->data[5], commandPackage->data[4]); // WD_RESPTIME register value (RESPTIME field)

    // Store RESPTIME value for configuration of Watchdog serving periodicity
    respTimeValue = (uint16)data[3];

    // Write to ASIC

    //TODO: JS: 17.6. bruteforce set to true to allow enable watchdog for test purpose +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    isSuccessfulFlag = TRUE;
    //isSuccessfulFlag = QSPIWriteSequence(&(address), &(data), &length); // TODO: configuration, not yet implemented; not available for AB12

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
        WatchdogManagementFlags.isConfigured = 1;
        packageToSend.status = USB_STATUS_ACK;
        packageToSend.dataLength = 0;
    }

    // Send report to GUI
    SendUSBPackage(&packageToSend);
}

void CmdGetConfigWatchdog(USBReceiveData const * const commandPackage)
{
    // Prepare for reading and read registers
    uint16 address[WD_CFG_PACKAGE_LEN] = {WD_CONFIG0_ADDRESS, WD_CONFIG1_ADDRESS, WD_TIME_WIN_ADDRESS, WD_RESPTIME_ADDRESS}; // Addresses of registers to read
    SPIReceiveData data[WD_CFG_PACKAGE_LEN];  // Values of registers
    uint16 length = WD_CFG_PACKAGE_LEN;
    // Read from ASIC
    boolean isSuccessfulFlag = FALSE;
    isSuccessfulFlag = QSPIReadSequence(&address, &data, &length);
    SPIReceiveData dataRecived;


    // Prepare message for GUI
    USBTransmitData packageToSend;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;
    }
    else
    {   // Reading was successful
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 6; // Six data bytes in MCU->PC package by command definition
        // Repack register values into bytes
        packageToSend.data[0] = GetLSB(data[0].bf.output_data); // WD_CONFIG0 (WDG_ENABLE field)
        packageToSend.data[1] = GetLSB(data[1].bf.output_data); // WD_CONFIG1 (WDG_EC_TH1, WDG_EN_LOCK, T_WIN_INITIAL fields)
        packageToSend.data[2] = GetLSB(data[2].bf.output_data); // WD_TIME_WIN (T_WIN field)
        packageToSend.data[3] = GetMSB(data[2].bf.output_data); // WD_TIME_WIN (T_WIN field)
        packageToSend.data[4] = GetLSB(data[3].bf.output_data); // WD_RESPTIME (RESPTIME field)
        packageToSend.data[5] = GetMSB(data[3].bf.output_data); // WD_RESPTIME (RESPTIME field)
    }
    // Send report to GUI
    SendUSBPackage(&packageToSend);
}

void CmdStartMonitoringWatchdog(USBReceiveData const * const commandPackage)
{
    // Save message ID
    monitoringMessageID = commandPackage->msg_id;
    // Enable monitoring procedure
    WatchdogManagementFlags.isEnabledMonitoringWatchdog = 1;
}

void CmdStopMonitoringWatchdog(USBReceiveData const * const commandPackage)
{
    // Disable monitoring procedure
    WatchdogManagementFlags.isEnabledMonitoringWatchdog = 0;

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;
    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void IntCmdServeWatchdog(void)
{
    uint8 requValue = 0;
    uint16 responseWord0, responseWord1 = 0;
    uint16 address, length = 0;

    // Get request from ASIC's Watchdog
    // No checks to not serve same Watchdog request more than once (expected behavior in case of failed previous serving)
    requValue = GetREQUValue();

    // Obtain response word from look-up table
    responseWord1 = GetResponseWordAb12(requValue, 1);
    responseWord0 = GetResponseWordAb12(requValue, 0);

    // Send response words to ASIC
    address = WD_RESP_ADDRESS;
    length = 1;
    QSPIWriteSequence(&address, &responseWord1, &length);
    QSPIWriteSequence(&address, &responseWord0, &length);

    // Monitoring
    if (WatchdogManagementFlags.isEnabledMonitoringWatchdog == TRUE)
    {
        // Message to PC
        USBTransmitData packageToSend;
        packageToSend.msg_id = SetResponseBit(monitoringMessageID);

        // Status register (WD_REQU)
        address = WD_REQU_ADDRESS;
        SPIReceiveData data;
        length = 1;
        boolean isSuccessfulFlag = QSPIReadSequence(&address, &(data.dw), &length);

        if (isSuccessfulFlag == FALSE)
        {
            // Reading of status register has failed, construct blanc error MCU->PC message
            // Common error frame setup
            packageToSend.status = USB_STATUS_ERROR;
            packageToSend.dataLength = 0;
        }
        else
        {
            // Timestamp value, in ms
            uint16 timeStampValueMs = (GetWatchdogPeriodicity()) * durationOfTimerInterruptUs/1000 /* us->ms coefficient*/;
            // Construct full Monitoring response message
            packageToSend.status = USB_STATUS_DATA;
            packageToSend.dataLength = 8;
            // Timestamp value
            packageToSend.data[0] = GetLSB(timeStampValueMs);
            packageToSend.data[1] = GetMSB(timeStampValueMs);
            // Response words
            packageToSend.data[2] = GetLSB(responseWord1);
            packageToSend.data[3] = GetMSB(responseWord1);
            packageToSend.data[4] = GetLSB(responseWord0);
            packageToSend.data[5] = GetMSB(responseWord0);
            // Status register
            // Insert requ value from completed monitoring window into status register's value
            uint16 wdREQURegValue = ((((uint16)data.bf.output_data) & REQU_WRITEMASK) | (requValue << REQU_OFFSET));
            packageToSend.data[6] = GetLSB(wdREQURegValue);
            packageToSend.data[7] = GetMSB(wdREQURegValue);
        }

        // Send message to PC
        SendUSBPackage(&packageToSend);
    }
}

void CmdStartWatchdog(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.dataLength = 0;
    uint16 address, length, data;

    if (WatchdogManagementFlags.isConfigured == TRUE)
    {
        // Sync event
        // Write Response Word 0 twice to cause start of new monitoring cycle
        address = WD_RESP_ADDRESS;
        length = 1;
        data = GetResponseWordAb12(0, 0);
        //QSPIWriteSequence(&address, &data, &length);  // TODO: handling ASIC communication for watchdog, not implemented
        //QSPIWriteSequence(&address, &data, &length);

        // Configure periodicity of Watchdog serving MCU interrupt
        ConfigureWatchdogPeriodicity(CalculateWatchdogAckPeriodicity());
        // Turn on Watchdog serving interrupt of MCU
        EnableWatchdogInterrupt();
        // Responses to Watchdog requests will be now generated by MCU starting with next Watchdog acknowledgement interrupt
        WatchdogManagementFlags.isEnabledAckWatchdog = 1;
        packageToSend.status = USB_STATUS_ACK;
    }
    else
    {
        // Watchdog serving can't be enabled if no successful WD configuration was done beforehand
        packageToSend.status = USB_STATUS_ERROR;
    }

    // Send message to GUI
    SendUSBPackage(&packageToSend);
}

void CmdStopWatchdog(USBReceiveData const * const commandPackage)
{
    // Turn off Watchdog serving interrupt of MCU
    DisableWatchdogInterrupt();
    // No responses to Watchdog requests will be now generated by MCU
    WatchdogManagementFlags.isEnabledAckWatchdog = 0;

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;
    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

uint16 GetResponseWordAb12(uint8 requValue, boolean respWrdNumber)
{
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

uint8 GetRespCnt(void)
{
    // Read ASIC's WD_REQU register
    uint16 address = WD_REQU_ADDRESS;
    SPIReceiveData data;
    uint16 length = 1;
    // Read from ASIC
    QSPIReadSequence(&address, &(data.dw), &length);
    // Obtain RESP_CNT field value from register's content
    uint8 respCntValue = (data.bf.output_data & RESP_CNT_READMASK) >> RESP_CNT_OFFSET;
    return respCntValue;
}

uint8 GetREQUValue(void)
{
    // Read ASIC's WD_REQU register
    uint16 address = WD_REQU_ADDRESS;
    SPIReceiveData data;
    uint16 length = 1;
    // Read from ASIC
    QSPIReadSequence(&address, &(data.dw), &length);
    // Obtain REQU field value from register's content
    uint8 requValue = (data.bf.output_data & REQU_READMASK) >> REQU_OFFSET;
    return requValue;
}

uint16 CalculateWatchdogAckPeriodicity(void)
{
    //TODO: can be optimized using IfxGpt12_T3_getFrequency (base freq of GPT12 + prescaler). Check

    // Should be equal to or longer than RESP_TIME and shorter than RESP_TIME + TIME_WINDOW durations
    // T_timer_itr(s) = (GENERAL_TIMER_PERIODICITY * Gpt1BlockPrescaler ∗ TimerInputPrescaler) / (FREQ_GPT12_HZ)
    // TODO: find a vay to use defines for Gpt1BlockPrescaler and TimerInputPrescaler (enum values of those constants aren't suited for straightforward translation, find a solution)
    durationOfTimerInterruptUs = (GENERAL_TIMER_PERIODICITY * 16 * 2) / (100); // Configured duration of General Timer interrupt, microseconds; 1/100 = (s->us constant)/(FREQ_GPT12_HZ)

    // Configured duration of ASIC's Watchdog Response Time, microseconds (datasheet translation: Response Time = RESPTIME_CFG * 0.1, ms)
    volatile uint32 respTimeValueUs = respTimeValue*100;
    volatile uint16 watchdogAckPeriodicity = respTimeValueUs/durationOfTimerInterruptUs;
    return watchdogAckPeriodicity;
}
