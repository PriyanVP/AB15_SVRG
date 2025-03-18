/**********************************************************************************************************************
 * \file Ifx_IntPrioDef.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef IFX_INTPRIODEF_H_
#define IFX_INTPRIODEF_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/* Interrupt Service Routine priorities for general purpose timers */
#define ISR_PRIORITY_GPT1_T4_TIMER    (5)             /** \brief Priority for GPT12 Timer T4 interrupt               */
#define ISR_PRIORITY_GPT1_T3_TIMER    (6)             /** \brief Priority for GPT12 Timer T3 interrupt               */
#define ISR_PRIORITY_GPT1_T2_TIMER    (7)             /** \brief Priority for GPT12 Timer T2 interrupt               */

/* Interrupt Service Routine priorities for USB communication */
#define ISR_PRIORITY_ASCLIN_TX        (8)             /** \brief Priority for interrupt ISR Transmit                 */
#define ISR_PRIORITY_ASCLIN_RX        (4)             /** \brief Priority for interrupt ISR Receive                  */
#define ISR_PRIORITY_ASCLIN_ER        (12)            /** \brief Priority for interrupt ISR Errors                   */

/* Interrupt Service Routine priorities for UART communication */
#define ISR_PRIORITY_UART_TX          (40)            /** \brief Priority for interrupt UART ISR Transmit            */

/* Interrupt Service Routine priorities for Master and Slave SPI communication */
#define ISR_PRIORITY_MASTER1_TX        (50)            /** \brief Priority for interrupt SPI ISR Transmit             */
#define ISR_PRIORITY_MASTER1_RX        (51)            /** \brief Priority for interrupt SPI ISR Receive              */
#define ISR_PRIORITY_MASTER1_ER        (52)            /** \brief Priority for interrupt SPI ISR Error                */

/* Interrupt Service Routine priorities for Master and Slave SPI communication */
#define ISR_PRIORITY_MASTER2_TX        (53)            /** \brief Priority for interrupt SPI ISR Transmit             */
#define ISR_PRIORITY_MASTER2_RX        (54)            /** \brief Priority for interrupt SPI ISR Receive              */
#define ISR_PRIORITY_MASTER2_ER        (55)            /** \brief Priority for interrupt SPI ISR Error                */

#endif /* IFX_INTPRIODEF_H_ */
