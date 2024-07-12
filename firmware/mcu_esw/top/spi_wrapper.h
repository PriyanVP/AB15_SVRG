/**********************************************************************************************************************
 * \file spi_wrapper.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef SPI_WRAPPER_H_
#define SPI_WRAPPER_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Ifx_Types.h"
#include "../common/spi_data_types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Enum for read/write bit in SPI package
 */
enum RWFlag {
    READ = 0,
    WRITE = 1
};

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief This function wraps low level QSPI periphery init functions
 *
 *  \return Returns nothing.
 */
void QSPIInit(void);

/** \brief This function wraps low level QSPI periphery deinit function
 *
 *  \return Returns nothing.
 */
void QSPIDeinit(void);

/** \brief Reads data via QSPI. Two SPI transaction (read request + dummy read to get response for request)
 * Created for single reading from specific address
 *
 * \param address absolute address to read from
 * \param data empty variable to store readout data
 * \return Returns TRUE if no errors in received data, FALSE - otherwise
 */
boolean QSPIReadFast(uint16 address, uint32 * const data);

/** \brief Writes data via QSPI without checking response. One SPI transaction
 * Created for fast command sending. Not safe, no guarantee of receiving package from slave or error handling
 *
 * \param address absolute address to write
 * \param data data to write
 * \return Returns nothing
 */
void QSPIWriteFast(uint16 address, uint16 data);

/** \brief Reads data via QSPI in sequence by specified addresses. Executes n + 1 transactions
 * Created for reading big chunks of data with minimum overhead and with communication error checking
 *
 * \param addressBuffer pointer array with absolute address to read from
 * \param dataBuffer empty buffer to store readout data from SPI (order matches order of addresses, no offset present)
 * \param length pointer variable storing number of SPI frames, after returning from function stores number of read frames
 * \return Returns TRUE if no errors occured, FALSE otherwise
 */
boolean QSPIReadSequence(const uint16 * const addressBuffer, uint32 * const dataBuffer, uint16 * const length);

/** \brief Writes data via QSPI in sequence by specified addresses. Executes n + 1 transactions
 * Created for writing big chunks of data with minimum overhead and with communication error checking
 *
 * \param addressBuffer pointer array with absolute address to write to
 * \param dataBuffer buffer with data to write (order matches order of addresses, no offset present)
 * \param length pointer variable storing number of SPI frames, after returning from function stores number of written frames
 * \return Returns TRUE if no errors occured, FALSE otherwise
 */
boolean QSPIWriteSequence(const uint16 * const addressBuffer, const uint16 * const dataBuffer, uint16 * const length);

/** \brief Reads/writes data via QSPI in sequence by specified addresses. Executes n + 1 transactions
 * Created for executing read+write sequences with minimum overhead and with communication error checking
 *
 * \param addressBuffer pointer array with absolute address to read from/write to
 * \param dataBuffer buffer with data to write (order matches order of addresses, no offset present); will contain output of read transactions after execution
 * \param length pointer variable storing number of SPI frames, after returning from function stores number of transmitted frames
 * \return Returns TRUE if no errors occured, FALSE otherwise
 */
boolean QSPIReadWriteSequence(const uint16 * const addressBuffer, uint32 * const dataBuffer, const enum RWFlag * const rwBuffer, uint16 * const length);

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

#endif /* SPI_WRAPPER_H_ */
