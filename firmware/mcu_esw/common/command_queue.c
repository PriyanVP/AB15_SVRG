/**********************************************************************************************************************
 * \file command_queue.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
// #include <stdio.h> // for memset
#include "Ifx_Types.h"
#include "usb_data_types.h"
#include "command_queue.h"

#include "Bsp.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static queue_t g_msgQueue;                                                    /** \brief global message queue handle */

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

// TODO: add check if 90% queue occupied . generate error frame for MCU

void QueueInit()
{
    g_msgQueue.head = 0;
    g_msgQueue.tail = 0;
    g_msgQueue.size = QUEUE_SIZE;
}

USBReceiveData QueueRead()
{
    // Disable all interrupts
    boolean interruptsState = disableInterrupts();

    USBReceiveData emptyPackage = { 0, 0, 0 };
    USBReceiveData handle;
    handle.command = _USB_CMD_MAX;
    if (g_msgQueue.tail == g_msgQueue.head)
    {
        return handle;
    }
    handle = g_msgQueue.data[g_msgQueue.tail];
    g_msgQueue.data[g_msgQueue.tail] = emptyPackage;
    g_msgQueue.tail = (g_msgQueue.tail + 1) % g_msgQueue.size;

    // Reenable interrupts
    restoreInterrupts(interruptsState);

    return handle;
}

boolean QueueWrite(USBReceiveData const * const handle)
{
    // Disable all interrupts
    boolean interruptsState = disableInterrupts();

    if (((g_msgQueue.head + 1) % g_msgQueue.size) == g_msgQueue.tail)
    {
        return FALSE;
    }
    g_msgQueue.data[g_msgQueue.head] = *handle;
    g_msgQueue.head = (g_msgQueue.head + 1) % g_msgQueue.size;

    // Reenable interrupts
    restoreInterrupts(interruptsState);

    return TRUE;
}

boolean QueueWriteTail(USBReceiveData const * const handle)
{
    // Disable all interrupts
    boolean interruptsState = disableInterrupts();

    uint8 newTailPos = (g_msgQueue.tail == 0) ? (g_msgQueue.size - 1) : (g_msgQueue.tail - 1);
    if (g_msgQueue.head == newTailPos)
    {
        return FALSE;
    }
    g_msgQueue.data[newTailPos] = *handle;
    g_msgQueue.tail = newTailPos;

    // Reenable interrupts
    restoreInterrupts(interruptsState);

    return TRUE;
}

uint8 QueueGetOccupiedSize()
{
    // Disable all interrupts
    boolean interruptsState = disableInterrupts();

    uint8 occupiedSize;
    if (g_msgQueue.tail == g_msgQueue.head)
    {
        occupiedSize = 0;
    }
    else if (g_msgQueue.head > g_msgQueue.tail)
    {
        occupiedSize = g_msgQueue.head - g_msgQueue.tail;
    }
    else if (g_msgQueue.head < g_msgQueue.tail)
    {
        occupiedSize = (g_msgQueue.size - g_msgQueue.tail) + g_msgQueue.head;
    }

    // Reenable interrupts
    restoreInterrupts(interruptsState);

    return occupiedSize;
}

void QueueClear()
{
    // Disable all interrupts
    boolean interruptsState = disableInterrupts();

    g_msgQueue.head = 0;
    g_msgQueue.tail = 0;

    // Reenable interrupts
    restoreInterrupts(interruptsState);
}
