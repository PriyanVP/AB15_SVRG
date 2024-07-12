/**********************************************************************************************************************
 * \file usb.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef USB_H_
#define USB_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "IfxAsclin_Asc.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Initializes the USB interface
 * After exiting this function USB communication is possible
 *
 * \return Returns nothing
 */
void InitUSBInterface(void);

/** \brief Send data via USB
 * Writes data to Tx buffer of UART. Sending via USB is done automatically
 *
 * \param buffer pointer to buffer with data to send
 * \param count pointer to number of elements to send
 * \return Returns TRUE if data could be written
 */
boolean SendUSBData(uint8 *buffer, Ifx_SizeT *count);

/** \brief Receive data via USB
 *  Reads data from Rx buffer of UART. Recieving via USB is done automatically
 *
 * \param buffer pointer to buffer where data will be stored
 * \param count pointer to number of elements to read
 * \return Returns TRUE if data could be read
 */
boolean ReceiveUSBData(uint8 *buffer, Ifx_SizeT *count);

/** \brief Gets number of free bytes in UART write buffer (Tx)
 *
 * \return Returns number of free bytes
 */
sint32 GetWriteBufferLeft();

/** \brief Gets number of free bytes in UART read buffer (Rx)
 *
 * \return Returns number of free bytes
 */
sint32 GetReadBufferLeft();

/** \brief Checks if no full messages in UART read buffer (Rx) present
 *
 * \return Returns TRUE if no full message in input buffer
 */
boolean IsInBufferEmpty();

/** \brief Clears UART input (Rx) buffer used for USB communication
 *
 * \return Returns nothing
 */
void CleanUSBInputBuffer();

/** \brief Clears UART output (Tx) buffer used for USB communication
 *
 * \return Returns nothing
 */
void CleanUSBOutputBuffer();

#endif /* USB_H_ */
