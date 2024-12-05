/**********************************************************************************************************************
 * \file crc_wrapper.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef CRC_WRAPPER_H_
#define CRC_WRAPPER_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/spi_data_types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Checkes if CRC8 of data matches expected value
 *
 * \param p_data pointer to buffer with data to calculate CRC
 * \param length data length in bytes
 * \param expectedCrc expected CRC value
 * \return Returns TRUE if CRC matches expected, otherwise FALSE
 */
boolean IsCRC8Correct(uint8 * const p_data, uint16 length, uint8 expectedCrc);

/** \brief Checkes if CRC3 of SPI receive (MISO) data matches expected value
 *
 * \param p_data pointer to received SPI package
 * \param expectedCrc expected CRC value
 * \return Returns TRUE if CRC matches expected, otherwise FALSE
 */
boolean IsCRC3Correct(uint32 * const p_data, uint8 expectedCrc);

/** \brief Calculates CRC8 of input data
 *
 * \param p_data pointer to buffer with data to calculate CRC
 * \param length data length in bytes
 * \return Returns CRC for input data; if Null pointer used as input returns 0
 */
uint8 GetCRC8(uint8 * const p_data, uint16 length);

/** \brief Calculates CRC3 of SPI transmit frame
 *
 * \param p_data pointer to package that will be transmitted via SPI
 * \return Returns CRC for input data; if Null pointer used as input returns 0
 */
uint8 GetCRC3(uint32 * const p_data);

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

#endif /* CRC_WRAPPER_H_ */
