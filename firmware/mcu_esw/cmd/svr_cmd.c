/**********************************************************************************************************************
 * \file svr_cmd.c
 * \copyright Copyright (C) RobertBosch GmbH
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "svr_cmd.h"
#include "common/bit_manipulation.h"
#include "top/usb_wrapper.h"
#include "periphery/svr.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define SVR_NO_CHANGE 0xFF
#define SVR_OFF 0x00
#define SVR_ON 0x11

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

static inline boolean UpdateSVRPin(SVRPinsEnum pinIdx, uint8 svrCommand);

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdSetSvr(USBReceiveData const * const commandPackage)
{
    // Construct package to PC
    USBTransmitData packageToSend;
    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.dataLength = 0;
    packageToSend.status = USB_STATUS_ACK;

    if (commandPackage->dataLength != 2)
    {
        packageToSend.status = USB_STATUS_ERROR;
    }
    else
    {
	    // Get value of SVR from received data
    	uint8 svr1Command = commandPackage->data[0];
    	uint8 svr2Command = commandPackage->data[1];

    	if (UpdateSVRPin(SVR1, svr1Command) == FALSE)
    	{
	        packageToSend.status = USB_STATUS_ERROR;
    	}

    	if (UpdateSVRPin(SVR2, svr2Command) == FALSE)
    	{
	        packageToSend.status = USB_STATUS_ERROR;
	    }
    }

    // Send data back to MCU
    SendUSBPackage(&packageToSend);
}

static inline boolean UpdateSVRPin(SVRPinsEnum pinIdx, uint8 svrCommand)
{
  	boolean isSuccessfulFlag = TRUE;
    if (svrCommand == SVR_ON)
    {
        SetSVRPin(pinIdx);
    }
    else if (svrCommand == SVR_OFF)
    {
        ClearSVRPin(pinIdx);
    }
    else if (svrCommand == SVR_NO_CHANGE)
    {
        // Do nothing
    }
    else
    {
        isSuccessfulFlag = FALSE;
    }
    return isSuccessfulFlag;
}
