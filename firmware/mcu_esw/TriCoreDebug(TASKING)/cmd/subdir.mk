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

COMPILED_SRCS += \
./cmd/bypass_cmd.src \
./cmd/cont_read_cmd.src \
./cmd/error_check.src \
./cmd/general_cmd.src \
./cmd/gpio_cmd.src \
./cmd/seq_cmd.src \
./cmd/watchdog.src 

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
cmd/%.src: ../cmd/%.c cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING C/C++ Compiler'
	cctc -cs --dep-file="$(basename $@).d" --misrac-version=2004 -D__CPU__=tc37x "-fC:/Entwicklung/AB15_Demo_Board/AB15_dev_Git/ab15_sw/firmware/mcu_esw/TriCoreDebug(TASKING)/TASKING_C_C___Compiler-Include_paths__-I_.opt" --iso=99 --c++14 --language=+volatile --exceptions --anachronisms --fp-model=3 -O0 --tradeoff=4 --compact-max-size=200 -g -Wc-w544 -Wc-w557 -Ctc37x -Y0 -N0 -Z0 -o "$@" "$<" && \
	if [ -f "$(basename $@).d" ]; then sed.exe -r  -e 's/\b(.+\.o)\b/cmd\/\1/g' -e 's/\\/\//g' -e 's/\/\//\//g' -e 's/"//g' -e 's/([a-zA-Z]:\/)/\L\1/g' -e 's/\d32:/@TARGET_DELIMITER@/g; s/\\\d32/@ESCAPED_SPACE@/g; s/\d32/\\\d32/g; s/@ESCAPED_SPACE@/\\\d32/g; s/@TARGET_DELIMITER@/\d32:/g' "$(basename $@).d" > "$(basename $@).d_sed" && cp "$(basename $@).d_sed" "$(basename $@).d" && rm -f "$(basename $@).d_sed" 2>/dev/null; else echo 'No dependency file to process';fi
	@echo 'Finished building: $<'
	@echo ' '

cmd/bypass_cmd.o: ./cmd/bypass_cmd.src cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

cmd/cont_read_cmd.o: ./cmd/cont_read_cmd.src cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

cmd/error_check.o: ./cmd/error_check.src cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

cmd/general_cmd.o: ./cmd/general_cmd.src cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

cmd/gpio_cmd.o: ./cmd/gpio_cmd.src cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

cmd/seq_cmd.o: ./cmd/seq_cmd.src cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

cmd/watchdog.o: ./cmd/watchdog.src cmd/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


clean: clean-cmd

clean-cmd:
	-$(RM) ./cmd/bypass_cmd.d ./cmd/bypass_cmd.o ./cmd/bypass_cmd.src ./cmd/cont_read_cmd.d ./cmd/cont_read_cmd.o ./cmd/cont_read_cmd.src ./cmd/error_check.d ./cmd/error_check.o ./cmd/error_check.src ./cmd/general_cmd.d ./cmd/general_cmd.o ./cmd/general_cmd.src ./cmd/gpio_cmd.d ./cmd/gpio_cmd.o ./cmd/gpio_cmd.src ./cmd/seq_cmd.d ./cmd/seq_cmd.o ./cmd/seq_cmd.src ./cmd/watchdog.d ./cmd/watchdog.o ./cmd/watchdog.src

.PHONY: clean-cmd

