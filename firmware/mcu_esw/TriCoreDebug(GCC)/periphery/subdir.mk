################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../periphery/button.c \
../periphery/control_pins.c \
../periphery/gpio.c \
../periphery/led.c \
../periphery/spi.c \
../periphery/timer.c \
../periphery/usb.c 

C_DEPS += \
./periphery/button.d \
./periphery/control_pins.d \
./periphery/gpio.d \
./periphery/led.d \
./periphery/spi.d \
./periphery/timer.d \
./periphery/usb.d 

OBJS += \
./periphery/button.o \
./periphery/control_pins.o \
./periphery/gpio.o \
./periphery/led.o \
./periphery/spi.o \
./periphery/timer.o \
./periphery/usb.o 


# Each subdirectory must supply rules for building sources it contributes
periphery/%.o: ../periphery/%.c periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: AURIX GCC Compiler'
	tricore-elf-gcc -std=c99 "@C:/Entwicklung/AB15_Demo_Board/AB15_dev_Git/ab15_sw/firmware/mcu_esw/TriCoreDebug(GCC)/AURIX_GCC_Compiler-Include_paths__-I_.opt" -O0 -g3 -Wall -c -fmessage-length=0 -fno-common -fstrict-volatile-bitfields -fdata-sections -ffunction-sections -mtc162 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


clean: clean-periphery

clean-periphery:
	-$(RM) ./periphery/button.d ./periphery/button.o ./periphery/control_pins.d ./periphery/control_pins.o ./periphery/gpio.d ./periphery/gpio.o ./periphery/led.d ./periphery/led.o ./periphery/spi.d ./periphery/spi.o ./periphery/timer.d ./periphery/timer.o ./periphery/usb.d ./periphery/usb.o

.PHONY: clean-periphery

