/**********************************************************************************************************************
 * \file crc.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef CRC_H_
#define CRC_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Calculate CRC (CRC-8-CCITT, -ITU)
 * Polynom: x^8+x^2+x+1
 *
 * \param buffer pointer to buffer with data to calculate CRC
 * \param length number of bytes to use for calculation
 * \return Returns CRC
 */
uint8 CRC8(uint8 * const buffer, uint16 length);

/** \brief Calculate CRC (CRC-5-USB)
 * Polynom: x^5+x^2+x+1
 *
 * \param buffer buffer with data to calculate CRC
 * \param length number of bits to use for calculation
 * \return Returns CRC
 */
uint8 CRC5(const uint32 buffer);

/** \brief Calculate CRC for SPI (CRC3)
 * Polynom: x^3 + x + 1 
 *
 * \param buffer buffer with data to calculate CRC
 * \param startBitPosition index of first LSB relevant for CRC bit (0 based)
 * \param endBitPosition index of last MSB relevant for CRC bit (0 based)
 * \return Returns CRC
 */
uint8 CRC3(const uint32 buffer, const uint8 startBitPosition, const uint8 endBitPosition);

#endif /* CRC_H_ */
