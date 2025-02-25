/**********************************************************************************************************************
 * \file usb.c
 * \copyright Copyright (C) RobertBosch GmbH
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "uart.h"
#include "IfxAsclin_Asc.h"
#include "IfxCpu_Irq.h"
#include "common/Ifx_IntPrioDef.h"
#include "Bsp.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/
/* Communication parameters */
#define UART_BAUDRATE         2000000                            /** \brief Baud rate of the UART interface in bit/s */

#define UART_PIN_TX           IfxAsclin7_TX_P23_3_OUT            /** \brief TX pin of the UART interface             */

#define UART_TX_BUFFER_SIZE      12                              /** \brief Definition of the buffer size in byte    */

#define UART_TIMEOUT             5                               /** \brief UART transmit busywait timeout           */
/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/
static IfxAsclin_Asc g_uart;                                                 /** \brief UART handle                  */

/** The FIFO buffer for UART transmission should have 8 bytes more for proper operation */
static uint8 g_uartTxBuffer[UART_TX_BUFFER_SIZE + sizeof(Ifx_Fifo) + 8];     /** \brief FIFOs parameters             */

static sint32 g_uart_timeout;                                               /** \brief UART transmit timeout in tics */
/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/


/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/
IFX_INTERRUPT(uartTxISR, 0, ISR_PRIORITY_UART_TX);

void uartTxISR(void)
{
    IfxAsclin_Asc_isrTransmit(&g_uart);
}

void InitUart(void)
{
    /* Initialize an instance of IfxAsclin_Asc_Config with default values */
    IfxAsclin_Asc_Config uartConfig;
    IfxAsclin_Asc_initModuleConfig(&uartConfig, UART_PIN_TX.module);

    /* Set the desired baud rate */
    uartConfig.baudrate.prescaler = 1;
    uartConfig.baudrate.baudrate = UART_BAUDRATE;

    /* ISR priorities and interrupt target */
    uartConfig.interrupt.txPriority = ISR_PRIORITY_UART_TX;
    uartConfig.interrupt.typeOfService = IfxSrc_Tos_cpu0;

    /* FIFO configuration */
    uartConfig.txBuffer = &g_uartTxBuffer;
    uartConfig.txBufferSize = UART_TX_BUFFER_SIZE;

    /* Port pins configuration */
    const IfxAsclin_Asc_Pins pins = {
        .cts        = NULL_PTR,                         /* CTS pin not used                                     */
        .ctsMode    = IfxPort_InputMode_pullUp,
        .rx         = NULL_PTR,
        .rxMode     = IfxPort_InputMode_pullUp,
        .rts        = NULL_PTR,                         /* RTS pin not used                                     */
        .rtsMode    = IfxPort_OutputMode_pushPull,
        .tx         = &UART_PIN_TX,
        .txMode     = IfxPort_OutputMode_pushPull,
        .pinDriver  = IfxPort_PadDriver_cmosAutomotiveSpeed1
    };
    uartConfig.pins = &pins;

    uartConfig.frame.parityBit = TRUE;
    uartConfig.frame.parityType = IfxAsclin_ParityType_odd;

    IfxAsclin_Asc_initModule(&g_uart, &uartConfig);                      /* Initialize module with above parameters  */

    g_uart_timeout = IfxStm_getTicksFromMilliseconds(BSP_DEFAULT_TIMER, UART_TIMEOUT);
}

void SendUartData(const uint8 * data, Ifx_SizeT count)
{
    //IfxAsclin_Asc_write(&g_uart, data, &count, g_uart_timeout);

    Ifx_SizeT size = 1;
    volatile int x = 0;

    for (Ifx_SizeT i = 0; i < count; i++)
    {
        IfxAsclin_Asc_write(&g_uart, &data[i], &size, g_uart_timeout);

        // TODO: Temporary Approach, fix in future with correct intermediate frame time
        for (uint32 j = 0; j < 240; j++) {
            x++;
        }
    }
}
