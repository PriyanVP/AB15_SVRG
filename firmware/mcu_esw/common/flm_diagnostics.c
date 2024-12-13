/**********************************************************************************************************************
 * \file flm_diagnostics.c
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/
#include "Ifx_Types.h"
#include "common/tap_addrMap_ExportedMemMap_memoryMap.h"
#include "common/spi_data_types.h"
#include "common/usb_data_types.h"
#include "flm_diagnostics.h"
#include "common/bit_manipulation.h"
#include "top/spi_wrapper.h"

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

#define FLM_DIAG_INTERRUPT_PERIODICITY      (2000)  /** \brief Periodicity of FLM diagnostic performing interrupt, in 
                                                    GENERAL_TIMER_PERIODICITY tics (50us) */

#define FLM_DIAG_READ_SHORT_REGS_COUNT      (5)     /** \brief Number of registers with results of IGL/IGH short and 
                                                    leakage to battery or ground detection */

#define FLM_DIAG_READ_SQUIB_REGS_COUNT      (2)     /** \brief Number of registers with results of squib detection */

#define FLM_DIAG_READ_RES_REGS_COUNT        (21)    /** \brief Number of registers with results of loop resistance 
                                                    measurement, including SQREF */

#define FLM_DIAG_READ_VHX_REGS_COUNT        (11)    /** \brief Number of registers with results of VHx measurement 
                                                    results*/ 

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief 
 */
void CmdEnableFLMDiag();

/** \brief
 */
void CmdDisableFLMDiag();

/** \brief
 */
void CmdReadFLMDiagResults();

/** \brief
 * FLM module cyclic diagnostics interrupt routine
 * Starts cyclic diagnostics, stores results to be later sent to GUI
 */
void IntCmdExecuteFLMDiag();

/** \brief 
 * cyclic test performed automatically, no need to set FLM_DIAG_START
 */ 
void FLMShortDiag();

/** \brief 
 * SPI-triggered test, must be set by FLM_diag_mode = VHx_volt_meas_all and started FLM_DIAG_START = 1
 */
void FLMVHxDiag();

/** \brief
 * SPI-triggered test, must be set by FLM_diag_mode = Loop_res_meas_all_ch and started FLM_DIAG_START = 1
 */
void FLMLoopResDiag();

/** \brief 
 * SPI-triggered test, must be set by FLM_diag_mode = Squib_pres_test_all and started FLM_DIAG_START = 1
 */
void FLMSquibDetErrDiag();

/** \brief Select diagnostic to be run by FLM diag module
 * write diagMode to flm_diag_mode field of FLM_DIAG_START register
 */
void SetFLMDiagMode(FLMDiagModeEnum diagMode);

/** \brief
 */
void StartFLMDiag();

/** \brief get FLM diagnostic execution status from ASIC (ongoing/evaluated)
 */
flm_DiagExecStatusEnum FLMReadDiagExecStatus(void);

/** \brief
 */
void SetFLMDiagExecStatus(flm_DiagExecStatusEnum FLMCycDiagExecStatus);

/** \brief
 */
flm_DiagExecStatusEnum GetFLMDiagExecStatus(void);

/** \brief
 */
void SetFLMDiagExecOrder(flm_DiagExecOrderEnum execNumber);

/** \brief
 */
flm_DiagExecOrderEnum GetFLMDiagExecOrder (void);

/** \brief
 * Measure Battery voltage, normal range to perform diagnostics
 * is 6...18V
 */
boolean CheckBatVoltage();

/*********************************************************************************************************************/
/*-------------------------------------------------Global variables--------------------------------------------------*/
/*********************************************************************************************************************/

static flm_DiagStateEnum g_flmDiagState = FLM_DIAG_STATE_DISABLED;
static FLMCycDiagFaults g_FLMCycDiagFaultsValues;
static FLMCycDiagResults g_flmCycDiagResultsValues;
static flm_DiagExecStatusEnum g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_IDLE;
static flm_DiagExecOrderEnum g_flmDiagExecNumber = FLM_DIAG_ORDER_SHORT_DET;

/*********************************************************************************************************************/
/*---------------------------------------------Function Implementations----------------------------------------------*/
/*********************************************************************************************************************/

void CmdEnableFLMDiag(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;

    // Enable command should not be executed twice
    if (g_flmDiagState == FLM_DIAG_STATE_ENABLED)
    {
        // Skip further function execution - FLM Diag already is enabled, 
        // GUI will see no response to repeated USB_CMD_FLM_DIAG_ENABLE command
        return;
    }

    // Enable FLM diag functionality
    // FLM diag state flag is set
    g_flmDiagState = FLM_DIAG_STATE_ENABLED;
    // Configure periodicity of FLM diagnoscics MCU interrupt
    ConfigureFLMDiagPeriodicity();
    // Turn on FLM diagnostics performing interrupt of MCU
    EnableFLMDiagInterrupt();

    packageToSend.device_id = commandPackage->device_id;
    packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
    packageToSend.status = USB_STATUS_ACK;
    packageToSend.dataLength = 0;

    // Send report to GUI
    SendUSBPackage(&packageToSend);
}

void CmdDisableFLMDiag(USBReceiveData const * const commandPackage)
{
    USBTransmitData packageToSend;

    // Disable command should not be executed twice
    if (g_flmDiagState == FLM_DIAG_STATE_DISABLED)
    {
        // Skip further function execution - FLM Diag already is disabled, 
        // GUI will see no response to repeated USB_CMD_FLM_DIAG_DISABLE command
        return;
    }

    // Disable FLM diag functionality
    // FLM diag state flag is reset
    g_flmDiagState = FLM_DIAG_STATE_DISABLED;
    // Set status and number of diag to init values for proper start
    // of diagnostics on next enable
    g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_IDLE;
    g_flmDiagExecNumber = FLM_DIAG_ORDER_SHORT_DET;

    // Turn off FLM diagnostics performing interrupt of MCU
    DisableFLMDiagInterrupt();
}

void CmdReadFLMDiagResults(USBReceiveData const * const commandPackage)
    {
        USBTransmitData packageToSend;

        packageToSend.device_id = 0;
        packageToSend.msg_id = SetResponseBit(commandPackage->msg_id);
        packageToSend.status = USB_STATUS_DATA;
        packageToSend.dataLength = 86;

        // Short detection results
        packageToSend.data[0] = GetLSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch4_1);
        packageToSend.data[1] = GetMSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch4_1);
        packageToSend.data[2] = GetLSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch8_5);
        packageToSend.data[3] = GetMSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch8_5);
        packageToSend.data[4] = GetLSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch12_9);
        packageToSend.data[5] = GetMSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch12_9);
        packageToSend.data[6] = GetLSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch16_13);
        packageToSend.data[7] = GetMSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch16_13);
        packageToSend.data[8] = GetLSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch20_17);
        packageToSend.data[9] = GetMSB(g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch20_17);
        
        // VHx diagnostic results flmVHxDiagResults[]
        packageToSend.data[10] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[0].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[11] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[0].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[12] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[1].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[13] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[1].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[14] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[2].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[15] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[2].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[16] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[3].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[17] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[3].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[18] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[4].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[19] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[4].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[20] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[5].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[21] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[5].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[22] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[6].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[23] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[6].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[24] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[7].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[25] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[7].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[26] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[8].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[27] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[8].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[28] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[9].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[29] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[9].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[30] = GetLSB(g_flmCycDiagResultsValues.flmVHxDiagResults[10].FLM_Read_Diag_VHx_voltage_value);
        packageToSend.data[31] = GetMSB(g_flmCycDiagResultsValues.flmVHxDiagResults[10].FLM_Read_Diag_VHx_voltage_value);
        
        uint8 tmp1VHxVoltageValid, tmp2VHxVoltageValid = 0;
        for (uint8 i = 0; i<11; i++)
        {
            if (i < 8)
            {
                if (g_flmCycDiagResultsValues.flmVHxDiagResults[i].FLM_Read_Diag_VHx_voltage_valid)
                {
                    tmp1VHxVoltageValid |= (1 << i);
                }
            }
            else
            {
                if (g_flmCycDiagResultsValues.flmVHxDiagResults[i].FLM_Read_Diag_VHx_voltage_valid)
                {
                    tmp2VHxVoltageValid |= (1 << (i-8));
                }
            }
        }
        packageToSend.data[32] = tmp1VHxVoltageValid;
        packageToSend.data[33] = tmp2VHxVoltageValid;

        // Squib detection error results
        uint8 tmp1SquibDetErr, tmp2SquibDetErr, tmp3SquibDetErr = 0;
        for (uint8 i = 0; i<20; i++)
        {
            if (i < 8)
            {
                if (g_flmCycDiagResultsValues.flmSquibErrorDiagResults[i])
                {
                    tmp1SquibDetErr |= (1 << i);
                }
            }
            else if (i < 16)
            {
                if (g_flmCycDiagResultsValues.flmSquibErrorDiagResults[i])
                {
                    tmp2SquibDetErr |= (1 << i);
                }
            }
            else
            {
                if (g_flmCycDiagResultsValues.flmSquibErrorDiagResults[i])
                {
                    tmp3SquibDetErr |= (1 << i);
                }
            }
        }
        packageToSend.data[34] = tmp1SquibDetErr;
        packageToSend.data[35] = tmp2SquibDetErr;
        packageToSend.data[36] = tmp3SquibDetErr;

        // Loop resistance diagnostics results
        packageToSend.data[37] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[0].flm_squib_res_value);
        packageToSend.data[38] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[0].flm_squib_res_value);
        packageToSend.data[39] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[1].flm_squib_res_value);
        packageToSend.data[40] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[1].flm_squib_res_value);
        packageToSend.data[41] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[2].flm_squib_res_value);
        packageToSend.data[42] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[2].flm_squib_res_value);
        packageToSend.data[43] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[3].flm_squib_res_value);
        packageToSend.data[44] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[3].flm_squib_res_value);
        packageToSend.data[45] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[4].flm_squib_res_value);
        packageToSend.data[46] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[4].flm_squib_res_value);
        packageToSend.data[47] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[5].flm_squib_res_value);
        packageToSend.data[48] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[5].flm_squib_res_value);
        packageToSend.data[49] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[6].flm_squib_res_value);
        packageToSend.data[50] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[6].flm_squib_res_value);
        packageToSend.data[51] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[7].flm_squib_res_value);
        packageToSend.data[52] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[7].flm_squib_res_value);
        packageToSend.data[53] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[8].flm_squib_res_value);
        packageToSend.data[54] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[8].flm_squib_res_value);
        packageToSend.data[55] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[9].flm_squib_res_value);
        packageToSend.data[56] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[9].flm_squib_res_value);
        packageToSend.data[57] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[10].flm_squib_res_value);
        packageToSend.data[58] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[10].flm_squib_res_value);
        packageToSend.data[59] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[11].flm_squib_res_value);
        packageToSend.data[60] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[11].flm_squib_res_value);
        packageToSend.data[61] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[12].flm_squib_res_value);
        packageToSend.data[62] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[12].flm_squib_res_value);
        packageToSend.data[63] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[13].flm_squib_res_value);
        packageToSend.data[64] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[13].flm_squib_res_value);
        packageToSend.data[65] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[14].flm_squib_res_value);
        packageToSend.data[66] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[14].flm_squib_res_value);
        packageToSend.data[67] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[15].flm_squib_res_value);
        packageToSend.data[68] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[15].flm_squib_res_value);
        packageToSend.data[69] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[16].flm_squib_res_value);
        packageToSend.data[70] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[16].flm_squib_res_value);
        packageToSend.data[71] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[17].flm_squib_res_value);
        packageToSend.data[72] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[17].flm_squib_res_value);
        packageToSend.data[73] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[18].flm_squib_res_value);
        packageToSend.data[74] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[18].flm_squib_res_value);
        packageToSend.data[75] = GetLSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[19].flm_squib_res_value);
        packageToSend.data[76] = GetMSB(g_flmCycDiagResultsValues.flmLoopResDiagResults[19].flm_squib_res_value);

        uint8 tmp1SquibResErr, tmp2SquibResErr, tmp3SquibResErr = 0;
        for (uint8 i = 0; i<20; i++)
        {
            if (i < 8)
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_err)
                {
                    tmp1SquibResErr |= (1 << i);
                }
            }
            else if (i < 16)
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_err)
                {
                    tmp2SquibResErr |= (1 << i);
                }
            }
            else
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_err)
                {
                    tmp3SquibResErr |= (1 << i);
                }
            }
        }
        packageToSend.data[77] = tmp1SquibResErr;
        packageToSend.data[78] = tmp2SquibResErr;
        packageToSend.data[79] = tmp3SquibResErr;

        uint8 tmp1SquibResValid, tmp2SquibResValid, tmp3SquibResValid = 0;
        for (uint8 i = 0; i<20; i++)
        {
            if (i < 8)
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_valid)
                {
                    tmp1SquibResValid |= (1 << i);
                }
            }
            else if (i < 16)
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_valid)
                {
                    tmp2SquibResValid |= (1 << i);
                }
            }
            else
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_valid)
                {
                    tmp3SquibResValid |= (1 << i);
                }
            }
        }
        packageToSend.data[80] = tmp1SquibResValid;
        packageToSend.data[81] = tmp2SquibResValid;
        packageToSend.data[82] = tmp3SquibResValid;

        uint8 tmp1SquibResPgndxLoss, tmp2SquibResPgndxLoss, tmp3SquibResPgndxLoss = 0;
        for (uint8 i = 0; i<20; i++)
        {
            if (i < 8)
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_pgndx_loss)
                {
                    tmp1SquibResPgndxLoss |= (1 << i);
                }
            }
            else if (i < 16)
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_pgndx_loss)
                {
                    tmp2SquibResPgndxLoss |= (1 << i);
                }
            }
            else
            {
                if (g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_pgndx_loss)
                {
                    tmp3SquibResPgndxLoss |= (1 << i);
                }
            }
        }
        packageToSend.data[83] = tmp1SquibResPgndxLoss;
        packageToSend.data[84] = tmp2SquibResPgndxLoss;
        packageToSend.data[85] = tmp3SquibResPgndxLoss;

        // Send data back to MCU
        SendUSBPackage(&packageToSend);
    }

void IntCmdExecuteFLMDiag()
{
    // Initial FLM Diagnostic execution state is initialised as Idle
    // and on later rounds updated from ASIC 
    if (g_FLMDiagExecStatus != FLM_DIAG_EXEC_STATUS_IDLE)
    {
        g_FLMDiagExecStatus = FLMReadDiagExecStatus();
    }

    // Start diagnostic and get out
    // On next entries, check execution status:
    switch (g_flmDiagExecNumber)
    {
    case FLM_DIAG_ORDER_SHORT_DET:
        // check status of diag execution, dont enter any diagnostic if status is ONGOING
        if ((g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)||(g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_IDLE))
        {
            FLMShortDiag();
            // Move on to next diagnostic
            g_flmDiagExecNumber = FLM_DIAG_ORDER_VHX_MEAS;
        }
        break;

    case FLM_DIAG_ORDER_VHX_MEAS: 
        FLMVHxDiag();
        if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
        {
            // Move on to next diagnostic
            g_flmDiagExecNumber = FLM_DIAG_ORDER_LOOP_RES_MEAS;
        }
        break;

    case FLM_DIAG_ORDER_LOOP_RES_MEAS:
        FLMLoopResDiag();
        if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
        {
            // Move on to next diagnostic
            g_flmDiagExecNumber = FLM_DIAG_ORDER_SQUIB_DET;
        }
        break;

    case FLM_DIAG_ORDER_SQUIB_DET:
        FLMSquibDetErrDiag();
        if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
        {
            // Move on to next diagnostic
            g_flmDiagExecNumber = FLM_DIAG_ORDER_SHORT_DET;
        }
        break;

    default:
        break;
    }
}

// Diagnostic is running automatically by default, just read results
void FLMShortDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_SHORT_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_SHORT_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagShortsRegsAddresses[FLM_DIAG_READ_SHORT_REGS_COUNT] = {FLM_FLM_READ_SHORT_CH4_1, FLM_FLM_READ_SHORT_CH8_5,
                                         FLM_FLM_READ_SHORT_CH12_9, FLM_FLM_READ_SHORT_CH16_13, 
                                         FLM_FLM_READ_SHORT_CH20_17};
    
    // read FLM_Read_Short_ch4_1 through FLM_Read_Short_ch20_17, store in FLMShortDiagResults
    g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

    // Read related registers from ASIC
    isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagShortsRegsAddresses, &data[0].dw, &length); //TODO: question: should data argument be &data[0].dw not &data[].dw ?

    // Store results //TODO: check order of data
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch4_1 = data[0].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch8_5 = data[1].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch12_9 = data[2].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch16_13 = data[3].bf.output_data;
    g_flmCycDiagResultsValues.flmShortDiagResults.FLM_Read_Short_ch20_17 = data[4].bf.output_data;

    // results are stored, get back
    g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    
    // TODO: quick result analisys to check for faults
    // if any active, FLMCycDiagFaultsValues.FLM_SC2G_SC2B_fault = TRUE
    return;
}

void FLMVHxDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_VHX_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_VHX_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagVHxRegsAddresses[FLM_DIAG_READ_VHX_REGS_COUNT] =  { FLM_FLM_READ_DIAG_VH1A, FLM_FLM_READ_DIAG_VH2, 
                                                                        FLM_FLM_READ_DIAG_VH3, FLM_FLM_READ_DIAG_VH4, 
                                                                        FLM_FLM_READ_DIAG_VH5, FLM_FLM_READ_DIAG_VH6, 
                                                                        FLM_FLM_READ_DIAG_VH7, FLM_FLM_READ_DIAG_VH8, 
                                                                        FLM_FLM_READ_DIAG_VH9, FLM_FLM_READ_DIAG_VH10,
                                                                        FLM_FLM_READ_DIAG_VH1B };

    if (CheckBatVoltage()==FALSE) 
    {
        // TODO: re-do to support current logic
        //g_FLMCycDiagStatus.flm_VHxMeasStatus = FLM_DIAG_STATUS_VHX_MEAS_SKIPPED;
    }

    // TODO: implement timeout for 1.5-2 duration of diagnostic -> if timeout then feature error to PC 

    if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode
        SetFLMDiagMode(FLM_DIAG_MODE_VHX_MEAS);

        // Start diagnostic
        StartFLMDiag();
        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // TODO: diagnostic was performed, store results
        // Read FLM_FLM_READ_DIAG_VH1A...FLM_FLM_READ_DIAG_VH10, FLM_FLM_READ_DIAG_VH1B
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagVHxRegsAddresses, &data[0].dw, &length);
        // Store results TODO: check order of data
        for (uint8 i = 0; i < FLM_DIAG_READ_VHX_REGS_COUNT ; i++)
        {
            flm_flm_read_diag_vh1a_ut flmReadDiagVHxTmp;
            flmReadDiagVHxTmp.as_uint16 = (data[i].bf.output_data);
            g_flmCycDiagResultsValues.flmVHxDiagResults[i].FLM_Read_Diag_VHx_voltage_valid = flmReadDiagVHxTmp.as_s.FlmVhVoltageValid_u1;
            g_flmCycDiagResultsValues.flmVHxDiagResults[i].FLM_Read_Diag_VHx_voltage_value = flmReadDiagVHxTmp.as_s.FlmVhVoltageValue_u12;
        }

        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    }

    // TODO: evaluate wether to implement error status handling
    // g_FLMCycDiagFaultsValues.FLM_VHxMeasErr_fault = 0; // as per Vasant's diagrams; clearing fault here doesn't seem right for me
    return;
}

void FLMSquibDetErrDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_SQUIB_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_SQUIB_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagSquibRegsAddresses[FLM_DIAG_READ_SQUIB_REGS_COUNT] = {FLM_FLM_READ_SQUIB_CH16_1, FLM_FLM_READ_SQUIB_CH20_17};

    if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode
        SetFLMDiagMode(FLM_DIAG_MODE_SQUIB_DET);

        // Start diagnostic
        StartFLMDiag();
        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // TODO: diagnostic was performed, store results
        // Read FLM_READ_SQUIB_CH16_1, FLM_READ_SQUIB_CH20_17
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagSquibRegsAddresses, &data[0].dw, &length);
        // Store results TODO: check order of data
        for (uint8 i = 0; i < 20 ; i++)
        {
            if (i < 16)
            {
                // Load results of channels 1-16
                g_flmCycDiagResultsValues.flmSquibErrorDiagResults[i] = ((data[0].bf.output_data)&(1<<i));
            }
            else
            {
                // Load results of channels 17-20
                g_flmCycDiagResultsValues.flmSquibErrorDiagResults[i] = ((data[1].bf.output_data)&(1<<(i-16)));
            }
        }

        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    }

    // TODO: evaluate wether to implement error status handling
    return;
}

void FLMLoopResDiag()
{
    SPIReceiveDataNormal data[FLM_DIAG_READ_RES_REGS_COUNT] = {0};
    uint16 length = FLM_DIAG_READ_RES_REGS_COUNT;
    boolean isSuccessfulFlag = FALSE;
    uint16 flmDiagResRegsAddresses[FLM_DIAG_READ_RES_REGS_COUNT] = {FLM_FLM_READ_SQUIB_RES_CH1, FLM_FLM_READ_SQUIB_RES_CH2, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH3, FLM_FLM_READ_SQUIB_RES_CH4, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH5, FLM_FLM_READ_SQUIB_RES_CH6, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH7, FLM_FLM_READ_SQUIB_RES_CH8, 
                                                                    FLM_FLM_READ_SQUIB_RES_CH9, FLM_FLM_READ_SQUIB_RES_CH10,
                                                                    FLM_FLM_READ_SQUIB_RES_CH11,FLM_FLM_READ_SQUIB_RES_CH12,
                                                                    FLM_FLM_READ_SQUIB_RES_CH13,FLM_FLM_READ_SQUIB_RES_CH14,
                                                                    FLM_FLM_READ_SQUIB_RES_CH15,FLM_FLM_READ_SQUIB_RES_CH16,
                                                                    FLM_FLM_READ_SQUIB_RES_CH17,FLM_FLM_READ_SQUIB_RES_CH18,
                                                                    FLM_FLM_READ_SQUIB_RES_CH19,FLM_FLM_READ_SQUIB_RES_CH20,
                                                                    FLM_FLM_READ_SQUIB_RES_SQREF};

    if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_FINISHED)
    {
        // Select corresponding mode
        SetFLMDiagMode(FLM_DIAG_MODE_LOOP_RES_MEAS);

        // Start diagnostic
        StartFLMDiag();
        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_ONGOING;

        // Get back out to check results on next interupt
        return;
    }

    if (g_FLMDiagExecStatus == FLM_DIAG_EXEC_STATUS_EVALUATED)
    {
        // TODO: diagnostic was performed, store results
        // Read FLM_READ_SQUIB_RES_CH1...FLM_READ_SQUIB_RES_CH20
        isSuccessfulFlag = QSPIReadSequenceNormal(SPI1_CS1MASTER, flmDiagResRegsAddresses, &data[0].dw, &length);
        // Store results TODO: check order of data
        for (uint8 i = 0; i < FLM_DIAG_READ_RES_REGS_COUNT ; i++)
        {
            flm_flm_read_squib_res_ch1_ut flmReadSquibResChxTmp; 
            flmReadSquibResChxTmp.as_uint16 = (data[i].bf.output_data);
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_value = flmReadSquibResChxTmp.as_s.FlmSquibResValue_u13;
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_err = flmReadSquibResChxTmp.as_s.FlmSquibResErr_u1;
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_valid = flmReadSquibResChxTmp.as_s.FlmSquibResValid_u1;     
            g_flmCycDiagResultsValues.flmLoopResDiagResults[i].flm_squib_res_pgndx_loss = flmReadSquibResChxTmp.as_s.FlmSquibResPgndxLoss_u1;
        }

        g_FLMDiagExecStatus = FLM_DIAG_EXEC_STATUS_FINISHED;
    }

    // TODO: evaluate wether to implement error status handling
    return;
}

void SetFLMDiagMode(FLMDiagModeEnum diagMode)
{
    SPIReceiveDataNormal data;
    boolean isSuccessfulFlag = TRUE;
    flm_flm_diag_start_ut tmpFLMDiagStartfRegister;
    
    // Get value from ASIC
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, &data.dw);
    tmpFLMDiagStartfRegister.as_uint16 = data.bf.output_data;
    
    // flm_diag_start = 1 starts selected diagnostic
    tmpFLMDiagStartfRegister.as_s.FlmDiagMode_u5 = diagMode;

    // Write
    QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, tmpFLMDiagStartfRegister.as_uint16);
}

void StartFLMDiag(void)
{
    SPIReceiveDataNormal data;
    boolean isSuccessfulFlag = TRUE;
    flm_flm_diag_start_ut tmpFLMDiagStartfRegister;
    
    // Get value from ASIC
    // TODO: check whether diags can be run on slaves (spiChannel selection)
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, &data.dw);
    tmpFLMDiagStartfRegister.as_uint16 = data.bf.output_data;
    
    // flm_diag_start = 1 starts selected diagnostic
    tmpFLMDiagStartfRegister.as_s.FlmDiagStart_u1 = 1;

    // Write
    QSPIWriteNormal(SPI1_CS1MASTER, FLM_FLM_DIAG_START, tmpFLMDiagStartfRegister.as_uint16);
}

flm_DiagExecStatusEnum FLMReadDiagExecStatus(void)
{
    // TODO FLM_FLM_STATUS2
    SPIReceiveDataNormal data;
    boolean isSuccessfulFlag = TRUE;
    flm_flm_status2_ut tmpFLMDiagStatus2fRegister;
    
    // Get value from ASIC
    isSuccessfulFlag &= QSPIReadNormal(SPI1_CS1MASTER, FLM_FLM_STATUS2, &data.dw);
    tmpFLMDiagStatus2fRegister.as_uint16 = data.bf.output_data;
    
    // Determine FLM diagnostic execution status
    if ((tmpFLMDiagStatus2fRegister.as_s.FlmDiagActive_u1 == 1) && (tmpFLMDiagStatus2fRegister.as_s.FlmDiagReady_u1 == 0))
    {
        return FLM_DIAG_EXEC_STATUS_ONGOING;
    }
    else if ((tmpFLMDiagStatus2fRegister.as_s.FlmDiagActive_u1 == 0) && (tmpFLMDiagStatus2fRegister.as_s.FlmDiagReady_u1 == 1))
    {
        return FLM_DIAG_EXEC_STATUS_EVALUATED;
    }
}

void SetFLMDiagExecStatus(flm_DiagExecStatusEnum FLMCycDiagExecStatus)
{
    g_FLMDiagExecStatus = FLMCycDiagExecStatus;
}

flm_DiagExecStatusEnum GetFLMDiagExecStatus(void)
{
    return g_FLMDiagExecStatus;
}

void SetFLMDiagExecOrder (flm_DiagExecOrderEnum execNumber)
    {
        g_flmDiagExecNumber = execNumber;
    }

flm_DiagExecOrderEnum GetFLMDiagExecOrder (void)
    {
        return g_flmDiagExecNumber;
    }

boolean CheckBatVoltage(void)
{
    // TODO
    // Mock for first iteration
    return TRUE;
}
