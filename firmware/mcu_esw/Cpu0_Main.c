/**********************************************************************************************************************
 * \file Cpu0_Main.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/
#include "Ifx_Types.h"
#include "IfxCpu.h"
#include "IfxScuWdt.h"
#include "common/usb_data_types.h"
#include "common/spi_data_types.h"
#include "common/command_queue.h"
#include "command_queue.h"
#include "cmd/general_cmd.h"
#include "cmd/seq_cmd.h"
#include "cmd/bypass_cmd.h"
#include "cmd/cont_read_cmd.h"
#include "cmd/gpio_cmd.h"
#include "cmd/watchdog.h"
#include "periphery/led.h"
#include "periphery/usb.h"
#include "periphery/timer.h"
#include "periphery/control_pins.h"
#include "periphery/gpio.h"
#include "top/status.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "led.h"
#include "Bsp.h"

// for debug messages on button press JS 4/2024
#include "button.h"

IFX_ALIGN(4) IfxCpu_syncEvent g_cpuSyncEvent = 0;

#define WAIT_TIME 2   /*mseconds */

/** \brief Watchdog interrupt routine
 * Arms single acknowledgement of ASIC watchdog
 */
void WatchdogInterruptRoutine(void)
{
    // Watchdog serving is an internal command
    USBReceiveData serveWatchdogCommand;
    serveWatchdogCommand.command = INT_CMD_ACK_WATCHDOG;
    serveWatchdogCommand.dataLength = 0;
    // Add WD serving internal command to command queue
    //QueueWriteTail(&serveWatchdogCommand);    // TODO: commented out to have MCU contained WD routine. Uncomment for actual communication with ASIC
    ToggleLED2();
}

//void FastInterruptRoutine(void)
//{
//    // fast timer
//    ToggleLED4();
//}

/** \brief Main function
 */
void core0_main(void)
{
    IfxCpu_enableInterrupts();
    
    /* !!WATCHDOG0 AND SAFETY WATCHDOG ARE DISABLED HERE!!
     * Enable the watchdogs and service them periodically if it is required
     */
    IfxScuWdt_disableCpuWatchdog(IfxScuWdt_getCpuWatchdogPassword());
    IfxScuWdt_disableSafetyWatchdog(IfxScuWdt_getSafetyWatchdogPassword());
    
    /* Wait for CPU sync event */
    IfxCpu_emitEvent(&g_cpuSyncEvent);
    IfxCpu_waitEvent(&g_cpuSyncEvent, 1);
    
    /* Initialize the LED port pins      */
    InitLEDs();


    /* Init pins*/
    ConfigureSelectPin();
    InitGPIOPins();

    // Init USB
    USBInit();

    /* Initialize the QSPI modules and the LED */
    QSPIInit();

    // Init message pump
    QueueInit();

    // Init status
    InitStatus();

    // Init timer module
    InitGpt12Timer();

    // Start general timer
    StartGeneralTimer();


    // Local temporary variable for receiving data
    USBReceiveData receivedPackage;

    // Local variable to store dispatched command
    USBReceiveData cmdPackage;

    // Flag for USB receive status
    boolean isSuccessfulFlag = FALSE;
    while(1)
    {

        //ToggleLED1();
        Blink_LED1_1Hz();

       if(GetButtonState())
       {
           //OnLED2();


       }
       else
       {
          // OffLED2();
       }

        // wait for 2ms for next polling
        waitTime(IfxStm_getTicksFromMilliseconds(BSP_DEFAULT_TIMER, WAIT_TIME));

        // Receive package (assumes it already in in buffer of UART
        isSuccessfulFlag = ReceiveUSBPackage(&receivedPackage);

        // Proceed with execution if message received or queue is not empty
        if (isSuccessfulFlag == TRUE)
        {
            // Store data to queue and proceed
            QueueWrite(&receivedPackage);
            /* package sucessul received*/
            ToggleLED3();
            // Clean package
            receivedPackage.command = _USB_CMD_MIN;
        }
        else if (QueueGetOccupiedSize() == 0)
        {
            // Queue is not empty - proceed
            continue;
        }

        // Dispatch command
        // Also correctly handles empty queue dispatch

        cmdPackage = QueueRead(); //TODO: include not found

        // Command handling. Empty queue will be handled in max command case
        switch (cmdPackage.command)
        {
            case USB_CMD_IS_ALIVE:
                CmdIsAlive(&cmdPackage);
                break;
            case USB_CMD_SPI_INSTRUCTION:
                //CmdSpiInstuction(&cmdPackage);
                handleCmdInstr(&cmdPackage);
                break;
            case USB_CMD_READ_DEV_ID:
                CmdGetDeviceId(&cmdPackage);
                break;
            // case :
            //     break;
            // case :
            //     break;
            // case :
            //     break;
            // case :
            //     break;
            case USB_CMD_GET_CONFIG_WATCHDOG:
                CmdGetConfigWatchdog(&cmdPackage);
                break;
            case USB_CMD_CONFIGURE_WATCHDOG:
                CmdConfigureWatchdog(&cmdPackage);
                break;
            case USB_CMD_START_WATCHDOG:
                CmdStartWatchdog(&cmdPackage);
                break;

           case USB_CMD_STOP_WATCHDOG:
                CmdStopWatchdog(&cmdPackage);
                break;

           case USB_CMD_GET_MCU_VERSION:
                CmdGetMcuVersion(&cmdPackage);
                break;

           case USB_CMD_GET_MCU_BUILD_DATE:
                CmdGetMcuBuildDate(&cmdPackage);
                break;

           case USB_CMD_GET_MCU_BUILD_TIME:
                CmdGetMcuBuildTime(&cmdPackage);
                break;



           case _USB_CMD_MAX:
                break;
            default :
                break;
        }
    }
}
