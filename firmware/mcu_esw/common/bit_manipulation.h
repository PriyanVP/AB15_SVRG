/**********************************************************************************************************************
 * \file bit_manipulation.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef BIT_MANIPULATION_H_
#define BIT_MANIPULATION_H_

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

/** \brief Construct word from 2 bytes
 *
 * \param msb MSB byte of the word
 * \param lsb LSB byte of the word
 * \return Constructed word
 */
uint16 ConstructWordFromBytes(uint8 msb, uint8 lsb);

/** \brief Get MSB byte of the word
 *
 * \param word 16 bit word
 * \return MSB byte of the 16 bit word
 */
uint8 GetMSB(uint16 word);

/** \brief Get LSB byte of the word
 *
 * \param word 16 bit word
 * \return LSB byte of the 16 bit word
 */
uint8 GetLSB(uint16 word);

/** \brief Get byte from 32bit word by index
 *
 * \param byteIdx byte index in word (LSB - 0, MSB - 3)
 * \param word 32 bit word
 * \return Requested byte. If index of of range - 0
 */
uint8 GetByteByIdx(uint8 byteIdx, uint32 word);

/** \brief Set response bit (8th) in message ID field
 *
 * \param msgID message ID
 * \return Message ID with response bit set
 */
uint8 SetResponseBit(uint8 msgID);

#endif /* BIT_MANIPULATION_H_ */
