/**********************************************************************************************************************
 * \file spi.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "spi.h"
#include "Ifx_Types.h"
#include "IfxPort.h"
#include "common/Ifx_IntPrioDef.h"
#include "common/spi_data_types.h"
#include "Bsp.h"
/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/* QSPI modules */
#define QSPI_MASTER1               &MODULE_QSPI1    /** \brief SPI1 Master module                                    */
#define QSPI_MASTER2               &MODULE_QSPI2    /** \brief SPI2 Master module                                    */

#define MASTER_CHANNEL_BAUDRATE     1000000         /** \brief Master channel baud rate in Hz                       */

#define SPI_TIMEOUT                 (1)             /** \brief SPI timeout, if elapsed - exit from busywait          */

#define SWAP_ENDIAN(val) \
 ( (((val) >> 24) & 0x000000FF) | (((val) >>  8) & 0x0000FF00) | \
   (((val) <<  8) & 0x00FF0000) | (((val) << 24) & 0xFF000000) )        /** \brief Swap endians for 32 bit values    */

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static QSPIHandles g_qspi1;                         /** \brief QSPI object to store module and channel handles               */
static QSPIHandles g_qspi2;                        /** \brief QSPI object to store spi2 module and channel handles               */

/* Select the port pins for SPI1 communication */
const IfxQspi_SpiMaster_Pins qspi1MasterPins = {
    &IfxQspi1_SCLK_P10_2_OUT, IfxPort_OutputMode_pushPull,          /* Digital pin 13 (PWM/SPCK) */ /* SCLK Pin                       (CLK)     */
    &IfxQspi1_MTSR_P10_3_OUT, IfxPort_OutputMode_pushPull,          /* Digital pin 11 (PWM/MOSI) */ /* MasterTransmitSlaveReceive pin (MOSI)    */
    &IfxQspi1_MRSTA_P10_1_IN, IfxPort_InputMode_pullDown,           /* Digital pin 12 (PWM/MISO) */ /* MasterReceiveSlaveTransmit pin (MISO)    */
    IfxPort_PadDriver_ttl3v3Speed3                                                                  /* Pad driver mode                          */
};

/* Select the port pins for SPI2 communication */
const IfxQspi_SpiMaster_Pins qspi2MasterPins = {
    &IfxQspi2_SCLK_P15_3_OUT, IfxPort_OutputMode_pushPull,          /* Digital pin  (PWM/SPCK) */ /* SCLK Pin                       (CLK)     */
    &IfxQspi2_MTSR_P15_5_OUT, IfxPort_OutputMode_pushPull,          /* Digital pin  (PWM/MOSI) */ /* MasterTransmitSlaveReceive pin (MOSI)    */
    //&IfxQspi2_MRST_P15_4_OUT, IfxPort_InputMode_pullDown,         /* Digital pin  (PWM/MISO) */ /* MasterReceiveSlaveTransmit pin (MISO)    */
    &IfxQspi2_MRSTA_P15_4_IN, IfxPort_InputMode_pullDown,           /* Digital pin  (PWM/MISO) */ /* MasterReceiveSlaveTransmit pin (MISO)    */
    IfxPort_PadDriver_ttl3v3Speed3                                  /* TODO: check Pad driver mode  ok for Slave 2 as well                    */
};


/* setup the different SLSO pins for SPI 1 slaves according SPI tab in PinMapping Table. only pin P10.5
 * can be identified to have pullup resistor, others can not confirmed --> set all to push pull to be safe*/

const IfxQspi_SpiMaster_Output qspi1Slave3Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO3_P11_10_OUT, IfxPort_OutputMode_pushPull,     /* Slave Select port pin (CS1_SENSOR2)      */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi1Slave4Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO4_P11_11_OUT, IfxPort_OutputMode_pushPull,     /* Slave Select port pin (CS1_SENSOR3)      */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi1Slave5Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO5_P11_2_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select port pin (CS1_SENSOR1)      */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi1Slave8Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO8_P10_4_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select port pin (CS_MON1)          */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi1Slave9Select = {                 /* QSPI1 Master selects the QSPI1 Slave     */
        &IfxQspi1_SLSO9_P10_5_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select port pin (CS1_MASTER)       */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};


/* setup the different SLSO pins for SPI 2 slaves according SPI tab in PinMapping Table.*/

const IfxQspi_SpiMaster_Output qspi2Slave5Select = {                 /* QSPI2 Master selects the QSPI2 Slave     */
        &IfxQspi2_SLSO5_P15_1_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select port pin (CS2_SENSOR1)      */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi2Slave0Select = {                 /* QSPI2 Master selects the QSPI2 Slave     */
        &IfxQspi2_SLSO0_P15_2_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select port pin (CS_MON2)          */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi2Slave8Select = {                 /* QSPI2 Master selects the QSPI2 Slave     */
        &IfxQspi2_SLSO8_P20_6_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select port pin (CS2_SENSOR2)      */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi2Slave10Select = {                /* QSPI2 Master selects the QSPI2 Slave     */
        &IfxQspi2_SLSO10_P33_2_OUT, IfxPort_OutputMode_pushPull,     /* Slave Select port pin (CS2_SLAVE1)       */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi2Slave9Select = {                 /* QSPI2 Master selects the QSPI2 Slave     */
        &IfxQspi2_SLSO9_P20_3_OUT, IfxPort_OutputMode_pushPull,      /* Slave Select port pin (CS2_SLAVE2)       */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

const IfxQspi_SpiMaster_Output qspi2Slave11Select = {                /* QSPI2 Master selects the QSPI2 Slave     */
        &IfxQspi2_SLSO11_P33_6_OUT, IfxPort_OutputMode_pushPull,     /* Slave Select port pin (CS2_SLAVE3)       */
        IfxPort_PadDriver_ttl3v3Speed1                               /* Pad driver mode                          */
};

/*
 * QSPI2
 * SPI2     Slave select output #
P15_1   CS2_SENSOR1     SLSO5
P15_2   CS_MON2         SLSO0
P15_3   SCK_MON2     
P15_4   SO_MON2  
P15_5   SI_MON2  
P20_10  P21_0 P20_6 CS2_SENSOR2 SLSO8
P33_2   CS2_SLAVE1      SLSO10
P33_4  P21_3 P20_3  CS2_SLAVE2  SLSO9
P33_6   CS2_SLAVE3      SLSO11
 *
 *
 * IfxQspi2_SLSO5_P15_1_OUT
 * IfxQspi2_SLSO0_P15_2_OUT
 * IfxQspi2_SLSO8_P20_6_OUT
 * IfxQspi2_SLSO10_P33_2_OUT
 * IfxQspi2_SLSO9_P20_3_OUT
 * IfxQspi2_SLSO11_P33_6_OUT
 *
 *
 */



/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief QSPI Master1 initialization
 * This functions:\n
 * 1) Initializes the QSPI1 module in Master mode.\n
 * 2) Configure MISO, MOSI, CLK pins.\n
 *
 * \return Returns nothing.
 */
void QSPIMaster1ModuleInit(void);

/** \brief QSPI Master2 initialization
 * This functions:\n
 * 1) Initializes the QSPI1 module in Master mode.\n
 * 2) Configure MISO, MOSI, CLK pins.\n
 *
 * \return Returns nothing.
 */
void QSPIMaster2ModuleInit(void);

/** \brief QSPI Master1 channel initialization
 * This functions:\n
 * 1) Initializes the QSPI1 Master channel.\n
 * 2) Configure CS pin.\n
 * 3) Configure baudrate.\n
 *
 * \return Returns nothing.
 */
void QSPIMaster1ChannelInit(SpiSlsoLinesEnum spiSlaveSel);

/** \brief QSPI Master2 channel initialization
 * This functions:\n
 * 1) Initializes the QSPI1 Master channel.\n
 * 2) Configure CS pin.\n
 * 3) Configure baudrate.\n
 *
 * \return Returns nothing.
 */
void QSPIMaster2ChannelInit(SpiSlsoLinesEnum spiSlaveSel);
/*********************************************************************************************************************/
/*----------------------------------------------Function Implementations---------------------------------------------*/
/*********************************************************************************************************************/
IFX_INTERRUPT(Master1TxISR, 0, ISR_PRIORITY_MASTER1_TX);                /** \brief SPI1 Master ISR for transmit data    */

void Master1TxISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrTransmit(&g_qspi1.spiMaster);
}

IFX_INTERRUPT(Master1RxISR, 0, ISR_PRIORITY_MASTER1_RX);                /** \brief SPI1 Master ISR for receive data     */

void Master1RxISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrReceive(&g_qspi1.spiMaster);
}

IFX_INTERRUPT(Master1ErISR, 0, ISR_PRIORITY_MASTER1_ER);                /** \brief SPI1 Master ISR for error            */

void Master1ErISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrError(&g_qspi1.spiMaster);
}


IFX_INTERRUPT(Master2TxISR, 0, ISR_PRIORITY_MASTER2_TX);                /** \brief SPI2 Master ISR for transmit data    */

void Master2TxISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrTransmit(&g_qspi2.spiMaster);
}

IFX_INTERRUPT(Master2RxISR, 0, ISR_PRIORITY_MASTER2_RX);                /** \brief SPI2 Master ISR for receive data     */

void Master2RxISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrReceive(&g_qspi2.spiMaster);
}

IFX_INTERRUPT(Master2ErISR, 0, ISR_PRIORITY_MASTER2_ER);                /** \brief SPI2 Master ISR for error            */

void Master2ErISR()
{
    IfxCpu_enableInterrupts();
    IfxQspi_SpiMaster_isrError(&g_qspi2.spiMaster);
}

void QSPIMaster1ModuleInit(void)
{
    IfxQspi_SpiMaster_Config spiMasterConfig;                           /* Define a Master configuration            */

    IfxQspi_SpiMaster_initModuleConfig(&spiMasterConfig, QSPI_MASTER1); /* Initialize it with default values        */

    spiMasterConfig.base.mode = SpiIf_Mode_master;                      /* Configure the mode                       */

    spiMasterConfig.pins = &qspi1MasterPins;                            /* Assign the Master's port pins            */

    /* Set the ISR priorities and the service provider */
    spiMasterConfig.base.txPriority = ISR_PRIORITY_MASTER1_TX;
    spiMasterConfig.base.rxPriority = ISR_PRIORITY_MASTER1_RX;
    spiMasterConfig.base.erPriority = ISR_PRIORITY_MASTER1_ER;
    spiMasterConfig.base.isrProvider = IfxSrc_Tos_cpu0;

    /* Initialize the QSPI Master module */
    IfxQspi_SpiMaster_initModule(&g_qspi1.spiMaster, &spiMasterConfig);
}

void QSPIMaster2ModuleInit(void)
{
    IfxQspi_SpiMaster_Config spiMasterConfig;                           /* Define a Master configuration            */

    IfxQspi_SpiMaster_initModuleConfig(&spiMasterConfig, QSPI_MASTER2); /* Initialize it with default values        */

    spiMasterConfig.base.mode = SpiIf_Mode_master;                      /* Configure the mode                       */

    spiMasterConfig.pins = &qspi2MasterPins;                            /* Assign the Master's port pins            */

    /* Set the ISR priorities and the service provider */
    spiMasterConfig.base.txPriority = ISR_PRIORITY_MASTER2_TX;
    spiMasterConfig.base.rxPriority = ISR_PRIORITY_MASTER2_RX;
    spiMasterConfig.base.erPriority = ISR_PRIORITY_MASTER2_ER;
    spiMasterConfig.base.isrProvider = IfxSrc_Tos_cpu0;

    /* Initialize the QSPI Master module */
    IfxQspi_SpiMaster_initModule(&g_qspi2.spiMaster, &spiMasterConfig);
}

void QSPIMaster1ChannelInit(SpiSlsoLinesEnum spiSlaveSel)
{
    IfxQspi_SpiMaster_ChannelConfig spiMasterChannelConfig;             /* Define a Master Channel configuration    */

    /* Initialize the configuration with default values */
    // Bitorder, default pin states, etc. are set in base configuration of SPI Master channel
    IfxQspi_SpiMaster_initChannelConfig(&spiMasterChannelConfig, &g_qspi1.spiMaster);

    spiMasterChannelConfig.base.baudrate = MASTER_CHANNEL_BAUDRATE;     /* Set SCLK frequency                       */
    // TODO: assert if SpiSlsoLinesEnum > SPI1_SLSO4

    /* Select the port pin for the Chip Select signal --> SLSO9 (CS_MASTER) as default*/
    switch(spiSlaveSel){
        case SPI1_SLSO8:
            spiMasterChannelConfig.sls.output = qspi1Slave8Select;
            break;
        case SPI1_SLSO9:
            spiMasterChannelConfig.sls.output = qspi1Slave9Select;
            break;
        case SPI1_SLSO5:
            spiMasterChannelConfig.sls.output = qspi1Slave5Select;
            break;
        case SPI1_SLSO3:
            spiMasterChannelConfig.sls.output = qspi1Slave3Select;
            break;
        case SPI1_SLSO4:
            spiMasterChannelConfig.sls.output = qspi1Slave4Select;
            break;
        default:
            /* by default use SPI1_DEFAULT_CHANNEL    */
            spiMasterChannelConfig.sls.output = qspi1Slave9Select;
        break;
    }
    /* Initialize the QSPI Master channel */
    IfxQspi_SpiMaster_initChannel(&g_qspi1.spiMasterChannel, &spiMasterChannelConfig);
}


void QSPIMaster2ChannelInit(SpiSlsoLinesEnum spiSlaveSel)
{
    IfxQspi_SpiMaster_ChannelConfig spiMasterChannelConfig;             /* Define a Master Channel configuration    */

    /* Initialize the configuration with default values */
    // Bitorder, default pin states, etc. are set in base configuration of SPI Master channel
    IfxQspi_SpiMaster_initChannelConfig(&spiMasterChannelConfig, &g_qspi2.spiMaster);

    spiMasterChannelConfig.base.baudrate = MASTER_CHANNEL_BAUDRATE;     /* Set SCLK frequency                       */
    // TODO: assert if SpiSlsoLinesEnum < SPI2_SLSO5

    /* Select the port pin for the Chip Select signal*/
    switch(spiSlaveSel){
        case SPI2_SLSO5:
            spiMasterChannelConfig.sls.output = qspi2Slave5Select;
            break;
        case SPI2_SLSO0:
            spiMasterChannelConfig.sls.output = qspi2Slave0Select;
            break;
        case SPI2_SLSO8:
            spiMasterChannelConfig.sls.output = qspi2Slave8Select;
            break;
        case SPI2_SLSO10:
            spiMasterChannelConfig.sls.output = qspi2Slave10Select;
            break;
        case SPI2_SLSO9:
            spiMasterChannelConfig.sls.output = qspi2Slave9Select;
            break;
        case SPI2_SLSO11:
            spiMasterChannelConfig.sls.output = qspi2Slave11Select;
            break;
        default:
            /* by default use SPI2_DEFAULT_CHANNEL  */
            spiMasterChannelConfig.sls.output = qspi2Slave10Select;
        break;
    }
    /* Initialize the QSPI Master channel */
    IfxQspi_SpiMaster_initChannel(&g_qspi2.spiMasterChannel, &spiMasterChannelConfig);
}

void QSPIInitPeriphery(void)
{
    /* Initialize the Master */
    QSPIMaster1ModuleInit();
    QSPIMaster2ModuleInit();
    QSPIMaster1ChannelInit(SPI1_DEFAULT_CHANNEL);
    QSPIMaster2ChannelInit(SPI2_DEFAULT_CHANNEL);
}

void QSPIDeinitPeriphery(void)
{
    // Clear all flags and requests for QSPI
    IfxQspi_clearAllEventFlags(g_qspi1.spiMaster.qspi);
    IfxQspi_clearAllEventFlags(g_qspi2.spiMaster.qspi);

    // Module deinit
    IfxQspi_resetModule(g_qspi1.spiMaster.qspi);

    // reset all spi1 pins to input without pull-up (high-Z state): switch off Pull-Device to save power consumption
    IfxPort_setPinModeInput(qspi1MasterPins.sclk->pin.port, qspi1MasterPins.sclk->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1MasterPins.mrst->pin.port, qspi1MasterPins.mrst->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1MasterPins.mtsr->pin.port, qspi1MasterPins.mtsr->pin.pinIndex, IfxPort_InputMode_noPullDevice);

    // reset all slave1 select pins
    IfxPort_setPinModeInput(qspi1Slave8Select.pin->pin.port, qspi1Slave8Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1Slave9Select.pin->pin.port, qspi1Slave9Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1Slave5Select.pin->pin.port, qspi1Slave5Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1Slave3Select.pin->pin.port, qspi1Slave3Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi1Slave4Select.pin->pin.port, qspi1Slave4Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);

    IfxQspi_resetModule(g_qspi2.spiMaster.qspi);

    // reset all spi2 pins to input without pull-up (high-Z state): switch off Pull-Device to save power consumption
    IfxPort_setPinModeInput(qspi2MasterPins.sclk->pin.port, qspi2MasterPins.sclk->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi2MasterPins.mrst->pin.port, qspi2MasterPins.mrst->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi2MasterPins.mtsr->pin.port, qspi2MasterPins.mtsr->pin.pinIndex, IfxPort_InputMode_noPullDevice);

    // reset all slave2 select pins
    IfxPort_setPinModeInput(qspi2Slave5Select.pin->pin.port, qspi2Slave5Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi2Slave0Select.pin->pin.port, qspi2Slave0Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi2Slave8Select.pin->pin.port, qspi2Slave8Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi2Slave10Select.pin->pin.port, qspi2Slave10Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi2Slave9Select.pin->pin.port, qspi2Slave9Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
    IfxPort_setPinModeInput(qspi2Slave11Select.pin->pin.port, qspi2Slave11Select.pin->pin.pinIndex, IfxPort_InputMode_noPullDevice);
}

// TODO: rename to exchane SPI1 , add SPI2
void QSPIExchangeData(SpiBusSelectEnum SpiBusNumber, const uint32 * const dataToSend, uint32 * const dataOut, uint8 length)
{
    // Temporary variables to store data with correct endian for communication
    uint32 dataToSendSwapped = SWAP_ENDIAN(*dataToSend);
    uint32 dataToRecive = 0;

    if (SpiBusNumber < SPI_BUS_1) return;
    if (SpiBusNumber > SPI_BUS_2) return;

    //use SpiBusSelectEnum as type would be good.
    if (SpiBusNumber == SPI_BUS_1)
    {
        ToggleLED4();
        sint32 timeout = IfxStm_getTicksFromMilliseconds(BSP_DEFAULT_TIMER, SPI_TIMEOUT);
        IfxQspi_SpiMaster_exchange(&g_qspi1.spiMasterChannel, &dataToSendSwapped, &dataToRecive, length);
        while (IfxQspi_SpiMaster_getStatus(&g_qspi1.spiMasterChannel) == SpiIf_Status_busy)
        {
            // If SPI timeout elepsed, exit from loop
            if ((--timeout) <= 0) break;
        }
    }
    if (SpiBusNumber == SPI_BUS_2)
    {
        ToggleLED4();
        sint32 timeout = IfxStm_getTicksFromMilliseconds(BSP_DEFAULT_TIMER, SPI_TIMEOUT);
        IfxQspi_SpiMaster_exchange(&g_qspi2.spiMasterChannel, &dataToSendSwapped, &dataToRecive, length);
        while (IfxQspi_SpiMaster_getStatus(&g_qspi2.spiMasterChannel) == SpiIf_Status_busy)
        {
            // If SPI timeout elepsed, exit from loop
            if ((--timeout) <= 0) break;
        }
    }
    *dataOut = SWAP_ENDIAN(dataToRecive);
}

uint8 QSPIUpdateChannelConfig(uint8 spiChannel)
{
    // Validate input
    if (spiChannel == SPI_CH_INVALID) return FALSE;
    if (spiChannel >= SPI_CH_ENUM_LAST) return FALSE;

    // Initialize variables
    static SpiChSlaveSelectEnum currentSpiChannelConfig = SPI_CH_ENUM_LAST; // force init to correct channel
    uint8 SpiChSlaveSelectLine;
    static uint8 SpiBusNumber = SPI_BUS_INVALID;// make SpiBusNumber static in order to return current bus number correct even if no other channel is selected.

    /*if config has changed : reconfigure SPI bus and SLSO lines to selected channel*/
    if (spiChannel != currentSpiChannelConfig)
    {
        currentSpiChannelConfig = spiChannel;
        switch(spiChannel)
        {
            case SPI1_CSMON1:                       /* SLSO8    */
                SpiChSlaveSelectLine = SPI1_SLSO8;
                SpiBusNumber = SPI_BUS_1;
                break;
            case SPI1_CS1MASTER:                    /* SLSO8    */
                SpiChSlaveSelectLine = SPI1_SLSO9;
                SpiBusNumber = SPI_BUS_1;
                break;
            case SPI1_CS1_SENSOR1:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI1_SLSO5;
                SpiBusNumber = SPI_BUS_1;
                break;
            case SPI1_CS1_SENSOR2:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI1_SLSO3;
                SpiBusNumber = SPI_BUS_1;
                break;
            case SPI1_CS1_SENSOR3:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI1_SLSO4;
                SpiBusNumber = SPI_BUS_1;
                break;
            /* SPI 2 */
            case SPI2_CS2_SENSOR1:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI2_SLSO5;
                SpiBusNumber = SPI_BUS_2;
                break;
            case SPI2_CS_MON2:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI2_SLSO0;
                SpiBusNumber = SPI_BUS_2;
                break;
            case SPI2_CS2_SENSOR2:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI2_SLSO8;
                SpiBusNumber = SPI_BUS_2;
                break;
            case SPI2_CS2_SLAVE1:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI2_SLSO10;
                SpiBusNumber = SPI_BUS_2;
                break;
            case SPI2_CS2_SLAVE2:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI2_SLSO9;
                SpiBusNumber = SPI_BUS_2;
                break;
            case SPI2_CS2_SLAVE3:                  /* SLSO8    */
                SpiChSlaveSelectLine = SPI2_SLSO11;
                SpiBusNumber = SPI_BUS_2;
                break;
            default:
                /* by default use SLSO9, default master    */
                SpiChSlaveSelectLine = SPI1_SLSO9;
                SpiBusNumber = SPI_BUS_1;
               break;
        }

        switch(SpiBusNumber)
        {
            case SPI_BUS_1:                       /* SLSO8    */
                QSPIMaster1ChannelInit(SpiChSlaveSelectLine);
                break;
            case SPI_BUS_2:                    /* SLSO8    */
                QSPIMaster2ChannelInit(SpiChSlaveSelectLine);
                break;
            default:
                SpiBusNumber = SPI_BUS_INVALID;
                break;
        }

    }


    return SpiBusNumber;
}
