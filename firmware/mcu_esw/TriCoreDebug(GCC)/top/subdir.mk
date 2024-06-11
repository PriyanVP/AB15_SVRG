################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../top/crc_wrapper.c \
../top/spi_wrapper.c \
../top/status.c \
../top/usb_wrapper.c 

C_DEPS += \
./top/crc_wrapper.d \
./top/spi_wrapper.d \
./top/status.d \
./top/usb_wrapper.d 

OBJS += \
./top/crc_wrapper.o \
./top/spi_wrapper.o \
./top/status.o \
./top/usb_wrapper.o 


# Each subdirectory must supply rules for building sources it contributes
top/%.o: ../top/%.c top/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: AURIX GCC Compiler'
	tricore-elf-gcc -std=c99 "@C:/Entwicklung/AB15_Demo_Board/AB15_dev_Git/ab15_sw/firmware/mcu_esw/TriCoreDebug(GCC)/AURIX_GCC_Compiler-Include_paths__-I_.opt" -O0 -g3 -Wall -c -fmessage-length=0 -fno-common -fstrict-volatile-bitfields -fdata-sections -ffunction-sections -mtc162 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


clean: clean-top

clean-top:
	-$(RM) ./top/crc_wrapper.d ./top/crc_wrapper.o ./top/spi_wrapper.d ./top/spi_wrapper.o ./top/status.d ./top/status.o ./top/usb_wrapper.d ./top/usb_wrapper.o

.PHONY: clean-top

