################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../cmd/bypass_cmd.c \
../cmd/cont_read_cmd.c \
../cmd/error_check.c \
../cmd/general_cmd.c \
../cmd/gpio_cmd.c \
../cmd/seq_cmd.c \
../cmd/watchdog.c 

C_DEPS += \
./cmd/bypass_cmd.d \
./cmd/cont_read_cmd.d \
./cmd/error_check.d \
./cmd/general_cmd.d \
./cmd/gpio_cmd.d \
./cmd/seq_cmd.d \
./cmd/watchdog.d 

OBJS += \
./cmd/bypass_cmd.o \
./cmd/cont_read_cmd.o \
./cmd/error_check.o \
./cmd/general_cmd.o \
./cmd/gpio_cmd.o \
./cmd/seq_cmd.o \
./cmd/watchdog.o 


# Each subdirectory must supply rules for building sources it contributes
cmd/%.o: ../cmd/%.c cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: AURIX GCC Compiler'
	tricore-elf-gcc -std=c99 "@C:/Entwicklung/AB15_Demo_Board/AB15_dev_Git/ab15_sw/firmware/mcu_esw/TriCoreDebug(GCC)/AURIX_GCC_Compiler-Include_paths__-I_.opt" -O0 -g3 -Wall -c -fmessage-length=0 -fno-common -fstrict-volatile-bitfields -fdata-sections -ffunction-sections -mtc162 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


clean: clean-cmd

clean-cmd:
	-$(RM) ./cmd/bypass_cmd.d ./cmd/bypass_cmd.o ./cmd/cont_read_cmd.d ./cmd/cont_read_cmd.o ./cmd/error_check.d ./cmd/error_check.o ./cmd/general_cmd.d ./cmd/general_cmd.o ./cmd/gpio_cmd.d ./cmd/gpio_cmd.o ./cmd/seq_cmd.d ./cmd/seq_cmd.o ./cmd/watchdog.d ./cmd/watchdog.o

.PHONY: clean-cmd

