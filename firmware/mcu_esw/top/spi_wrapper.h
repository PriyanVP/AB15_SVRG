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
#include "common/global_defines.h"
#include "common/spi_data_types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

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

// AB12 prototypes

/** \brief Execute SPI instruction for AB12 via QSPI
 *
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param instruction absolute address to read from
 * \param programmingEnable boolean flag to enable ASIC programming
 * \param dataToSend data that should be send as SPI input data (look for correct format for each instruction individually)
 * \param p_data empty variable (as pointer) to store readout data
 * \return Returns TRUE if no errors in received data, FALSE - otherwise
 */
boolean QSPIExecuteInstruction(uint8 spiChannel, AB12SPIInstructionsEnum instruction, boolean programmingEnable, uint16 dataToSend, uint32 * const p_data);

// AB15 prototypes

/** \brief Reads sensor data via QSPI
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param address absolute address of sensor
 * \param p_data pointer to empty variable to store readout data
 * \return Returns TRUE if no errors in received data, FALSE - otherwise
 */
boolean QSPIReadSensor(uint8 spiChannel, uint16 address, uint32 * const p_data);

/** \brief Read sequentially sensors data via QSPI
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param p_addressBuffer pointer to array with register addresses
 * \param p_dataBuffer pointer to empty (0s) array, after execution contains responses from sensor (full SPI frames)
 * \param p_length pointer to variable storing length of input buffers, after execution stores length of p_dataBuffer
 * \return Returns TRUE is there were no errors during operation, FALSE otherwise
 */
boolean QSPIReadSequenceSensor(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, uint16 * const p_length);

/** \brief Reads data from ASIC via QSPI
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param address absolute address to read from
 * \param p_data pointer to empty variable to store readout data
 * \return Returns TRUE if no errors in received data, FALSE - otherwise
 */
boolean QSPIReadNormal(uint8 spiChannel, uint16 address, uint32 * const p_data);

/** \brief Writes data via QSPI to ASIC
 * Created for fast command sending. Not safe, no guarantee of receiving package from slave or error handling
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param address absolute address to write
 * \param data data to write
 * \return Returns TRUE if no errors in received data, FALSE - otherwise
 */
boolean QSPIWriteNormal(SpiChSlaveSelectEnum spiChannel, uint16 address, uint16 data);

/** \brief Writes 32bit raw data  via QSPI to SPI device
 * Created for raw command sending.
 *
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param p_data 32bit SPI frame; is also used for sending back raw SPI receive frame
 * \return Returns TRUE
 */
boolean QSPIExecuteRawTransaction(uint8 spiChannel, uint32 * const p_data);

/** \brief Read sequentially data from ASIC via QSPI
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param p_addressBuffer pointer to array with register addresses
 * \param p_dataBuffer pointer to empty (0s) array, after execution contains responses from ASIC (full SPI frames)
 * \param p_length pointer to variable storing length of input buffers, after execution stores length of p_dataBuffer
 * \return Returns TRUE is there were no errors during operation, FALSE otherwise
 */
boolean QSPIReadSequenceNormal(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, uint16 * const p_length);


/** \brief Write sequentially data to ASIC via QSPI
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param p_addressBuffer pointer to array with register addresses
 * \param p_dataBuffer pointer to array with register content (aligned by LSB), after execution contains responses from ASIC (full SPI frames)
 * \param p_length pointer to variable storing length of input buffers, after execution stores length of p_dataBuffer
 * \return Returns TRUE is there were no errors during operation, FALSE otherwise
 */
boolean QSPIWriteSequenceNormal(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, uint16 * const p_length);

/** \brief Reads/writes data via QSPI to ASIC in sequence by specified addresses 
 * Created for executing read+write sequences with minimum overhead and with communication error checking
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param p_addressBuffer pointer to array with register addresses
 * \param p_dataBuffer pointer to array with data that should be written to registers (0s for read commands), after execution contains responses from ASIC
 * \param p_rwBuffer pointer to array with flags indicating if read or write operation should be executed
 * \param p_length pointer to variable storing length of input buffers, after execution stores length of p_dataBuffer
 * \return Returns TRUE is there were no errors during operation, FALSE otherwise
 */
boolean QSPIReadWriteSequenceNormal(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, 
                                    const RWFlagEnum * const p_rwBuffer, uint16 * const p_length);

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

#endif /* SPI_WRAPPER_H_ */
