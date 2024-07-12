/**********************************************************************************************************************
 * \file control_pins.h
 * \copyright Copyright (C) RobertBosch GmbH e SELECT pin as input
 * Should be executed after POR
 * \return Returns nothing
 */
void ConfigureSelectPin(void);

/** \brief Check if requested to bypass MCU via SELECT pin or via command from GUI
 * \return Returns true if MCU should be bypassed, false - otherwise
 */
boolean IsMCUBypassSelected(void);

#endif /* CONTROL_PINS_H_ */
