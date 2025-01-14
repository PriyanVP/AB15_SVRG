/**********************************************************************************************************************
 * \file hacked_timer_cmd.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Ifx_Types.h"
#include "Ifx_Ssw_Compilers.h"
#include "common/global_defines.h"
#include "common/spi_data_types.h"
#include "common/bit_manipulation.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "periphery/timer.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "hacked_timer_cmd.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define HACKED_TIMEOUT     (40000)              /** \brief Timeout of 2 s - TODO: refactor */

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/


/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/
uint8 g_msgID;
/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void StartHackedTimer(USBReceiveData const * const commandPackage)
{
    // Configure periodicity of Test mode check interrupt
    ConfigureTimerPeriodicity(HACKED_TIMER, HACKED_TIMEOUT);

    // Turn on Test mode interrupt of MCU
    EnableTimerInterrupt(HACKED_TIMER);

    g_msgID = SetResponseBit(commandPackage->msg_id);
}

void StopHackedTimer(USBReceiveData const * const commandPackage)
{
    // Turn off Test mode interrupt of MCU
    DisableTimerInterrupt(HACKED_TIMER);

    // Prepare acknowledge message
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send acknowledge message to GUI
    SendUSBPackage(&packageToSend);
}

void IntCmdExecuteHackedTimer(void)
{
    // Parameters for SPI packages and variable to store output data
    USBTransmitData packageToSend;
    SPIReceiveDataNormal dataReceived;
    boolean isSuccessfulFlag = FALSE;
    uint8 spiChannel;


    // Reset INIT mode timer
    QSPIWriteNormal(SPI1_CS1MASTER, COMMON_SYSSTATES_RESET_CONFIG, COMMON_SYSSTATES_RESET_CONFIG_SPI_CLEAR_IMT_MASK);

    // Read register with ASIC state
    isSuccessfulFlag = QSPIReadNormal(SPI1_CS1MASTER, COMMON_SYSTEM_STATE, &dataReceived.dw);

    // Construct package to PC
    packageToSend.device_id = SPI1_CS1MASTER;
    packageToSend.msg_id = g_msgID;

    // Construct packages based on error status
    if (isSuccessfulFlag == FALSE)
    {
        // Common error frame setup
        packageToSend.status = USB_STATUS_ERROR;

        // Fill data in error frame with invalid response from ASIC
        packageToSend.dataLength = 4;
        packageToSend.data[0] = GetByteByIdx(0, dataReceived.dw);
        packageToSend.data[1] = GetByteByIdx(1, dataReceived.dw);
        packageToSend.data[2] = GetByteByIdx(2, dataReceived.dw);
        packageToSend.data[3] = GetByteByIdx(3, dataReceived.dw);
    }
    else
    {
        // Store SPI response frame to temporary variable for extracting data
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 2;
        packageToSend.data[0] = GetByteByIdx(0, dataReceived.bf.output_data);
        packageToSend.data[1] = GetByteByIdx(1, dataReceived.bf.output_data);
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}
