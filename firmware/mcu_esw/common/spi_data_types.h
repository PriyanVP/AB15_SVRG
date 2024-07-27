/**********************************************************************************************************************
 * \file spi_data_types.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef SPI_DATA_TYPES_H_
#define SPI_DATA_TYPES_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Ifx_Types.h"
#include "common/global_defines.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Enum for read/write bit in SPI package
 */
enum RWFlag {
    READ = 0,
    WRITE = 1
};

/** \brief Defines applicable instructions for SPI communication with AB12
 */
typedef enum
{
    /* Module SPI */
    READ_DEV_ID             = 0b0000000000,             /** \brief read device ID */
    READ_REV_ID             = 0b0000000001,             /** \brief read revision ID */
    READ_MASK_ID            = 0b0000000010,             /** \brief read mask ID */
    EOP                     = 0b0000000110,             /** \brief end of programming */

    /* Module WD */
    WD2_TRIGGER             = 0b0000000100,             /** \brief SPI instruction WD2_TRIGGER              */
    WD3_TRIGGER             = 0b0000000101,             /** \brief SPI instruction WD3_TRIGGER              */
    WD_STATUS               = 0b0000000111,             /** \brief SPI instruction WD_STATUS                */

    /* Module ... */



    /* Module TEST */
    TEST_FLM                = 0b0101101100              /** \brief Test FLM                                 */
} AB12SPIInstructionsEnum;

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

#ifndef AB12_PLATFORM
// AB15 SPI data types

/** \brief Structure for transmit SPI data (normal data frame)
 */
typedef union
{
    struct
    {
        uint32 unused     : 2;    /** \brief unused bits */
        uint32 crc        : 3;    /** \brief package checksum */
        uint32 data       : 16;   /** \brief data being written into register */
        uint32 rw         : 1;    /** \brief transfer direction: 0 - read, 1 - write */
        uint32 address    : 9;    /** \brief register address */
        uint32 sensor_data: 1;    /** \brief flag indicating if sensor data is send (0 for normal frame) */
    } bf;
    uint32 dw;
} SPITransmitDataNormal;

/** \brief Structure for received SPI data (normal data frame)
 */
typedef union
{
    struct
    {
        uint32 crc              : 3;  /** \brief package checksum */
        uint32 asic_error_flag  : 1;  /** \brief ASIC error flag (oscillator or APB read access time error) */
        uint32 output_data      : 16; /** \brief data read from register (0s for write) */
        uint32 as_flags         : 5;  /** \brief additional status flags */
        uint32 sensor_data      : 1;  /** \brief flag indicating if sensor data is send (0 for normal frame) */
        uint32 gs0              : 1;  /** \brief APB Access Time Error */
        uint32 gs1              : 1;  /** \brief ADC error */
        uint32 gs2              : 1;  /** \brief APB Bus Transaction Error */
        uint32 gs3              : 1;  /** \brief End Of Programming */
        uint32 gs4              : 1;  /** \brief Test Active Flag */
        uint32 gs5              : 1;  /** \brief Transfer Failure Flag */
    } bf;
    uint32 dw;
} SPIReceiveDataNormal;

/** \brief Structure for transmit SPI data (sensor data frame)
 */
typedef union
{
    struct
    {
        uint32 unused     : 2;    /** \brief unused bits */
        uint32 crc        : 3;    /** \brief package checksum */
        uint32 dont_care  : 21;   /** \brief don't care bits */
        uint32 address    : 5;    /** \brief sensor address (A8...A4) */
        uint32 sensor_data: 1;    /** \brief flag indicating if sensor data is send (1 for sensor frame) */
    } bf;
    uint32 dw;
} SPITransmitDataSensor;

/** \brief Structure for received SPI data (sensor data frame)
 */
typedef union
{
    struct
    {
        uint32 crc              : 3;  /** \brief package checksum */
        uint32 asic_error_flag  : 1;  /** \brief ASIC or sensor error flag (oscillator or APB read access time error) */
        uint32 output_data      : 16; /** \brief data read from register (0s for write) */
        uint32 sid              : 5;  /** \brief PSI Sensor ID */
        uint32 sensor_data      : 1;  /** \brief flag indicating if sensor data is send (0 for normal frame) */
        uint32 gs0              : 1;  /** \brief APB Access Time Error */
        uint32 gs1              : 1;  /** \brief ADC error */
        uint32 gs2              : 1;  /** \brief APB Bus Transaction Error */
        uint32 gs3              : 1;  /** \brief End Of Programming */
        uint32 gs4              : 1;  /** \brief Test Active Flag */
        uint32 gs5              : 1;  /** \brief Transfer Failure Flag */
    } bf;
    uint32 dw;
} SPIReceiveDataSensor;

#else
// AB12 SPI data types

/** \brief Structure for transmit SPI data
 */
typedef union
{
    struct
    {
        uint32 unused       : 2;   /** \brief unused bit*/
        uint32 crc          : 3;   /** \brief crc3 */
        uint32 data         : 16;  /** \brief input data  */
        uint32 pe           : 1;   /** \brief programming enable flag */
        uint32 instruction  : 10;  /** \brief spi instruction */
    } bf;
    uint32 dw;
} SPITransmitData;

/** \brief Structure for received SPI data
 */
typedef union
{
    struct
    {
        uint32 crc              : 3;  /** \brief package checksum */
        uint32 gs_flag          : 1;  /** \brief global status flag */
        uint32 output_data      : 16; /** \brief receive data */
        uint32 sid_add_status   : 5;  /** \brief safety ID */
        uint32 s_bit            : 1;  /** \brief sebsor data flag */
        uint32 dis1             : 1;  /** \brief Disposal flag 1 */
        uint32 dis2             : 1;  /** \brief Disposal flag 2 */
        uint32 wdf              : 1;  /** \brief Watchdog fault */
        uint32 eop              : 1;  /** \brief End Of Programming */
        uint32 tst              : 1;  /** \brief Test Active Flag */
        uint32 tff              : 1;  /** \brief Transfer Failure Flag */
    } bf;
    uint32 dw;
} SPIReceiveData;

#endif

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

#endif /* SPI_DATA_TYPES_H_ */
