/**********************************************************************************************************************
 * \file command_queue.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef COMMAND_QUEUE_H_
#define COMMAND_QUEUE_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "usb_data_types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define QUEUE_SIZE  (17)                  /** \brief Queue size in frames (1 is unused due to queue implementation)  */

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Structure for command queue (FIFO)
 */
typedef struct
{
    uint8 head;                         /** \brief data array index to add new record */
    uint8 tail;                         /** \brief data array index to read record */
    uint8 size;                         /** \brief data size (allocated, not occupied) */
    USBReceiveData data[QUEUE_SIZE];    /** \brief array with records of queue*/
} queue_t;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Initialize queue variable with default data
 */
void QueueInit(void);

/** \brief Read item from queue tail (first out) and remove it from queue
 *
 * \return Item from queue tail
 */
USBReceiveData QueueRead(void);

/** \brief Write item to queue head (first in)
 *
 * \param handle data to enqueue
 * \return True if possible to enqueue item, false if queue is full
 */
boolean QueueWrite(USBReceiveData const * const handle);

/** \brief Write item to queue tail (first out)
 * Required for fast response to command - ex. watchdog ack
 * (command will be executed immediately after current one finished)
 *
 * \param handle data to enqueue
 * \return True if possible to enqueue item, false if queue is full
 */
boolean QueueWriteTail(USBReceiveData const * const handle);

/** \brief Get occupied queue length
 *
 * \return Number of items in queue
 */
uint8 QueueGetOccupiedSize(void);

/** \brief Remove all items from queue
 */
void QueueClear(void);

#endif /* COMMAND_QUEUE_H_ */
