/**********************************************************************************************************************
 * \file gpio_cmd.c
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
#include "gpio_cmd.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static boolean g_GPIOPollingConfigured = FALSE;                     /** \brief Continuous read was configured        */

static USBReceiveData g_GPIOReadPackage;                            /** \brief Package to perform gpio read          */

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/
void CmdStartGPIOReading(USBReceiveData const * const commandPackage)
{
    // Create empty package
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);

    // Set main fields to default values
    g_GPIOReadPackage.command = _USB_CMD_MIN;
    g_GPIOReadPackage.msg_id = 0;
    g_GPIOReadPackage.dataLength = 0;

    // Check if possible to start
    if (GetStateGPIOInterrupt() == FALSE)
    {
        // Configure continuous read interrupt periodicity
        if (commandPackage->dataLength >= 4)
        {
            uint16 continuousReadPeriodicity = ConstructWordFromBytes(commandPackage->data[1], commandPackage->data[0]);
            ConfigureGPIOPeriodicity(continuousReadPeriodicity); // TODO: can be calculated on MCU side

            // Store configuration to command
            g_GPIOReadPackage.command = commandPackage->command;
            g_GPIOReadPackage.dataLength = commandPackage->dataLength - 2;
            for (uint8 i = 0; i < g_GPIOReadPackage.dataLength; i++)
            {
                g_GPIOReadPackage.data[i] = commandPackage->data[i + 2]; // 2 used because first 2 bytes used for periodicity configuration
            }

            // Set flag indicating finishing configuration of feature
            g_GPIOPollingConfigured = TRUE;

            // Report success
            packageToSend.status = USB_STATUS_ACK;
            packageToSend.dataLength = 0;

            // Report feature state
            SetGPIOState(FEATURE_CONFIGURED);

            // Enable continuous read irq generation
            EnableGPIOInterrupt();

            // Set msg_id for responses
            g_GPIOReadPackage.msg_id = commandPackage->msg_id;

            // Report feature state
            SetGPIOState(FEATURE_RUNNING);
        }
        else
        {
            // Clear flag indicating finishing configuration of feature
            g_GPIOPollingConfigured = FALSE;

            // Report error
            packageToSend.status = USB_STATUS_ERROR;
            packageToSend.dataLength = 0;

            // Report feature state
            SetGPIOState(FEATURE_DISABLED);
        }

        // Send response back to MCU
        SendUSBPackage(&packageToSend);
    }
}

void CmdStopGPIOReading(USBReceiveData const * const commandPackage)
{
    // Create empty package
    USBTransmitData packageToSend;

    // Clear flag indicating finishing configuration of feature
    g_GPIOPollingConfigured = FALSE;

    // Disable continuous read irq generation
    DisableGPIOInterrupt();

    // Send acknowlege back to MCU
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;
    SendUSBPackage(&packageToSend);

    // Report feature state
    SetGPIOState(FEATURE_DISABLED);
}

void IntCmdGPIORead(void)
{
    // Stop executing if not configured
    if (g_GPIOPollingConfigured == FALSE) return;

    // Call function to read sequence of registers (addresses to read set during feature configuration)
    // CmdExecuteReadSequence(&g_GPIOReadPackage); TODO: uncomment
}
