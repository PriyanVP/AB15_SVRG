/**********************************************************************************************************************
 * \file pwm.h
 * \copyright Copyright (C) RobertBosch GmbH
 *********************************************************************************************************************/

#ifndef PWM_H_
#define PWM_H_

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/


/** \brief Function to initialize the CCU6 module to generate PWM signals
 *
 * \param void
 * \return Returns nothing
 */
void SetDefaultPWMFrequency(void);

/** \brief Function that starts the timer and thus the generation of the PWM signals
 *
 * \param void
 * \return Returns nothing
 */
void StartPWMGeneration(void);

/** \brief Function that switches external PWM to 2MHz, if alredy at 2MHZ no action is taken
 *
 * \param void
 * \return Returns nothing
 */
void SetPWMGeneration2MHZ(void);

/** \brief Function that switches external PWM to 4MHz, if alredy at 2MHZ no action is taken
 *
 * \param void
 * \return Returns nothing
 */
void SetPWMGeneration4MHZ(void);

#endif /* PWM_H_ */
