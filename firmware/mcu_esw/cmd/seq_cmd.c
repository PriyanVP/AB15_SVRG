/**********************************************************************************************************************
 * \file seq_cmd.c
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

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define RW_ITEM_SIZE        (6)           /** \brief Size of 1 item in RW command package (rw flag - address - data) */
#define READ_ITEM_SIZE      (2)           /** \brief Size of 1 item in read command package (address)                */
#define WRITE_ITEM_SIZE     (4)           /** \brief Size of 1 item in read command package (address - data)         */

const uint8 rwSeqLength    = MAX_USB_RECIEVE_PAYLOAD_LENGTH / RW_ITEM_SIZE;
const uint8 readSeqLength  = MAX_USB_RECIEVE_PAYLOAD_LENGTH / READ_ITEM_SIZE;
const uint8 writeSeqLength = MAX_USB_RECIEVE_PAYLOAD_LENGTH / WRITE_ITEM_SIZE;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdExecuteRWSequence(USBReceiveData const * const commandPackage)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    uint16 address[rwSeqLength];
    uint32 data[rwSeqLength];
    enum RWFlag rwOption[rwSeqLength];
    SPIReceiveData dataRecived;
    uint16 length = (commandPackage->dataLength)/RW_ITEM_SIZE;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    for (uint8 i = 0; i < length; i++)
    {
        // Data array address
        uint8 dataArrayIdx = i*RW_ITEM_SIZE;

        // Unpack byte array item into corresponding variables
        rwOption[i] = commandPackage->data[dataArrayIdx];
        address[i]  = ConstructWordFromBytes(commandPackage->data[dataArrayIdx+3], commandPackage->data[dataArrayIdx+2]);
        data[i]     = ConstructWordFromBytes(commandPackage->data[dataArrayIdx+5], commandPackage->data[dataArrayIdx+4]);
    }

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
            // Fill data in error frame with invalid response from CS600
            // length variable holds first error SPI frame index
            packageToSend.dataLength = 4;
            packageToSend.data[0] = GetByteByIdx(0, data[length-1]);
            packageToSend.data[1] = GetByteByIdx(1, data[length-1]);
            packageToSend.data[2] = GetByteByIdx(2, data[length-1]);
            packageToSend.data[3] = GetByteByIdx(3, data[length-1]);
        }
    }
    else
    {
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = length << 1; // each data item is send as 2 bytes

        for (uint8 i = 0; i < length; i++)
        {
            // Store SPI response frame to temporary variable for extracting data
            dataRecived.dw = data[i];

            packageToSend.data[i*2]     = GetLSB(dataRecived.bf.output_data);
            packageToSend.data[(i*2)+1] = GetMSB(dataRecived.bf.output_data);
        }
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdExecuteReadSequence(USBReceiveData const * const commandPackage)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    uint16 address[readSeqLength];
    uint32 data[readSeqLength];
    enum RWFlag rwOption[readSeqLength];
    SPIReceiveData dataRecived;
    uint16 length = (commandPackage->dataLength)/READ_ITEM_SIZE;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    for (uint8 i = 0; i < length; i++)
    {
        // Data array address
        uint8 dataArrayIdx = i*READ_ITEM_SIZE;

        // Unpack byte array item into corresponding variables
        rwOption[i] = READ;
        address[i]  = ConstructWordFromBytes(commandPackage->data[dataArrayIdx+1], commandPackage->data[dataArrayIdx]);
    }

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
            // Fill data in error frame with invalid response from CS600
            // length variable holds first error SPI frame index
            packageToSend.dataLength = 4;
            packageToSend.data[0] = GetByteByIdx(0, data[length-1]);
            packageToSend.data[1] = GetByteByIdx(1, data[length-1]);
            packageToSend.data[2] = GetByteByIdx(2, data[length-1]);
            packageToSend.data[3] = GetByteByIdx(3, data[length-1]);
        }
    }
    else
    {
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = length << 1; // each data item is send as 2 bytes

        for (uint8 i = 0; i < length; i++)
        {
            // Store SPI response frame to temporary variable for extracting data
            dataRecived.dw = data[i];

            packageToSend.data[i*2]     = GetLSB(dataRecived.bf.output_data);
            packageToSend.data[(i*2)+1] = GetMSB(dataRecived.bf.output_data);
        }
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdExecuteWriteSequence(USBReceiveData const * const commandPackage)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    uint16 address[writeSeqLength];
    uint32 data[writeSeqLength];
    enum RWFlag rwOption[writeSeqLength];
    SPIReceiveData dataRecived;
    uint16 length = (commandPackage->dataLength)/WRITE_ITEM_SIZE;
    boolean isSuccessfulFlag = FALSE;

    // Unpack received data to variables
    for (uint8 i = 0; i < length; i++)
    {
        // Data array address
        uint8 dataArrayIdx = i*WRITE_ITEM_SIZE;

        // Unpack byte array item into corresponding variables
        rwOption[i] = WRITE;
        address[i]  = ConstructWordFromBytes(commandPackage->data[dataArrayIdx+1], commandPackage->data[dataArrayIdx+0]);
        data[i]     = ConstructWordFromBytes(commandPackage->data[dataArrayIdx+3], commandPackage->data[dataArrayIdx+2]);
    }

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
            // Fill data in error frame with invalid response from CS600
            // length variable holds first error SPI frame index
            packageToSend.dataLength = 4;
            packageToSend.data[0] = GetByteByIdx(0, data[length-1]);
            packageToSend.data[1] = GetByteByIdx(1, data[length-1]);
            packageToSend.data[2] = GetByteByIdx(2, data[length-1]);
            packageToSend.data[3] = GetByteByIdx(3, data[length-1]);
        }
    }
    else
    {
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 0;
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}
