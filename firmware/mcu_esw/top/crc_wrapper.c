/**********************************************************************************************************************
 * \file crc_wrapper.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/global_defines.h"
#include "common/usb_data_types.h"
#include "common/spi_data_types.h"
#include "common/crc.h"

#include "crc_wrapper.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define CRC5_LENGTH             (3)                                                 /** \brief CRC 5 length in bits  */

#define CRC3_LENGTH             (3)                                                 /** \brief CRC 3 length in bits  */
#define CRC3_OFFSET             (2)                                                 /** \brief CRC 3 offset          */

#ifdef AB12_PLATFORM

/* Defines for AB12 */
#define CRC3_MO_START_IDX       (5)                                                 /** \brief CRC 3 MO start index  */
#define CRC3_MO_END_IDX         (31)                                                /** \brief CRC 3 MO end index    */
#define CRC3_MI_START_IDX       (3)                                                 /** \brief CRC 3 MI start index  */
#define CRC3_MI_END_IDX         (26)                                                /** \brief CRC 3 MI end index    */

#else

/* Defines for AB15 */
#define CRC3_MO_START_IDX       (5)                                                 /** \brief CRC 3 MO start index  */
#define CRC3_MO_END_IDX         (31)                                                /** \brief CRC 3 MO end index    */
#define CRC3_MI_START_IDX       (3)                                                 /** \brief CRC 3 MI start index  */
#define CRC3_MI_END_IDX         (31)                                                /** \brief CRC 3 MI end index    */

#endif

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

boolean IsCRC8Correct(uint8 * const data, uint16 length, uint8 expectedCrc)
{
    uint8 calculatedCRC = 0;
    boolean result = FALSE;
    if (data != NULL_PTR)
    {
        calculatedCRC = CRC8(data, length);
        result = (expectedCrc == calculatedCRC) ? (TRUE) : (FALSE);
    }

    return result;
}

boolean IsCRC5Correct(SPIReceiveData * const data)
{
    uint8 calculatedCRC = 0;
    boolean result = FALSE;
    uint8 expectedCrc = data->bf.crc;
    uint32 twentySevenBitData = (data->dw) >> CRC5_LENGTH; // get only data from package without CRC

    calculatedCRC = CRC5(twentySevenBitData);
    result = (expectedCrc == calculatedCRC) ? (TRUE) : (FALSE);

    return result;
}

boolean IsCRC3Correct(uint8 * const data, uint8 expectedCrc)
{
    uint8 calculatedCRC;
    boolean result;

    // Error check
    if (data == NULL_PTR) return FALSE;

    calculatedCRC = CRC3(*data, CRC3_MI_START_IDX, CRC3_MI_END_IDX);
    result = (expectedCrc == calculatedCRC) ? (TRUE) : (FALSE);

    return result;
}

uint8 GetCRC8(uint8 * const data, uint16 length)
{
    uint8 calculatedCRC = 0;
    if (data != NULL_PTR)
    {
        calculatedCRC = CRC8(data, length);
    }
    return calculatedCRC;
}

uint8 GetCRC5(SPITransmitData * const data)
{
    uint8 calculatedCRC = 0;
    uint32 twentySevenBitData;
    if (data != NULL_PTR)
    {
        twentySevenBitData = (data->dw) >> CRC5_LENGTH;
        calculatedCRC = CRC5(twentySevenBitData);
    }
    return calculatedCRC;
}

uint8 GetCRC3(uint32 * const data)
{
    uint8 calculatedCRC;

    // Error check
    if (data == NULL_PTR) return 0;

    calculatedCRC = CRC3(*data, CRC3_MO_START_IDX, CRC3_MO_END_IDX);
    return calculatedCRC;
}