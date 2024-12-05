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

/** \brief Defines applicable instructions for SPI communication with AB12
 */
typedef enum
{
    /* Module SPI */
    READ_DEV_ID             = 0x0000,             /** \brief read device ID                           */
    READ_REV_ID             = 0x0001,             /** \brief read revision ID                         */
    READ_MASK_ID            = 0x0002,             /** \brief read mask ID                             */
    EOP                     = 0x0006,             /** \brief end of programming                       */

    /* Module WD */
    WD2_TRIGGER             = 0x0004,             /** \brief SPI instruction WD2_TRIGGER              */
    WD3_TRIGGER             = 0x0005,             /** \brief SPI instruction WD3_TRIGGER              */
    WD_STATUS               = 0x0007,             /** \brief SPI instruction WD_STATUS                */

    /* Module ... */



    /* Module TEST */
    TEST_FLM                = 0x016c              /** \brief Test FLM                                 */
} AB12SPIInstructionsEnum;


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
        uint32 s_bit            : 1;  /** \brief sensor data flag */
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
