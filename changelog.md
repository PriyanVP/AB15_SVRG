# Version 01.00.00 (2024-12-05) 
MVP. Firing feature release.

### **MCU SW (firmware)**:
- Implemented device Routing on SPI1 and SPI2 according to EvalBoard requirements
- SPI Device id 1...11 is now accepted by MCU
- Special handling for sending SPI command to dev id #2 (CS_MON1):
    CS for CS_MON1 is activated by sw directly before SPI communication and SPI Command is sent to CS1_MASTER with HW Chip Select. CS_MON1 is deactivated directly after communication.
- Special handling for sending SPI command to dev id #7 (CS_MON2):
    CS for CS_MON2 is activated by sw directly before SPI communication and SPI Command is sent to CS1_MASTER with HW Chip Select. CS_MON2 is deactivated directly after communication.
- Bugfix in Write Register: correct return value
- Added command for sending raw data
- Implemented FLM cyclic Diagnostics handling (not fully debugged) 
- Added read, write register commands
- Added read, write register sequence commands
- Added commands for executing test modes (not fully debugged)
- Added command for periodic status reading and resetting init mode timer
- Fixed bugs in WD
- Updated CRC3 calculation to match ASIC implementation
- Removed code for AB12
- Added define for bypassing CRC check on MCU (for dev purposes)
- Refactored existing pytests
- Added CRC16, WD, general cmd, flm diagnostics tests

### **PC SW (GUI)**:
#### **UI**:
- Added firing page with tabs
#### **Backend**
- Added firing backend
- Added ASIC model (handling ASIC level features)
- Added ASIC wrapper model (handling multi-ASIC features)
- Added mechanism to collect and write configuration to ASIC
- Removed unused code and fixed bugs in WD VM
- Added command tracer feature (logs in and out communication on COM port)
- Refactored logger to avoid threading issues and to allow using mocks
- Removed AB12 code
- Added models for all main configuration registers
- Refactored Register interface, updated template for generation
- Fixed bug in register models generator for enumerations
- Updated payload models
- Added method for calculating CRC16

#### **Other**
- Updated naming convention
- Fixed issue in units for WD response and lock times


# Version 00.03.00 (2024-09-19) 
Watchdog feature release.

### **MCU SW (firmware)**:
- Implemented firmware testing framework
- Implemented device routing on SPI1
- Fixed bug in watchdog state transition

### **PC SW (GUI)**:
#### **UI**:
- Refactored approach for locking buttons
#### **Backend**
- Added WD ViewModel tests
- Refactored multithreading for logs
- Refactored approach for locking buttons. Improved performance


# Version 00.02.00 (2024-08-12) 
Watchdog feature release.

### **MCU SW (firmware)**:
- Implemented WD functionality for WD1 and WD2 (WD2 and WD3 in AB12)
- Updated timer to generate interrupts required for WD functionality
- Added gloabl_defines.h file with define of currently used HW platform
- Updated NamingConvention for C (const variables naming). Fixed naming violations
- Implemented CRC3
- Implemented SPI top level functionality
- Updated spi data types to be more verbose
- Updated Get device id command
- Implemented generic CmdSpiIntruction command
- Added ECLK implementation (configurable - 2/4 MHz)
- Tested ECLK implementation
- Refactored WD commands (more verbose output)

### **PC SW (GUI)**:
#### **UI**:
- Added top level WD status indicator
- Added tooltips for errors
- Added tooltips for help messages
- Implemented switch for help messages display
- Implemented visualization of errors on UI
- Fixed bugs
- Created WD slider control and added it to the project
- Refactored light and dark themes
#### **Backend**
- Implemented package handling for WD status data
- Implemented package handling for WD
- Added unit tests
- Implemented state machine for handling WD workflow
- Implemented commands for WD configuration
- Added MCU debug configuration for Oleksii
- Added OSS state machine framework
- Added generic approach for help messages storing
- Added approach for buttons locking (avoid double command sending)
- Implemented register abstraction on GUI
- Updated View to ViewModel binding to be more universal
- Added conditional define to support simultaneous code development for AB12 and AB15
- Refactored approach to handliing absent response from MCU
- Added methods to do common bit operations
- Added generic payload handler for address-data payload


# Version 00.01.05 (2024-06-24) 
Initial changelog update.

### **MCU SW (firmware)**:
- transfered frimware from MiToCoTA project
- applied initial modifications to allow operations with AB12
- updated version.h , header and comments 
- removed unneeded strings in USB_CMD_GET_MCU_VERSION:  V0.1.5 --> 015
- removed unneeded strings in USB_CMD_GET_MCU_BUILD_DATE: yyyy-mm-dd  --> yyyymmdd
- removed unneeded strings in USB_CMD_GET_MCU_BUILD_TIME:  Thh:mm --> hhmm
- removed comments from functions in general commands.c --> shall be at prototyped

### **PC SW (GUI)**:
- added gitignore, gitattributes files
- added changelog
- added README, NamingConvention
- set up general folder structure
- implemented Logger
- added test projects and tests
