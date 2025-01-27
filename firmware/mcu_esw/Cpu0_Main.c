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
#include "cmd/flm_diagnostics.h"
#include "cmd/general_cmd.h"
#include "cmd/seq_cmd.h"
#include "cmd/watchdog.h"
#include "cmd/testmode_cmd.h"
#include "cmd/hacked_timer_cmd.h" // TODO: refactor
#include "periphery/led.h"
#include "periphery/usb.h"
#include "periphery/timer.h"
#include "periphery/control_pins.h"
#include "top/status.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"
#include "Bsp.h"
#include "periphery/pwm.h"
#include "periphery/spi.h"
#include "IfxPort.h" // for gpio.h
#include "IfxPort_PinMap.h" // for gpio.h // TODO: remove dependency to IFxPort stuff
#include "periphery/gpio.h" // for chip select lines


IFX_ALIGN(4) IfxCpu_syncEvent g_cpuSyncEvent = 0;

#define WAIT_TIME 25   /*useconds */

/** \brief Watchdog 1 interrupt routine
 * Arms single acknowledgement of ASIC watchdog
 */
void Watchdog1InterruptRoutine(void)
{
    // Watchdog serving is an internal command
    static USBReceiveData serveWatchdogCommand = 
    {
        .device_id = 1,
        .dataLength = 0,
        .command = INT_CMD_ACK_WATCHDOG1
    };
    ToggleLED4();
    // Add WD serving internal command to command queue
    QueueWriteTail(&serveWatchdogCommand);
}

/** \brief Watchdog 2 interrupt routine
 * Arms single acknowledgement of ASIC watchdog
 */
void Watchdog2InterruptRoutine(void)
{
    // Watchdog serving is an internal command
    static USBReceiveData serveWatchdogCommand = 
    {
        .device_id = 1,
        .dataLength = 0,
        .command = INT_CMD_ACK_WATCHDOG2
    };
    ToggleLED2();
    // Add WD serving internal command to command queue
    QueueWriteTail(&serveWatchdogCommand);
}

/** \brief Watchdogs status reading interrupt routine
 * Arms single reading of watchdogs satus registers
 */
void WatchdogStatusReadingInterruptRoutine(void)
{
    // Watchdog status reading is an internal command
    static USBReceiveData serveWatchdogStatusCommand = 
    {
        .device_id = 1,
        .dataLength = 0,
        .command = INT_CMD_READ_WD_STATUS
    };

    // Add WD serving internal command to command queue
    QueueWrite(&serveWatchdogStatusCommand);
}

/** \brief FLM module cyclic diagnostics interrupt routine
 * Starts cyclic diagnostics, stores results to be later sent to GUI
*/
void FLMDiagInterruptRoutine(void)
{
    // FLM Cyclic Diag iteration is an internal command
    static USBReceiveData serveFLMDiagCommand = 
    {
        .device_id = 1,
        .dataLength = 0,
        .command = INT_CMD_EXECUTE_FLM_DIAG
    };

    // Add Test mode internal command to command queue
    QueueWrite(&serveFLMDiagCommand);
}

/** \brief Test mode 1/2 check interrupt routine
 * Arms single check of Test mode 1/2 results + arm next test if needed
 */
void TestModeInterruptRoutine(void)
{
    // Test mode iteration is an internal command
    static USBReceiveData serveTestModeCommand = 
    {
        .device_id = 1,
        .dataLength = 0,
        .command = INT_CMD_EXECUTE_TEST_MODE
    };

    // Add Test mode internal command to command queue
    QueueWrite(&serveTestModeCommand);
}

void HackedTimerInterruptRoutine(void)
{
    // Test mode iteration is an internal command
    static USBReceiveData serveHackedTimerCommand = 
    {
        .device_id = 1,
        .dataLength = 0,
        .command = INT_CMD_EXECUTE_HACKED_TIMER
    };

    // Add Test mode internal command to command queue
    QueueWrite(&serveHackedTimerCommand);
}

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
    InitCSPins();

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

    /* set default Frequency for PWM generation */
    SetDefaultPWMFrequency();

    // start CCU6 module PWM generation */
    StartPWMGeneration();

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

        Blink_LED1_1Hz();


        // wait for 2ms for next polling // TODO: remove and check
        waitTime(IfxStm_getTicksFromMicroseconds(BSP_DEFAULT_TIMER, WAIT_TIME));

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
            /* Region: external commands */

            case USB_CMD_IS_ALIVE:
                CmdIsAlive(&cmdPackage);
                break;

            case USB_CMD_READ_DEV_ID:
                CmdGetDeviceId(&cmdPackage);
                break;

            case USB_CMD_READ_REG:
                CmdReadReg(&cmdPackage);
                break;

            case USB_CMD_WRITE_REG:
                CmdWriteReg(&cmdPackage);
                break;

            case USB_CMD_EXECUTE_RW_SEQUENCE:
                CmdExecuteRWSequence(&cmdPackage);
                break;

            case USB_CMD_EXECUTE_READ_SEQUENCE:
                CmdExecuteReadSequence(&cmdPackage);
                break;

            case USB_CMD_EXECUTE_WRITE_SEQUENCE:
                CmdExecuteWriteSequence(&cmdPackage);
                break;

            case USB_CMD_WRITE_RAW_DATA_SPI:
                CmdSendRawData(&cmdPackage);
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

           case USB_CMD_SET_EXT_OSC_2MHZ:
                CmdSetExtOsc2Mhz(&cmdPackage);
                break;

           case USB_CMD_SET_EXT_OSC_4MHZ:
                CmdSetExtOsc4Mhz(&cmdPackage);
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

            case USB_CMD_START_TEST_MODE1:
                CmdStartTestMode1(&cmdPackage);
                break;

            case USB_CMD_START_TEST_MODE2:
                CmdStartTestMode2(&cmdPackage);
                break;

            case USB_CMD_STOP_TEST_MODE12:
                CmdStopTestMode12(&cmdPackage);
                break;

            case USB_CMD_START_HACKED_TIMER:
                StartHackedTimer(&cmdPackage);
                break;

            case USB_CMD_STOP_HACKED_TIMER:
                StopHackedTimer(&cmdPackage);
                break;

            case USB_CMD_START_MONITORING_WATCHDOG:
                CmdStartMonitoringWatchdog(&cmdPackage);
                break;

            case USB_CMD_STOP_MONITORING_WATCHDOG:
                CmdStopMonitoringWatchdog(&cmdPackage);
                break;

            case USB_CMD_FLM_DIAG_ENABLE:
                CmdEnableFLMDiag(&cmdPackage);
                break;

            case USB_CMD_FLM_DIAG_DISABLE:
                CmdDisableFLMDiag(&cmdPackage);
                break;

            case USB_CMD_FLM_DIAG_READ_RESULTS:
                CmdReadFLMDiagResults(&cmdPackage);
                break;

            /* Region: internal commands */

            case INT_CMD_ACK_WATCHDOG1:
                IntCmdAcknowledgeWatchdog1();
                break;

            case INT_CMD_ACK_WATCHDOG2:
                IntCmdAcknowledgeWatchdog2();
                break;

            case INT_CMD_READ_WD_STATUS:
                IntCmdMonitorWatchdog();
                break;

            case INT_CMD_EXECUTE_TEST_MODE:
                IntCmdExecutePowerstageTest();
                break;

            case INT_CMD_EXECUTE_HACKED_TIMER:
                IntCmdExecuteHackedTimer();
                break;

            case INT_CMD_EXECUTE_FLM_DIAG:
                IntCmdExecuteFLMDiag();
                break;

           case _USB_CMD_MAX:
                break;

            default :
                break;
        }
    }
}
