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

/** \brief Calculate CRC for master output () TODO: needs refactoring. Notes in .c file
 * Polynom: x^3 + x + 1 
 *
 * \param buffer buffer with data to calculate CRC
 * \param length number of bits to use for calculation
 * \return Returns CRC
 */
uint8 CRC3MO(const uint32 buffer);

/** \brief Calculate CRC for master input () TODO: needs refactoring. Notes in .c file
 * Polynom: x^3 + x + 1 
 *
 * \param buffer buffer with data to calculate CRC
 * \param length number of bits to use for calculation
 * \return Returns CRC
 */
uint8 CRC3MI(const uint32 buffer);


#endif /* CRC_H_ */
