/**********************************************************************************************************************
 * \file cont_read_cmd.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/usb_data_types.h"
#include "common/bit_manipulation.h"
#include "periphery/timer.h"
#include "top/status.h"
#include "top/usb_wrapper.h"

#include "seq_cmd.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static boolean g_continuousReadConfigured = FALSE;                  /** \brief Continuous read was configured        */

static USBReceiveData g_continuousReadPackage;                      /** \brief Package to perform continuous read    */

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdConfigureReading(USBReceiveData const * const commandPackage)
{
    // Create empty package
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // Set main fields to default values
    g_continuousReadPackage.command = _USB_CMD_MIN;
    g_continuousReadPackage.msg_id = 0;
    g_continuousReadPackage.dataLength = 0;

    // Configure continuous read interrupt periodicity
    if (commandPackage->dataLength >= 4)
    {
        uint16 continuousReadPeriodicity = ConstructWordFromBytes(commandPackage->data[1], commandPackage->data[0]);
        ConfigureContinuousReadPeriodicity(continuousReadPeriodicity); // TODO: can be calculated on MCU side

        // Store configuration to command
        g_continuousReadPackage.command = commandPackage->command;
        g_continuousReadPackage.dataLength = commandPackage->dataLength - 2;
        for (uint8 i = 0; i < g_continuousReadPackage.dataLength; i++)
        {
            g_continuousReadPackage.data[i] = commandPackage->data[i + 2]; // 2 used because first 2 bytes used for periodicity configuration
        }

        // Set flag indicating finishing configuration of feature
        g_continuousReadConfigured = TRUE;

        // Report success
        packageToSend.status = USB_STATUS_ACK;
        packageToSend.dataLength = 2;
        packageToSend.data[0] = 2;
        packageToSend.data[1] = 0;

        // Report feature state
        SetContinuousReadState(FEATURE_CONFIGURED);
    }
    else
    {
        // Clear flag indicating finishing configuration of feature
        g_continuousReadConfigured = FALSE;

        // Report error
        packageToSend.status = USB_STATUS_ERROR;
        packageToSend.dataLength = 0;

        // Report feature state
        SetContinuousReadState(FEATURE_DISABLED);
    }

    // Send response back to MCU
    SendUSBPackage(&packageToSend);
}

void CmdStartReading(USBReceiveData const * const commandPackage)
{
    // Check if possible to start
    if (g_continuousReadConfigured == TRUE && GetStateContinuousReadInterrupt() == FALSE)
    {
        // Enable continuous read irq generation
        EnableContinuousReadInterrupt();

        // Set msg_id for responses
        g_continuousReadPackage.msg_id = commandPackage->msg_id;

        // Report feature state
        SetContinuousReadState(FEATURE_RUNNING);
    }
}

void CmdStopReading(USBReceiveData const * const commandPackage)
{
    // Create empty package
    USBTransmitData packageToSend;

    // Clear flag indicating finishing configuration of feature
    g_continuousReadConfigured = FALSE;

    // Disable continuous read irq generation
    DisableContinuousReadInterrupt();

    // Send acknowlege back to MCU
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 2;
    packageToSend.data[0] = 1;
    packageToSend.data[1] = 0;
    SendUSBPackage(&packageToSend);

    // Report feature state
    SetContinuousReadState(FEATURE_DISABLED);
}

void IntCmdContRead(void)
{
    // Stop executing if not configured
    if (g_continuousReadConfigured == FALSE) return;

    // Call function to read sequence of registers (addresses to read set during feature configuration)
    // CmdExecuteReadSequence(&g_continuousReadPackage); // TODO: uncomment
}