/**********************************************************************************************************************
 * \file spi_wrapper.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Ifx_Types.h"
#include "Ifx_Ssw_Compilers.h"
#include "IfxAsclin_Asc.h"
#include "common/global_defines.h"
#include "common/spi_data_types.h"
#include "periphery/spi.h"
#include "common/crc.h"
#include "top/crc_wrapper.h"
#include "top/spi_wrapper.h"
#include "top/usb_wrapper.h"


/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define SPI_TRANSACTION_LENGTH          (4)                /** \brief Length of one SPI transaction in bytes         */

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Defines applicable types of sequences for SPI sequence operations
 */
typedef enum
{
    READ_ONLY   = 0,             /** \brief sequence contains only read commands      */
    WRITE_ONLY  = 1,             /** \brief sequence contains only write commands     */
    COMBINATION = 2              /** \brief sequence contains read and write commands */
} SequenceTypeEnum;

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static boolean enSPICommunication = FALSE;                 /** \brief Flag indicating if SPI communication enabled   */

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Common implementation of sequence operation on SPI 
 * 
 * \param spiChannel SPI Slave to and from which the SPI instruction is executed
 * \param p_addressBuffer pointer to array with register addresses
 * \param p_dataBuffer pointer to array with data that should be written to registers, after execution contains responses from ASIC
 * \param p_rwBuffer pointer to array with flags indicating if read or write operation should be executed
 * \param SEQ_TYPE type of SPI sequence
 * \param p_length pointer to variable storing length of input buffers, after execution stores length of p_dataBuffer
 * \return Returns TRUE is there were no errors during operation, FALSE otherwise
 */
IFX_INLINE boolean QSPIReadWriteSequenceNormalInline(uint8 spiChannel, const uint16 * const p_addressBuffer, 
                                                     uint32 * const p_dataBuffer, const RWFlagEnum * const p_rwBuffer, 
                                                     const SequenceTypeEnum SEQ_TYPE, uint16 * const p_length);

// boolean QSPIReadWriteSequenceNormalInline(uint8 spiChannel, const uint16 * const p_addressBuffer, 
//                                                      uint32 * const p_dataBuffer, const RWFlagEnum * const p_rwBuffer, 
//                                                      const SequenceTypeEnum SEQ_TYPE, uint16 * const p_length);

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void QSPIInit(void)
{
    enSPICommunication = TRUE;
    QSPIInitPeriphery();
}

void QSPIDeinit(void)
{
    enSPICommunication = FALSE;
    QSPIDeinitPeriphery();
}

#ifdef AB12_PLATFORM
// AB12 implementations

boolean QSPIExecuteInstruction(uint8 spiChannel, AB12SPIInstructionsEnum instruction, boolean programmingEnable, uint16 dataToSend, uint32 * const p_data)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Configure SPI channel for communication and stop execution if unsuccussful
    SpiBusSelectEnum SpiBusNumber = QSPIUpdateChannelConfig(spiChannel);
    if (SpiBusNumber == SPI_BUS_INVALID) return FALSE;

    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    SPITransmitData dataToTransmit;
    SPIReceiveData dataToReceive;

    // Configure and execute read request
    dataToTransmit.dw = 0;
    dataToTransmit.bf.instruction = instruction;
    dataToTransmit.bf.pe = programmingEnable;
    dataToTransmit.bf.data = dataToSend;
    dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));

    QSPIExchangeData(SpiBusNum, &dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

    // Validating input
    isReceivedDataValid = IsCRC3Correct(&(dataToReceive.dw), dataToReceive.bf.crc);

    // Return data
    *p_data = dataToReceive.dw;

    return (isReceivedDataValid);
}

#else
// AB15 implementations

boolean QSPIReadNormal(uint8 spiChannel, uint16 address, uint32 * const p_data)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Configure SPI channel for communication and stop execution if unsuccussful
    SpiBusSelectEnum SpiBusNumber = QSPIUpdateChannelConfig(spiChannel);
    if (SpiBusNumber == SPI_BUS_INVALID) return FALSE;

    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    SPITransmitDataNormal dataToTransmit;
    SPIReceiveDataNormal dataToReceive;

    // Configure and execute read request
    dataToTransmit.dw = 0;
    dataToTransmit.bf.sensor_data = FALSE;
    dataToTransmit.bf.address = address;
    dataToTransmit.bf.rw = READ;
    dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
    QSPIExchangeData(SpiBusNumber, &dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

    // Validating input
    isReceivedDataValid = IsCRC3Correct(&(dataToReceive.dw), dataToReceive.bf.crc);
    isReceivedDataValid &= (dataToReceive.bf.asic_error_flag == FALSE);
    isReceivedDataValid &= (dataToReceive.bf.gs0 == FALSE);
    isReceivedDataValid &= (dataToReceive.bf.gs2 == FALSE);
    isReceivedDataValid &= (dataToReceive.bf.gs5 == FALSE);

    // Return data
    *p_data = dataToReceive.dw;

    return (isReceivedDataValid);
}

boolean QSPIWriteNormal(uint8 spiChannel, uint16 address, uint16 data) // TODO: 
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Configure SPI channel for communication and stop execution if unsuccessful
    SpiBusSelectEnum SpiBusNumber = QSPIUpdateChannelConfig(spiChannel);
    if (SpiBusNumber == SPI_BUS_INVALID) return FALSE;


    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    SPITransmitDataNormal dataToTransmit;
    SPIReceiveDataNormal dataToReceive; // data in this variable unused, provides place to store incoming data

    // Configure and execute write request
    dataToTransmit.dw  = 0;
    dataToTransmit.bf.sensor_data = FALSE;
    dataToTransmit.bf.address = address;
    dataToTransmit.bf.rw = WRITE;
    dataToTransmit.bf.data = data;
    dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
    QSPIExchangeData(SpiBusNumber, &dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

    // Validating input
    isReceivedDataValid = IsCRC3Correct(&(dataToReceive.dw), dataToReceive.bf.crc);
    isReceivedDataValid &= (dataToReceive.bf.asic_error_flag == FALSE);
    isReceivedDataValid &= (dataToReceive.bf.gs0 == FALSE);
    isReceivedDataValid &= (dataToReceive.bf.gs2 == FALSE);
    isReceivedDataValid &= (dataToReceive.bf.gs5 == FALSE);

    return isReceivedDataValid;
}

boolean QSPIReadSequenceNormal(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, uint16 * const p_length)
{
    return QSPIReadWriteSequenceNormalInline(spiChannel, p_addressBuffer, p_dataBuffer, NULL_PTR, READ_ONLY, p_length);
}

boolean QSPIWriteSequenceNormal(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, uint16 * const p_length)
{
    return QSPIReadWriteSequenceNormalInline(spiChannel, p_addressBuffer, p_dataBuffer, NULL_PTR, WRITE_ONLY, p_length);
}

boolean QSPIReadWriteSequenceNormal(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, const RWFlagEnum * const p_rwBuffer, uint16 * const p_length)
{
    return QSPIReadWriteSequenceNormalInline(spiChannel, p_addressBuffer, p_dataBuffer, p_rwBuffer, COMBINATION, p_length);
}

IFX_INLINE boolean QSPIReadWriteSequenceNormalInline(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, const RWFlagEnum * const p_rwBuffer, const SequenceTypeEnum SEQ_TYPE, uint16 * const p_length)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Configure SPI channel for communication and stop execution if unsuccessful
    SpiBusSelectEnum SpiBusNumber = QSPIUpdateChannelConfig(spiChannel);
    if (SpiBusNumber == SPI_BUS_INVALID) return FALSE;


    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    uint16 numberOfFrames = (*p_length);
    SPITransmitDataNormal dataToTransmit;
    SPIReceiveDataNormal dataToReceive;

    // Reset length variable to store number of received frames there
    *p_length = 0;

    // Null pointer check
    if ((p_addressBuffer == NULL_PTR) || (p_dataBuffer == NULL_PTR) || 
        (p_length == NULL_PTR) || ((SEQ_TYPE == COMBINATION) && (p_rwBuffer == NULL_PTR)))
    {
        return FALSE;
    }

    // Configure common parameters of SPI normal frame
    dataToTransmit.dw = 0;
    dataToTransmit.bf.sensor_data = FALSE;
    dataToTransmit.bf.rw = (SEQ_TYPE == READ_ONLY) ? (READ) : (WRITE);

    // Execute series of SPI transactions
    for (uint16 i = 0; i < numberOfFrames; i++)
    {
        // Reading/writing data from addresses in address buffer
        dataToTransmit.bf.address = p_addressBuffer[i];
        if (SEQ_TYPE == COMBINATION)
        {
            // Use rwBuffer only if mix of read and write transactions required
            dataToTransmit.bf.rw = p_rwBuffer[i];
        }
        dataToTransmit.bf.data = p_dataBuffer[i];
        dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));

        // Execute SPI transaction exchange
        QSPIExchangeData(SpiBusNumber, &dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

        // Validating input
        isReceivedDataValid = IsCRC3Correct(&(dataToReceive.dw), dataToReceive.bf.crc);
        isReceivedDataValid &= (dataToReceive.bf.asic_error_flag == FALSE);
        isReceivedDataValid &= (dataToReceive.bf.gs0 == FALSE);
        isReceivedDataValid &= (dataToReceive.bf.gs2 == FALSE);
        isReceivedDataValid &= (dataToReceive.bf.gs5 == FALSE);

        // Store received data
        *p_length = i; // store number of received frames

        // save package to output buffer only if read option used, otherwise store 0
        p_dataBuffer[i] = (dataToTransmit.bf.rw == READ) ? (dataToReceive.dw) : (0);

        if (isReceivedDataValid == FALSE)
        {
            // Stop sequence execution if any frame of the received data is not valid
            // Last item in output data caused validation error, all previous elements are valid
            return FALSE;
        }
    }
    return TRUE;
}

boolean QSPIReadSensor(uint8 spiChannel, uint16 address, uint32 * const p_data)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Configure SPI channel for communication and stop execution if unsuccessful
    SpiBusSelectEnum SpiBusNumber = QSPIUpdateChannelConfig(spiChannel);
    if (SpiBusNumber == SPI_BUS_INVALID) return FALSE;


    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    SPITransmitDataSensor dataToTransmit;
    SPIReceiveDataSensor dataToReceive;

    // Configure and execute read request
    dataToTransmit.dw = 0;
    dataToTransmit.bf.sensor_data = TRUE;
    dataToTransmit.bf.address = address;
    dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
    QSPIExchangeData(SpiBusNumber, &dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

    // Validating input
    isReceivedDataValid = IsCRC3Correct(&(dataToReceive.dw), dataToReceive.bf.crc);
    isReceivedDataValid &= (dataToReceive.bf.asic_error_flag == FALSE);

    // Return data
    *p_data = dataToReceive.dw;

    return (isReceivedDataValid);
}

boolean QSPIReadSequenceSensor(uint8 spiChannel, const uint16 * const p_addressBuffer, uint32 * const p_dataBuffer, uint16 * const p_length)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Configure SPI channel for communication and stop execution if unsuccessful
    SpiBusSelectEnum SpiBusNumber = QSPIUpdateChannelConfig(spiChannel);
    if (SpiBusNumber == SPI_BUS_INVALID) return FALSE;

    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    uint16 numberOfFrames = (*p_length);
    SPITransmitDataSensor dataToTransmit;
    SPIReceiveDataSensor dataToReceive;

    // Reset length variable to store number of received frames there
    *p_length = 0;

    // Null pointer check
    if ((p_addressBuffer == NULL_PTR) || (p_dataBuffer == NULL_PTR) || 
        (p_length == NULL_PTR))
    {
        return FALSE;
    }

    // Configure common parameters of SPI normal frame
    dataToTransmit.dw = 0;
    dataToTransmit.bf.sensor_data = TRUE;

    // Execute series of SPI transactions
    for (uint16 i = 0; i < numberOfFrames; i++)
    {
        // Reading/writing data from addresses in address buffer
        dataToTransmit.bf.address = p_addressBuffer[i];
        dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));

        // Execute SPI transaction exchange
        QSPIExchangeData(SpiBusNumber, &dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

        // Validating input
        isReceivedDataValid = IsCRC3Correct(&(dataToReceive.dw), dataToReceive.bf.crc);
        isReceivedDataValid &= (dataToReceive.bf.asic_error_flag == FALSE);

        // Store received data
        *p_length = i; // store number of received frames

        // save package to output buffer, -1 to account on first transaction response (not related)
        p_dataBuffer[i] = dataToReceive.dw;

        if (isReceivedDataValid == FALSE)
        {
            // Stop sequence execution if any frame of the received data is not valid
            // Last item in output data caused validation error, all previous elements are valid
            return FALSE;
        }
    }
    return TRUE;
}

#endif
