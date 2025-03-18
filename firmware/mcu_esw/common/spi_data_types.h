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
typedef enum 
{
    READ = 0,
    WRITE = 1
} RWFlagEnum;

/** \brief SPI Slave Device ID to SPI bus and chip select mapping
 */
typedef enum
{
    // SPI1_CS_INVALID     = 0,                  /** \brief  no chip select    */
    SPI_CH_INVALID      = 0,                  /** \brief  no chip select    */
    SPI1_CS1MASTER      = 1,
    SPI1_CS_MON1         = 2, // master 1 + cs mon 1
    SPI1_CS1_SENSOR1    = 3,
    SPI1_CS1_SENSOR2    = 4,
    SPI1_CS1_SENSOR3    = 5,
    /*SPI2*/
    SPI2_CS2_SENSOR1    = 6,
    SPI2_CS_MON2        = 7, // master 1 + cs mon 2
    SPI2_CS2_SENSOR2    = 8,
    SPI2_CS2_SLAVE1     = 9,
    SPI2_CS2_SLAVE2     = 10,
    SPI2_CS2_SLAVE3     = 11,
    SPI_CH_ENUM_LAST    = 12                  /** \brief  enum Last    */
} SpiChSlaveSelectEnum;

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

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
        uint32 address    : 10;   /** \brief register address */
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
        uint32 address    : 6;    /** \brief sensor address (A8...A4) */
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

/** \brief Structure for transmit SPI data (raw data frame)
 */
typedef union
{
    struct
    {
        uint32 lsb        : 8;    /** \brief  least significant byte */
        uint32 byte1      : 8;    /** \brief  byte with index 1 */
        uint32 byte2      : 8;    /** \brief  byte with index 2 */
        uint32 msb        : 8;    /** \brief  most significant byte */
    } bf;
    uint32 dw;
} SPITransmitDataRaw;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

#endif /* SPI_DATA_TYPES_H_ */
