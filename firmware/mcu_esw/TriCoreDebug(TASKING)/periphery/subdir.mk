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

COMPILED_SRCS += \
./periphery/button.src \
./periphery/control_pins.src \
./periphery/gpio.src \
./periphery/led.src \
./periphery/spi.src \
./periphery/timer.src \
./periphery/usb.src 

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
periphery/%.src: ../periphery/%.c periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING C/C++ Compiler'
	cctc -cs --dep-file="$(basename $@).d" --misrac-version=2004 -D__CPU__=tc37x "-fC:/Entwicklung/AB15_Demo_Board/AB15_dev_Git/ab15_sw/firmware/mcu_esw/TriCoreDebug(TASKING)/TASKING_C_C___Compiler-Include_paths__-I_.opt" --iso=99 --c++14 --language=+volatile --exceptions --anachronisms --fp-model=3 -O0 --tradeoff=4 --compact-max-size=200 -g -Wc-w544 -Wc-w557 -Ctc37x -Y0 -N0 -Z0 -o "$@" "$<" && \
	if [ -f "$(basename $@).d" ]; then sed.exe -r  -e 's/\b(.+\.o)\b/periphery\/\1/g' -e 's/\\/\//g' -e 's/\/\//\//g' -e 's/"//g' -e 's/([a-zA-Z]:\/)/\L\1/g' -e 's/\d32:/@TARGET_DELIMITER@/g; s/\\\d32/@ESCAPED_SPACE@/g; s/\d32/\\\d32/g; s/@ESCAPED_SPACE@/\\\d32/g; s/@TARGET_DELIMITER@/\d32:/g' "$(basename $@).d" > "$(basename $@).d_sed" && cp "$(basename $@).d_sed" "$(basename $@).d" && rm -f "$(basename $@).d_sed" 2>/dev/null; else echo 'No dependency file to process';fi
	@echo 'Finished building: $<'
	@echo ' '

periphery/button.o: ./periphery/button.src periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

periphery/control_pins.o: ./periphery/control_pins.src periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

periphery/gpio.o: ./periphery/gpio.src periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

periphery/led.o: ./periphery/led.src periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

periphery/spi.o: ./periphery/spi.src periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

periphery/timer.o: ./periphery/timer.src periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

periphery/usb.o: ./periphery/usb.src periphery/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


clean: clean-periphery

clean-periphery:
	-$(RM) ./periphery/button.d ./periphery/button.o ./periphery/button.src ./periphery/control_pins.d ./periphery/control_pins.o ./periphery/control_pins.src ./periphery/gpio.d ./periphery/gpio.o ./periphery/gpio.src ./periphery/led.d ./periphery/led.o ./periphery/led.src ./periphery/spi.d ./periphery/spi.o ./periphery/spi.src ./periphery/timer.d ./periphery/timer.o ./periphery/timer.src ./periphery/usb.d ./periphery/usb.o ./periphery/usb.src

.PHONY: clean-periphery

