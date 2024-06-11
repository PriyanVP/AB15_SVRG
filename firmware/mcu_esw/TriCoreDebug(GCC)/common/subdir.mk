################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../common/bit_manipulation.c \
../common/command_queue.c \
../common/crc.c 

C_DEPS += \
./common/bit_manipulation.d \
./common/command_queue.d \
./common/crc.d 

OBJS += \
./common/bit_manipulation.o \
./common/command_queue.o \
./common/crc.o 


# Each subdirectory must supply rules for building sources it contributes
common/%.o: ../common/%.c common/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: AURIX GCC Compiler'
	tricore-elf-gcc -std=c99 "@C:/Entwicklung/AB15_Demo_Board/AB15_dev_Git/ab15_sw/firmware/mcu_esw/TriCoreDebug(GCC)/AURIX_GCC_Compiler-Include_paths__-I_.opt" -O0 -g3 -Wall -c -fmessage-length=0 -fno-common -fstrict-volatile-bitfields -fdata-sections -ffunction-sections -mtc162 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


clean: clean-common

clean-common:
	-$(RM) ./common/bit_manipulation.d ./common/bit_manipulation.o ./common/command_queue.d ./common/command_queue.o ./common/crc.d ./common/crc.o

.PHONY: clean-common

