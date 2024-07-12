/**********************************************************************************************************************
 * \file general_cmd.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/usb_data_types.h"
#include "common/spi_data_types.h"
#include "common/command_queue.h"
#include "common/bit_manipulation.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "top/version.h"
#include "periphery/led.h"
#include "general_cmd.h"
/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdIsAlive(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;
    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdGetMcuVersion(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;

    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_STATUS;
    packageToSend.dataLength = 3;
    packageToSend.data[0] = (char)VERSION_MAJOR;
    packageToSend.data[1] = (char)VERSION_MINOR;
    packageToSend.data[2] = (char)VERSION_PATCH;
    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdGetMcuBuildDate(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;

    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_STATUS;
    packageToSend.dataLength = 8;
    packageToSend.data[0] = BUILD_YEAR_CH0;
    packageToSend.data[1] = BUILD_YEAR_CH1;
    packageToSend.data[2] = BUILD_YEAR_CH2;
    packageToSend.data[3] = BUILD_YEAR_CH3;
    packageToSend.data[4] = BUILD_MONTH_CH0;
    packageToSend.data[5] = BUILD_MONTH_CH1;
    packageToSend.data[6] = BUILD_DAY_CH0;
    packageToSend.data[7] = BUILD_DAY_CH1;
    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdGetMcuBuildTime(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;

    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_STATUS;
    packageToSend.dataLength = 4;
    packageToSend.data[0] = BUILD_HOUR_CH0;
    packageToSend.data[1] = BUILD_HOUR_CH1;
    packageToSend.data[2] = BUILD_MIN_CH0;
    packageToSend.data[3] = BUILD_MIN_CH1;
    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdSpiInstuction(USBReceiveData const * const commandPackage)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    uint16 instruction;
    uint32 data;
    SPIReceiveData dataRecived;
    //TODO: rename RWFlag rwOption, is rudiment from CS 600
    enum RWFlag rwOption = READ;
    // for AB12 length will always be 1?
    uint16 length = 1;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    instruction = ConstructWordFromBytes(commandPackage->data[1], commandPackage->data[0]);

    // Send data to SPI with waiting for response
    isSuccessfulFlag = QSPIReadWriteSequence(&instruction, &data, &rwOption, &length);

    // Construct package to PC
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // Construct packages based on error status
    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;

        // Check if SPI response frame was received
        if (length > 0)
        {
            // Fill data in error frame with invalid response from ASIC
            packageToSend.dataLength = 4;
            packageToSend.data[0] = GetByteByIdx(0, data);
            packageToSend.data[1] = GetByteByIdx(1, data);
            packageToSend.data[2] = GetByteByIdx(2, data);
            packageToSend.data[3] = GetByteByIdx(3, data);
        }
    }
    else
    {
        // Store SPI response frame to temporary variable for extracting data
        dataRecived.dw = data;

        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 5;
        packageToSend.data[0] = dataRecived.bf.gen_status;
        packageToSend.data[1] = dataRecived.bf.s_bit;
        packageToSend.data[2] = dataRecived.bf.sid_add_status;
        packageToSend.data[3] = GetLSB(dataRecived.bf.output_data);
        packageToSend.data[4] = GetMSB(dataRecived.bf.output_data);

    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdGetDeviceId(USBReceiveData * commandPackage)
{
    USBTransmitData packageToSend;

    // SPI instruction for get device ID
    commandPackage->data[0] = 0;
    commandPackage->data[1] = 0;

    handleSpiInstr(&packageToSend, commandPackage);

    if (packageToSend.status != USB_STATUS_ERROR)
    {
        /*handleSpiInstr was successful, we change content of receive package*/
        packageToSend.status = USB_STATUS_ACK;
        // only return device Id which is in LSb of output data
        packageToSend.dataLength = 1;
        packageToSend.data[0] =  packageToSend.data[3];
    }
    // Send data back to MCU
    SendUSBPackage(&packageToSend);

}

void handleCmdInstr(USBReceiveData const * const commandPackage)
{
    // TODO: tmp solution, refactoring pending
    USBTransmitData packageToSend;
    handleSpiInstr(&packageToSend, commandPackage);

    // Send data back to MCU
    SendUSBPackage(&packageToSend);

}

void handleSpiInstr(USBTransmitData * packageToSend, USBReceiveData const * const commandPackage)
{
    // TODO: tmp solution, refactoring pending
    // TODO: approach for passing data should be clarified
    // Parameters for SPI packages and variable to store output data

    uint16 instruction;
    uint32 data;
    SPIReceiveData dataRecived;
    //TODO: rename RWFlag rwOption, is rudiment from CS 600
    enum RWFlag rwOption = READ;
    uint16 length = 1;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    instruction = ConstructWordFromBytes(commandPackage->data[1], commandPackage->data[0]);

    // Send data to SPI with waiting for response
    isSuccessfulFlag = QSPIReadWriteSequence(&instruction, &data, &rwOption, &length);

    // Construct package to PC
    packageToSend->msg_id = SetResponseBit(commandPackage->msg_id);

    // Construct packages based on error status
    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend->status = USB_STATUS_ERROR;
        packageToSend->dataLength = 0;

        /* Check if SPI response frame was received used in CS600
        if (length > 0)
        {
            // Fill data in error frame with invalid response from CS600
            packageToSend->dataLength = 4;
            packageToSend->data[0] = GetByteByIdx(0, data);
            packageToSend->data[1] = GetByteByIdx(1, data);
            packageToSend->data[2] = GetByteByIdx(2, data);
            packageToSend->data[3] = GetByteByIdx(3, data);
        }*/
    }
    else
    {
        // Store SPI response frame to temporary variable for extracting data
        dataRecived.dw = data;

        packageToSend->status = USB_STATUS_DATA;
        packageToSend->dataLength = 5;
        packageToSend->data[0] = dataRecived.bf.gen_status;
        packageToSend->data[1] = dataRecived.bf.s_bit;
        packageToSend->data[2] = dataRecived.bf.sid_add_status;
        packageToSend->data[3] = GetLSB(dataRecived.bf.output_data);
        packageToSend->data[4] = GetMSB(dataRecived.bf.output_data);

    }
    // Sendig data back is performed by calling function
}

#ifdef CS600
void CmdWriteReg(USBReceiveData const * const commandPackage)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    uint16 address;
    uint32 data;
    SPIReceiveData dataRecived;
    enum RWFlag rwOption = WRITE;
    uint16 length = 1;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    address = ConstructWordFromBytes(commandPackage->data[1], commandPackage->data[0]);
    data = ConstructWordFromBytes(commandPackage->data[3], commandPackage->data[2]);

    // Send data to SPI with waiting for response
    isSuccessfulFlag = QSPIReadWriteSequence(&address, &data, &rwOption, &length);

    // Construct package to PC
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // Construct packages based on error status
    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;

        // Check if SPI response frame was received
        if (length > 0)
        {
            // Fill data in error frame with invalid response from ASIC
            packageToSend.dataLength = 4;
            packageToSend.data[0] = GetByteByIdx(0, data);
            packageToSend.data[1] = GetByteByIdx(1, data);
            packageToSend.data[2] = GetByteByIdx(2, data);
            packageToSend.data[3] = GetByteByIdx(3, data);
        }
    }
    else
    {
        // Store SPI response frame to temporary variable for extracting data
        dataRecived.dw = data;

        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 2;
        packageToSend.data[0] = GetLSB(dataRecived.bf.output_data);
        packageToSend.data[1] = GetMSB(dataRecived.bf.output_data);
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdWriteRegRaw(USBReceiveData const * const commandPackage)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    uint16 address;
    uint32 data;
    enum RWFlag rwOption = WRITE;
    uint16 length = 1;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    address = ConstructWordFromBytes(commandPackage->data[1], commandPackage->data[0]);
    data = ConstructWordFromBytes(commandPackage->data[3], commandPackage->data[2]);

    // Send data to SPI with waiting for response
    isSuccessfulFlag = QSPIReadWriteSequence(&address, &data, &rwOption, &length);

    // Construct package to PC
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // Construct packages based on error status
    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;

        // Check if SPI response frame was received
        if (length > 0)
        {
            // Fill data in error frame with invalid response from ASIC
            packageToSend.dataLength = 4;
            packageToSend.data[0] = GetByteByIdx(0, data);
            packageToSend.data[1] = GetByteByIdx(1, data);
            packageToSend.data[2] = GetByteByIdx(2, data);
            packageToSend.data[3] = GetByteByIdx(3, data);
        }
    }
    else
    {
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 4;
        packageToSend.data[0] = GetByteByIdx(0, data);
        packageToSend.data[1] = GetByteByIdx(1, data);
        packageToSend.data[2] = GetByteByIdx(2, data);
        packageToSend.data[3] = GetByteByIdx(3, data);
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdReadRegRaw(USBReceiveData const * const commandPackage)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    uint16 address;
    uint32 data;
    SPIReceiveData dataRecived;
    enum RWFlag rwOption = READ;
    uint16 length = 1;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    address = ConstructWordFromBytes(commandPackage->data[1], commandPackage->data[0]);

    // Send data to SPI with waiting for response
    isSuccessfulFlag = QSPIReadWriteSequence(&address, &data, &rwOption, &length);

    // Construct package to PC
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // Construct packages based on error status
    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;

        // Check if SPI response frame was received
        if (length > 0)
        {
            // Fill data in error frame with invalid response from ASIC
            packageToSend.dataLength = 4;
            packageToSend.data[0] = GetByteByIdx(0, data);
            packageToSend.data[1] = GetByteByIdx(1, data);
            packageToSend.data[2] = GetByteByIdx(2, data);
            packageToSend.data[3] = GetByteByIdx(3, data);
        }
    }
    else
    {
        // Store SPI response frame to temporary variable for extracting data
        dataRecived.dw = data;

        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 4;
        packageToSend.data[0] = GetByteByIdx(0, data);
        packageToSend.data[1] = GetByteByIdx(1, data);
        packageToSend.data[2] = GetByteByIdx(2, data);
        packageToSend.data[3] = GetByteByIdx(3, data);
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}
#endif // CS600
