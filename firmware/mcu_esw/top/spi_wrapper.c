/**********************************************************************************************************************
 * \file spi_wrapper.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

#include "Ifx_Types.h"
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
#define SPI_DUMMY_TRANSACTION           (0x00000004)       /** \brief Dummy SPI read transaction incl CRC3 checksum  */

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static boolean enSPICommunication = FALSE;                 /** \brief Flag indicating if SPI communication enabled   */

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

boolean QSPIReadFast(uint16 address, uint32 * const data)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    SPITransmitData dataToTransmit;
    SPIReceiveData dataToReceive;

    // Configure and execute read request
    dataToTransmit.dw = 0;
    dataToTransmit.bf.instruction = address;
    dataToTransmit.bf.pe = 0x0;
    dataToTransmit.bf.data = READ;
    dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
    QSPIExchangeData(&dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

    // Validating input
    isReceivedDataValid = IsCRC5Correct(&dataToReceive);
    isReceivedDataValid &= !(dataToReceive.bf.errBus || dataToReceive.bf.errAt || dataToReceive.bf.errCRC || dataToReceive.bf.errSck);
    isReceivedDataValid = TRUE;

    // Return data
    *data = dataToReceive.dw;

    return (isReceivedDataValid);
}

void QSPIWriteFast(uint16 address, uint16 data)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return;

    SPITransmitData dataToTransmit;
    SPIReceiveData dataToReceive; // data in this variable unused, provides place to store incoming data

    dataToTransmit.dw = 0;
    dataToTransmit.bf.instruction = address;
    dataToTransmit.bf.data = data;
    dataToTransmit.bf.pe = WRITE;
    dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));

    QSPIExchangeData(&dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

    return;
}

boolean QSPIReadSequence(const uint16 * const addressBuffer, uint32 * const dataBuffer, uint16 * const length)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    uint16 numberOfFrames = (*length);
    SPITransmitData dataToTransmit;
    SPIReceiveData dataToReceive;

    // Reset length variable to store number of received frames there
    *length = 0;

    // Null pointer check
    if ((addressBuffer == NULL_PTR) || (dataBuffer == NULL_PTR) || (length == NULL_PTR))
    {
        return FALSE;
    }

    // Configure common parameters of SPI read frame
    dataToTransmit.dw = 0;
    dataToTransmit.bf.data = 0x0;
    dataToTransmit.bf.pe = READ;

    // Execute series of SPI transactions
    for (uint16 i = 0; i < numberOfFrames + 1; i++)
    {
        // Configure read transaction
        if (i < numberOfFrames)
        {
            // Reading data from addresses in address buffer
            dataToTransmit.bf.instruction = addressBuffer[i];
            dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
        }
        else
        {
            // Injecting one extra transaction to receive data from last address
            dataToTransmit.dw = SPI_DUMMY_TRANSACTION;
        }

        // Execute SPI transaction exchange
        QSPIExchangeData(&dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

        // Ignore response for first transaction
        if (i == 0) continue;

        // Validating input
        // TODO:
        //isReceivedDataValid = IsCRC5Correct(&dataToReceive);
        //isReceivedDataValid &= !(dataToReceive.bf.errBus || dataToReceive.bf.errAt || dataToReceive.bf.errCRC || dataToReceive.bf.errSck);
        isReceivedDataValid = TRUE;
        // Store received data
        *length = i; // store number of received frames
        dataBuffer[i - 1] = dataToReceive.dw; // save package to output buffer, -1 to account on first transaction response (not related)

        if (isReceivedDataValid == FALSE)
        {
            // Stop reading sequence if one of received data is not valid
            // Last item in output data caused validation error, all previous elements are valid
            return FALSE;
        }
    }

    return TRUE;
}

boolean QSPIWriteSequence(const uint16 * const addressBuffer, const uint16 * const dataBuffer, uint16 * const length)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Initialize variables
    boolean noErrorInRespose = FALSE;
    uint16 numberOfFrames = (*length);
    SPITransmitData dataToTransmit;
    SPIReceiveData dataToReceive;

    // Reset length variable to store number of received frames there
    *length = 0;

    // Null pointer check
    if ((addressBuffer == NULL_PTR) || (dataBuffer == NULL_PTR) || (length == NULL_PTR))
    {
        return FALSE;
    }

    // Configure common parameters of SPI read frame
    dataToTransmit.dw = 0;
    dataToTransmit.bf.pe = WRITE;

    // Execute series of SPI transactions
    for (uint16 i = 0; i < numberOfFrames + 1; i++)
    {
        // Configure read transaction
        if (i < numberOfFrames)
        {
            // Reading data from addresses in address buffer
            dataToTransmit.bf.instruction = addressBuffer[i];
            dataToTransmit.bf.data = dataBuffer[i];
            dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
        }
        else
        {
            // Injecting one extra transaction to receive data from last address
            dataToTransmit.dw = SPI_DUMMY_TRANSACTION;
        }

        // Execute SPI transaction exchange
        QSPIExchangeData(&dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

        // Ignore response for first transaction
        if (i == 0) continue;

        // Validating input
        // TODO
        //noErrorInRespose = IsCRC5Correct(&dataToReceive);
        //noErrorInRespose &= !(dataToReceive.bf.errBus || dataToReceive.bf.errAt || dataToReceive.bf.errCRC || dataToReceive.bf.errSck);
        noErrorInRespose = TRUE;
        if (noErrorInRespose == FALSE)
        {
            // Stop reading sequence if one of received data is not valid
            return FALSE;
        }
        else
        {
            *length = i; // store number of received frames
        }
    }

    return TRUE;
}

/*
 * use QSPI command from CS600 for AB12. using adress instead of instruction*/
boolean QSPIReadWriteSequence(const uint16 * const addressBuffer, uint32 * const dataBuffer, const enum RWFlag * const rwBuffer, uint16 * const length)
{
    // Execute only if enabled
    if (enSPICommunication == FALSE) return FALSE;

    // Initialize variables
    boolean isReceivedDataValid = FALSE;
    uint16 numberOfFrames = (*length);
    SPITransmitData dataToTransmit;
    SPIReceiveData dataToReceive;

    // Reset length variable to store number of received frames there
    *length = 0;

    // Null pointer check
    if ((addressBuffer == NULL_PTR) || (dataBuffer == NULL_PTR) || (length == NULL_PTR))
    {
        return FALSE;
    }

    // Configure common parameters of SPI read frame
    dataToTransmit.dw = 0;

    // Execute series of SPI transactions

    // TODO: code for removal
#ifdef CS600
    /* CS 600 needs two write/read sequence, first seq initiaztes request, second collects data*/
    for (uint16 i = 0; i < numberOfFrames + 1; i++)
    {

        // Configure read transaction
        if (i < numberOfFrames)
        {
            // Reading/writing data from addresses in address buffer
            dataToTransmit.bf.instruction = addressBuffer[i];
            dataToTransmit.bf.pe = rwBuffer[i];
            dataToTransmit.bf.data = dataBuffer[i];
            dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
        }
        else
        {
            // Injecting one extra transaction to receive data from last address
            dataToTransmit.dw = SPI_DUMMY_TRANSACTION;
        }
#else
    // for AB12 we data comes back in same SPI command --> no dummy read needed
    for (uint16 i = 0; i < numberOfFrames ; i++)
    {
        // Reading/writing data from addresses in address buffer
        dataToTransmit.bf.instruction = addressBuffer[i];
        dataToTransmit.bf.pe = rwBuffer[i];
        dataToTransmit.bf.data = dataBuffer[i];
        dataToTransmit.bf.crc = GetCRC3(&(dataToTransmit.dw));
#endif

        // Execute SPI transaction exchange
        QSPIExchangeData(&dataToTransmit.dw, &dataToReceive.dw, SPI_TRANSACTION_LENGTH);

   // TODO: code for removal
#ifdef CS600
        // CS600: Ignore response for first transaction
        /*TCD 6.12.1.The SPI protocol supported by the CS600 uses an out-of-frame transfer. Register read data is provided
        by the slave in the next MISO-frame after a read request. Register write data is directly accepted from
        the MOSI-frame */
        if (i == 0) continue;
#else
        //AB12 relevant data in first transaction, dont ignore first return
#endif
        // TODO: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // Validating input
        // isReceivedDataValid = IsCRC5Correct(&dataToReceive);
        // isReceivedDataValid &= !(dataToReceive.bf.errBus || dataToReceive.bf.errAt || dataToReceive.bf.errCRC || dataToReceive.bf.errSck);
        isReceivedDataValid = TRUE;
        // Store received data
        *length = i; // store number of received frames
        // save package to output buffer, -1 to account on first transaction response (not related)
#ifdef CS600
        dataBuffer[i - 1] = dataToReceive.dw;
#else
        dataBuffer[i] = dataToReceive.dw;
#endif
        if (isReceivedDataValid == FALSE)
        {
            // Stop reading sequence if one of received data is not valid
            // Last item in output data caused validation error, all previous elements are valid
            return FALSE;
        }

    }
    return TRUE;
}
