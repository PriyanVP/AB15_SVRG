/**********************************************************************************************************************
 * \file usb_wrapper.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "IfxAsclin_Asc.h"
#include "_Utilities/Ifx_Assert.h"
#include "common/usb_data_types.h"
#include "top/crc_wrapper.h"
#include "periphery/usb.h"
#include "top/usb_wrapper.h"

// for debugging commands JS 4/2024
#include "led.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define USB_START_BYTE          (0xAB)                          /** \brief USB start byte value                      */
#define USB_STOP_BYTE           (0xBA)                          /** \brief USB stop byte value                       */

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------
 * https://rb-tracker.bosch.com/tracker19/browse/ABEVBSW-68
 */
/*********************************************************************************************************************/

void USBInit(void)
{
    InitUSBInterface();
}

boolean SendUSBPackage(const USBTransmitData * data)
{
    // Validate that data is not null pointer
    IFX_ASSERT(IFX_VERBOSE_LEVEL_ERROR, data != NULL_PTR);

    // Initialize temporary variables
    uint8 buffer[MAX_USB_PACKAGE_LENGTH];
    Ifx_SizeT itemCounter = 0;

    // Fill buffer with package
    buffer[USB_STARTBYTE_POS] = USB_START_BYTE;             /* Start package byte  */
    itemCounter++;

    buffer[USB_MSG_ID_POS] = data->msg_id;
    itemCounter++;

    buffer[USB_ASIC_ID_POS] = data->asic_id;
    itemCounter++;

    buffer[USB_CMD_STAT_POS] = data->status;
    itemCounter++;

    buffer[USB_PAYLOAD_LEN_POS] = data->dataLength; /*   Payload length   */
    itemCounter++;

    for (uint16 i = 0; i < data->dataLength; i++)
    {
        // TODO: needs check for same function JS
        buffer[USB_PAYLOAD_POS + i] = data->data[i]; /* Payload */
        itemCounter++;
    }
    // TODO: refactor
    buffer[itemCounter] = GetCRC8(&(buffer[1]), itemCounter-1); /*   CRC, ommit 1st byte   */
    itemCounter++;

    buffer[itemCounter] = USB_STOP_BYTE;             /* End package byte  */
    itemCounter++;

    // Check if enough space in UART buffer
    sint32 bytesLeft;
    do
    {
        // TODO : ["../top/usb_wrapper.c" 106/39] calling a function without a prototype
        bytesLeft = GetWriteBufferLeft();
    }
    while (bytesLeft < itemCounter);

    // Send package
    uint8 result = SendUSBData(buffer, &itemCounter);

    return result;
}

boolean ReceiveUSBPackage(USBReceiveData * const data)
{
    // Validate that data is not null pointer
    IFX_ASSERT(IFX_VERBOSE_LEVEL_ERROR, data != NULL_PTR);

    // Initialize temporary variables
    static uint8 buffer[MAX_USB_PACKAGE_LENGTH] = {0};
    static Ifx_SizeT messageLength = 0;
    static Ifx_SizeT crcLength = 0;
    static Ifx_SizeT remainingBytesInMessange = 0;
    uint8 expectedCrc;
    static boolean previousReadNotFinished = FALSE;
    Ifx_SizeT bytesToRead = 0;
    volatile sint32 bytesIncoming;

    /*buffer is read in 3 steps, now doing step 1/3 */
    // Read beginning of package if previous read was finished
    if (previousReadNotFinished == FALSE)
    {
        // Return if no items in buffer
        if (IsInBufferEmpty()) return FALSE;

        // Clean buffer
        for (int i = 0; i < MAX_USB_PACKAGE_LENGTH; i++) buffer[i] = 0; // TODO: can be optimized

        // Start reading sequence
        // Read first byte of message, searching for USB_START_BYTE
        bytesToRead = USB_STARTBYTE_LENGTH;
        ReceiveUSBData(buffer, &bytesToRead);

        // Verify that first byte of data is start byte
        if (buffer[USB_STARTBYTE_POS] != USB_START_BYTE)
        {
            IFX_ASSERT(IFX_VERBOSE_LEVEL_ERROR, FALSE);

            //poll buffer[0] for start of message
            //Read buffer by byte till we find USB_START_BYTE
            //TODO refactor :  loop is ended in case first byte is found, if not the loop will loop "for ever" shall we find a better soution
            while (buffer[USB_STARTBYTE_POS] != USB_START_BYTE)
            {
                // If read all buffer, but no required start byte - return
                if (IsInBufferEmpty()) return FALSE;

                bytesToRead = USB_STARTBYTE_LENGTH;
                ReceiveUSBData(buffer, &bytesToRead);
            }
        }


        /*buffer is read in 3 steps, now doing read step 2/3 */
        /*we found startbyte now read next bytes up to payload length in order to handle final read correct*/
        bytesToRead = (USB_MSG_ID_LENGTH + USB_ASIC_ID_LENGTH + USB_CMD_LENGTH + USB_PAYLOADLEN_LENGTH);
        ReceiveUSBData(&(buffer[USB_MSG_ID_POS]), &bytesToRead);

        /*construct the number of remaining bytes to read */
        remainingBytesInMessange =  buffer[USB_PAYLOAD_LEN_POS] + USB_CRC_LENGTH + USB_STOPBYTE_LENGTH; /* bytes = payload  + CRC + end byte */

    }

    // Check if package fully received

    bytesIncoming = GetReadBufferLeft(); // TODO: inlcude not found
    if (bytesIncoming < remainingBytesInMessange)
    {
        previousReadNotFinished = TRUE;
        return FALSE;
    }
    else
    {
        // TODO: might also check for Buffer end byte?
        previousReadNotFinished = FALSE;
    }

    /*buffer is read in 3 steps, now doing read step 3/3 */
    // Read last part of message (till end)
    ReceiveUSBData(&(buffer[USB_PAYLOAD_POS]), &remainingBytesInMessange);
    // reset the value becaus it coul have been modified


    // construct full Message leghth
    //itemCounter = USB_PAYLOAD_POS + itemCounter; /* all package length */
    messageLength = USB_STARTBYTE_LENGTH + USB_MSG_ID_LENGTH + USB_ASIC_ID_LENGTH + USB_CMD_LENGTH + \
            USB_PAYLOADLEN_LENGTH + buffer[USB_PAYLOAD_LEN_POS] + USB_CRC_LENGTH + USB_STOPBYTE_LENGTH;


    // Verify that second part of data is correct
    if (buffer[messageLength - 1] != USB_STOP_BYTE)
    {
        IFX_ASSERT(IFX_VERBOSE_LEVEL_ERROR, FALSE);
        return FALSE;
    }

    // Check that CRC is correct (CRC occupies (end-1)th byte)
    // calc CRC over the full meaasge without start, stop and crc
    crcLength = messageLength - (USB_STARTBYTE_LENGTH + USB_CRC_LENGTH + USB_STOPBYTE_LENGTH);
    expectedCrc = buffer[messageLength - (USB_CRC_LENGTH + USB_STOPBYTE_LENGTH)];
    if (IsCRC8Correct(&(buffer[USB_MSG_ID_POS]), crcLength, expectedCrc) == FALSE)
    {
        IFX_ASSERT(IFX_VERBOSE_LEVEL_ERROR, FALSE);
        return FALSE;
    }

    // Unpack data from buffer to struct
    data->msg_id     = buffer[USB_MSG_ID_POS];
    data->asic_id    = buffer[USB_ASIC_ID_POS];
    data->command    = (USBCommandsEnum) buffer[USB_CMD_STAT_POS];
    data->dataLength = buffer[USB_PAYLOAD_LEN_POS];
    for (uint16 i = 0; i < data->dataLength; i++)
    {
        data->data[i] = buffer[USB_PAYLOAD_POS + i];
    }

    // Data receivement and unpacking was successful
    return TRUE;
}
