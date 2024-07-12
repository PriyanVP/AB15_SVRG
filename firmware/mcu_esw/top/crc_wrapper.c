/**********************************************************************************************************************
 * \file crc_wrapper.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/usb_data_types.h"
#include "common/spi_data_types.h"
#include "common/crc.h"

#include "crc_wrapper.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

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