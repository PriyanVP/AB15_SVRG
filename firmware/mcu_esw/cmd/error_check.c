/**********************************************************************************************************************
 * \file error_check.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Platform_Types.h"
#include "common/usb_data_types.h"
#include "common/spi_data_types.h"
#include "common/bit_manipulation.h"
#include "periphery/timer.h"
#include "top/spi_wrapper.h"
#include "general_cmd.h"
#include "error_check.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define ERR_STATUS_1        0x00B0              /** \brief Address of 1st CS600 error status register                */
#define ERR_STATUS_2        0x00B1              /** \brief Address of 2nd CS600 error status register                */
#define ERR_STATUS_3        0x00B2              /** \brief Address of 3rd CS600 error status register                */
#define ERR_STATUS_4        0x00B3              /** \brief Address of 4th CS600 error status register                */
#define ERR_STATUS_5        0x00B4              /** \brief Address of 5th CS600 error status register                */
#define ERR_STATUS_6        0x00B5              /** \brief Address of 6th CS600 error status register                */
#define ERR_CNT_STATUS      0x00B6              /** \brief Address of errors counter CS600 error status register     */

#define ERROR_LENGTH        7                   /** \brief Number of error status registers                          */

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static uint8 errorCheckMessageID;            /** \brief Variable to store message ID of USB_CMD_START_READ_ERRORS    */

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdStartReadErrors(USBReceiveData const * const commandPackage)
{
    errorCheckMessageID = commandPackage->msg_id;
    ConfigureErrorCheckPeriodicity(20);
    EnableErrorCheckInterrupt();
    CmdIsAlive(commandPackage);
}

void CmdStopReadErrors(USBReceiveData const * const commandPackage)
{
    DisableErrorCheckInterrupt();
    CmdIsAlive(commandPackage);
}

void IntCmdCS600ErrorCheck(void)
{
    USBTransmitData packageToSend;
    static uint16 errorMangerAddressRead[ERROR_LENGTH] = {ERR_CNT_STATUS, ERR_STATUS_1, ERR_STATUS_2, ERR_STATUS_3, ERR_STATUS_4, ERR_STATUS_5, ERR_STATUS_6};
    uint16 dataToWrite[ERROR_LENGTH];
    SPIReceiveData errorManagerData[ERROR_LENGTH];
    uint16 length = ERROR_LENGTH;
    boolean isSuccessfulFlag;

    // use saved msg ID for sending message
    packageToSend.msg_id = SetResponseBit(errorCheckMessageID);

    // Read all error fields from CS600
    isSuccessfulFlag = QSPIReadSequence(&errorMangerAddressRead, &errorManagerData, &length);

    if (isSuccessfulFlag == FALSE)
    {
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;
    }
    else
    {
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = ERROR_LENGTH * 2;

        for (uint8 i = 0; i < ERROR_LENGTH; i++)
        {
            packageToSend.data[i*2]     = GetLSB(errorManagerData[i].bf.output_data);
            packageToSend.data[(i*2)+1] = GetMSB(errorManagerData[i].bf.output_data);
            dataToWrite[i] = errorManagerData[i+1].bf.output_data; // last element unused
        }

        // Change length for clearing error status registers
        length = ERROR_LENGTH - 1;

        // Clear error flags
        QSPIWriteSequence(&errorMangerAddressRead[1], dataToWrite, &length);
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}
