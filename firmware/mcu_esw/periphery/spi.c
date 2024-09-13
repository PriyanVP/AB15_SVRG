/**********************************************************************************************************************
 * \file spi.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "spi.h"
#include "IfxPort.h"
#include "common/Ifx_IntPrioDef.h"
#include "Bsp.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/* QSPI modules */
#define QSPI_MASTER                &MODULE_QSPI1    /** \brief SPI Master module                                    */

#define MASTER_CHANNEL_BAUDRATE     1000000         /** \brief Master channel baud rate in Hz                       */

#define SPI_TIMEOUT                 (1)             /** \brief SPI timeout, if elapsed - exit from busywait          */

#define SWAP_ENDIAN(val) \
 ( (((val) >> 24) & 0x000000FF) | (((val) >>  8) & 0x0000FF00) | \
   (((val) <<  8) & 0x00FF0000) | (((val) << 24) & 0xFF000000) )        /** \brief Swap endians for 32 bit values    */

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static QSPIHandles g_qspi;                         /** \brief QSPI object to store module and channel handles               */

/* Select the port pins for communication */
const IfxQspi_SpiMaster_Pins qspi1MasterPins = {
    &IfxQspi1_SCLK_P10_2_OUT, IfxPort_OutputMode_pushPull,          /* Digital pin 13 (PWM/SPCK) */ /* SCLK Pin                       (CLK)     */
    &IfxQspi1_MTSR_P10_3_OUT, IfxPort_OutputMode_pushPull,          /* Digital pin 11 (PWM/MOSI) */ /* MasterTransmitSlaveReceive pin (MOSI)    */
    &IfxQspi1_MRSTA_P10_1_IN, IfxPort_InputMode_pullDown,           /* Digital pin 12 (PWM/MISO) */ /* MasterReceiveSlaveTransmit pin (MISO)    */
    IfxPort_PadDriver_ttl3v3Speed3                                                                  /* Pad driver mode                          */
};

/* setup the different SLSO pins for SPi slaves according SPI tab in PinMapping Table */

const IfxQspi_SpiMaster_Output qspi1Slave3Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO3_P11_10_OUT, IfxPort_OutputMode_pushPull,     /* Slave Select 1 port pin (CS1_SENSOR2)    */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi1Slave4Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO4_P11_11_OUT, IfxPort_OutputMode_pushPull,     /* Slave Select 1 port pin (CS1_SENSOR3)    */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi1Slave5Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO5_P11_2_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select 1 port pin (CS1_SENSOR1)    */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi1Slave8Select = {                 /* QSPI1 Master selects the QSPI2 Slave      */
        &IfxQspi1_SLSO8_P10_4_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select 2 port pin (CS_MON1)         */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                           */
};

const IfxQspi_SpiMaster_Output qspi1Slave9Select = {                 /* QSPI1 Master selects the QSPI1 Slave      */
        &IfxQspi1_SLSO9_P10_5_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select 1 port pin (CS1_MASTER)      */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                           */
};
/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief QSPI Master initialization
 * This functions:\n
 * 1) Initializes the QSPI1 module in Master mode.\n
 * 2) Configure MISO, MOSI, CLK pins.\n
 *
 * \return Returns nothing.
 */
void QSPIMasterModuleInit(void);

/** \brief QSPI Master channel initialization
 * This functions:\n
 * 1) Initializes the QSPI1 Master channel.\n
 * 2) Configure CS pin.\n
 * 3) Configure baudrate.\n
 *
 * \return Returns nothing.
 */
// TODO: bug in iLLD: function not existing : void QSPIMasterModuleInitChannel(void);

/*********************************************************************************************************************/
/*----------------------------------------------Function Implementations---------------------------------------------*/
/*********************************************************************************************************************/
IFX_INTERRUPT(MasterTxISR, 0, ISR_PRIORITY_MASTER_TX);                /** \brief SPI Master ISR for transmit data    */

void MasterTxISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrTransmit(&g_qspi.spiMaster);
}

IFX_INTERRUPT(MasterRxISR, 0, ISR_PRIORITY_MASTER_RX);                /** \brief SPI Master ISR for receive data     */

void MasterRxISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrReceive(&g_qspi.spiMaster);
}

IFX_INTERRUPT(MasterErISR, 0, ISR_PRIORITY_MASTER_ER);                /** \brief SPI Master ISR for error            */

void MasterErISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrError(&g_qspi.spiMaster);
}

void QSPIMasterModuleInit(void)
{
    IfxQspi_SpiMaster_Config spiMasterConfig;                           /* Define a Master configuration            */

    IfxQspi_SpiMaster_initModuleConfig(&spiMasterConfig, QSPI_MASTER); /* Initialize it with default values        */

    spiMasterConfig.base.mode = SpiIf_Mode_master;                      /* Configure the mode                       */

    spiMasterConfig.pins = &qspi1MasterPins;                            /* Assign the Master's port pins            */

    /* Set the ISR priorities and the service provider */
    spiMasterConfig.base.txPriority = ISR_PRIORITY_MASTER_TX;
    spiMasterConfig.base.rxPriority = ISR_PRIORITY_MASTER_RX;
    spiMasterConfig.base.erPriority = ISR_PRIORITY_MASTER_ER;
    spiMasterConfig.base.isrProvider = IfxSrc_Tos_cpu0;

    /* Initialize the QSPI Master module */
    IfxQspi_SpiMaster_initModule(&g_qspi.spiMaster, &spiMasterConfig);
}

void QSPIMasterChannelInit(Spi1SlaveSelectEnum SpiChannel)
{
    IfxQspi_SpiMaster_ChannelConfig spiMasterChannelConfig;             /* Define a Master Channel configuration    */

    /* Initialize the configuration with default values */
    // Bitorder, default pin states, etc. are set in base configuration of SPI Master chanel
    IfxQspi_SpiMaster_initChannelConfig(&spiMasterChannelConfig, &g_qspi.spiMaster);

    spiMasterChannelConfig.base.baudrate = MASTER_CHANNEL_BAUDRATE;     /* Set SCLK frequency                       */

    /* Select the port pin for the Chip Select signal --> SLSO9 (CS_MASTER) as default*/
    switch(SpiChannel){
        case SPI1_CSMON1:                    /* SLSO8    */
            spiMasterChannelConfig.sls.output = qspi1Slave8Select;
            break;
        case SPI1_CS1MASTER:                 /* SLSO9   */
            spiMasterChannelConfig.sls.output = qspi1Slave9Select;
            break;
        case SPI1_CS1_SENSOR1:                 /* SLSO5    */
            spiMasterChannelConfig.sls.output = qspi1Slave5Select;
            break;
        case SPI1_CS1_SENSOR2:                 /* SLSO3    */
            spiMasterChannelConfig.sls.output = qspi1Slave3Select;
            break;
        case SPI1_CS1_SENSOR3:                 /* SLSO4    */
            spiMasterChannelConfig.sls.output = qspi1Slave4Select;
            break;
        default:
            /* by fdefault use SLSO9, default master    */
            spiMasterChannelConfig.sls.output = qspi1Slave9Select;
        break;
    }
    /* Initialize the QSPI Master channel */
    IfxQspi_SpiMaster_initChannel(&g_qspi.spiMasterChannel, &spiMasterChannelConfig);
}


void QSPIInitPeriphery(void)
{
    /* Initialize the Master */
    QSPIMasterModuleInit();
    QSPIMasterChannelInit(SPI1_CS1MASTER);
}

void QSPIDeinitPeriphery(void)
{
    // Clear all flags and requests for QSPI
    IfxQspi_clearAllEventFlags(g_qspi.spiMaster.qspi);

    // Module deinit
    IfxQspi_resetModule(g_qspi.spiMaster.qspi);

    // Set all pins to input without pull-up (high-Z state)
    IfxPort_setPinModeInput(qspi1MasterPins.sclk->pin.port, qspi1MasterPins.sclk->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1MasterPins.mrst->pin.port, qspi1MasterPins.mrst->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1MasterPins.mtsr->pin.port, qspi1MasterPins.mtsr->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    // set all slave select pins
    IfxPort_setPinModeInput(qspi1Slave8Select.pin->pin.port, qspi1Slave8Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice); // has HW pull up
    IfxPort_setPinModeInput(qspi1Slave9Select.pin->pin.port, qspi1Slave9Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice); // has HW pull up
    IfxPort_setPinModeInput(qspi1Slave5Select.pin->pin.port, qspi1Slave5Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice); // has HW pull up
    IfxPort_setPinModeInput(qspi1Slave3Select.pin->pin.port, qspi1Slave3Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice); // has HW pull up
    IfxPort_setPinModeInput(qspi1Slave4Select.pin->pin.port, qspi1Slave4Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice); // has HW pull up
}

void QSPIExchangeData(const uint32 * const dataToSend, uint32 * const dataOut, uint8 length)
{
    // Temporary variables to store data with correct endian for communication
    uint32 dataToSendSwapped = SWAP_ENDIAN(*dataToSend);
    uint32 dataToRecive = 0;

    sint32 timeout = IfxStm_getTicksFromMilliseconds(BSP_DEFAULT_TIMER, SPI_TIMEOUT);
    IfxQspi_SpiMaster_exchange(&g_qspi.spiMasterChannel, &dataToSendSwapped, &dataToRecive, length);
    while (IfxQspi_SpiMaster_getStatus(&g_qspi.spiMasterChannel) == SpiIf_Status_busy)
    {
        // If SPI timeout elepsed, exit from loop
        if ((--timeout) <= 0) break;
    }
    *dataOut = SWAP_ENDIAN(dataToRecive);
}
