/**********************************************************************************************************************
 * \file gpio.h
 * \copyright Copyright (C) RobertBosch GmbH 
 *********************************************************************************************************************/

#ifndef GPIO_H_
#define GPIO_H_

/*********************************************************************************************************************/
/*-----------------------------------------------------Includes------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*------------------------------------------------------Macros-------------------------------------------------------*/
/*********************************************************************************************************************/

/*********************************************************************************************************************/
/*--------------------------------------------------Enumerations-----------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Defines pin indexes for CS600 GPIO
 */
typedef enum
{
    CS600_GPIO0 = 0,             /** \brief GPIO pin 0 at CS600 */
    CS600_GPIO1 = 1,             /** \brief GPIO pin 1 at CS600 */
    CS600_GPIO2 = 2,             /** \brief GPIO pin 2 at CS600 */
    CS600_GPIO3 = 3,             /** \brief GPIO pin 3 at CS600 */
    CS600_GPIO4 = 4,             /** \brief GPIO pin 4 at CS600 */
    CS600_GPIO5 = 5,             /** \brief GPIO pin 5 at CS600 */
    CS600_GPIO6 = 6              /** \brief GPIO pin 6 at CS600 */
} GPIOPinsEnum;


#define SPI1_CS_MON1_PIN                         &MODULE_P10,4                          /*  Port Pin                         */
#define SPI2_CS_MON2_PIN                         &MODULE_P15,2                          /*  Port Pin                         */

/*********************************************************************************************************************/
/*------------------------------------------------Function Prototypes------------------------------------------------*/
/*********************************************************************************************************************/

/** \brief Default configuration of GPIO pins (after POR of MCU)
 * Should be executed after POR
 * \return Returns nothing
 */
void InitGPIOPins(void);

/** \brief Configure GPIO pin
 * Can change mode (input/output) and type (push-pull, pull-up, pull-down, etc.)
 * \param pinIdx GPIO pin index as enum value
 * \param mode new mode of pin
 * \return Returns nothing
 */
void ConfigureGPIOPin(GPIOPinsEnum pinIdx, IfxPort_Mode mode);

/** \brief Activate GPIO pin (set pin to high level)
 * Should be configured as output first!
 * \param pinIdx GPIO pin index as enum value
 * \return Returns nothing
 */
void SetGPIOPin(GPIOPinsEnum pinIdx);

/** \brief Deactivate GPIO pin (set pin to low level)
 * Should be configured as output first!
 * \param pinIdx GPIO pin index as enum value
 * \return Returns nothing
 */
void ClearGPIOPin(GPIOPinsEnum pinIdx);

/** \brief Toggle GPIO pin (set pin to opposite level)
 * Should be configured as output first!
 * \param pinIdx GPIO pin index as enum value
 * \return Returns nothing
 */
void ToggleGPIOPin(GPIOPinsEnum pinIdx);

/** \brief Get GPIO pin state
 * Can be used for both input and output pins
 * \param pinIdx GPIO pin index as enum value
 * \return Returns true if pin is in active state (high), false if in deactivated state (low)
 */
boolean GetGPIOPin(GPIOPinsEnum pinIdx);

/** \brief Get all GPIO pins states
 * 0th bit - GPIO0, ..., 6th bit - GPIO6, 7th bit - unused
 *
 * \return Returns all GPIO pins state as variable
 */
uint8 GetGPIOPort(void);

/** \brief Set all GPIO pins states
 * 0th bit - GPIO0, ..., 6th bit - GPIO6, 7th bit - unused
 */
void SetGPIOPort(uint8 portState);

#endif /* GPIO_H_ */
