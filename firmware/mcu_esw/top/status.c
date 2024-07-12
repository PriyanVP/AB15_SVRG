/**********************************************************************************************************************
 * \file status.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "status.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*************************************************************************************************************************/
/*-------------------------------------------------Data Structures-------------------------------------------------------*/
/*************************************************************************************************************************/

/** \brief Structure for holding status data for MCU
 */
typedef union
{
    struct
    {
        uint32 state            : 4;    /** \brief state of MCU SW */
        uint32 usbError         : 1;    /** \brief error during USB communication occured */
        uint32 spiError         : 1;    /** \brief error during SPI communication occured */
        uint32 contReadState    : 2;    /** \brief continuous read feature state */
        uint32 errorCheckState  : 2;    /** \brief error check feature state */
        uint32 watchdogState    : 2;    /** \brief watchdog feature state */
        uint32 gpioState        : 2;    /** \brief gpio feature state */
        uint32 reserved         : 18;   /** \brief reserved bits */
    } bf;
    uint32 dw;
} StatusType;

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

StatusType g_status;                              /** \brief Global status variable                                  */

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void InitStatus(void)
{
    g_status.dw = 0;
}

uint32 GetStatus(void)
{
    return g_status.dw;
}

void SetState(MCUStatesEnum newState)
{
    g_status.bf.state = (uint8) newState;
}

boolean GetUSBError(void)
{
    return (g_status.bf.usbError == 1);
}

boolean GetSPIError(void)
{
    return (g_status.bf.spiError == 1);
}

void SetUSBError(void)
{
    g_status.bf.usbError = 1;
}

void SetSPIError(void)
{
    g_status.bf.spiError = 1;
}

void ClearUSBError(void)
{
    g_status.bf.usbError = 0;
}

void ClearSPIError(void)
{
    g_status.bf.spiError = 0;
}

ContinuousFeaturesStatesEnum GetContinuousReadState(void)
{
    return g_status.bf.contReadState;
}

ContinuousFeaturesStatesEnum GetErrorCheckState(void)
{
    return g_status.bf.errorCheckState;
}

ContinuousFeaturesStatesEnum GetWatchdogState(void)
{
    return g_status.bf.watchdogState;
}

ContinuousFeaturesStatesEnum GetGPIOState(void)
{
    return g_status.bf.gpioState;
}

void SetContinuousReadState(ContinuousFeaturesStatesEnum newState)
{
    g_status.bf.contReadState = (uint8) newState;
}

void SetErrorCheckState(ContinuousFeaturesStatesEnum newState)
{
    g_status.bf.errorCheckState = (uint8) newState;
}

void SetWatchdogState(ContinuousFeaturesStatesEnum newState)
{
    g_status.bf.watchdogState = (uint8) newState;
}

void SetGPIOState(ContinuousFeaturesStatesEnum newState)
{
    g_status.bf.gpioState = (uint8) newState;
}