/**********************************************************************************************************************
 * \file spi.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef SPI_H_
#define SPI_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "IfxQspi_SpiMaster.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Data Structures---------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Structure for QSPI handle required for its functions
 */
typedef struct
{
    IfxQspi_SpiMaster         spiMaster;            /** \brief QSPI Master handle            */
    IfxQspi_SpiMaster_Channel spiMasterChannel;     /** \brief QSPI Master Channel handle    */
} QSPIHandles;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Initialize the QSPI HW and SW handle for operations
 *
 * \return Returns nothing.
 */
void QSPIInitPeriphery(void);

/** \brief Deinitialize the QSPI HW and SW handle for operations
 * Set pins to high-z state (when possible)
 *
 * \return Returns nothing.
 */
void QSPIDeinitPeriphery(void);

/** \brief Initialize SPI transaction and exchange 32bit packages
 * This functions:\n
 * 1) Initiates SPI communication from master side\n
 * 2) Sends 32 bit package to slave\n
 * 3) Receives 32 bit package from slave\n
 * 4) Waits for configured time to avoid issues in communication
 *
 * \param dataToSend pointer to buffer with data to send
 * \param dataToReceive pointer to empty buffer which will be filled with data from slave
 * \param length data length in bytes
 * \return Returns nothing.
 */
void QSPIExchangeData(const uint32 * const dataToSend, uint32 * const dataOut, uint8 length);

#endif /* SPI_H_ */
