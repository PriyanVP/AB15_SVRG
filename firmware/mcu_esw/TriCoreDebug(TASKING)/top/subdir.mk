################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
../top/crc_wrapper.c \
../top/spi_wrapper.c \
../top/status.c \
../top/usb_wrapper.c 

COMPILED_SRCS += \
./top/crc_wrapper.src \
./top/spi_wrapper.src \
./top/status.src \
./top/usb_wrapper.src 

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
top/%.src: ../top/%.c top/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING C/C++ Compiler'
	cctc -cs --dep-file="$(basename $@).d" --misrac-version=2004 -D__CPU__=tc37x "-fC:/Entwicklung/AB15_Demo_Board/AB15_dev_Git/ab15_sw/firmware/mcu_esw/TriCoreDebug(TASKING)/TASKING_C_C___Compiler-Include_paths__-I_.opt" --iso=99 --c++14 --language=+volatile --exceptions --anachronisms --fp-model=3 -O0 --tradeoff=4 --compact-max-size=200 -g -Wc-w544 -Wc-w557 -Ctc37x -Y0 -N0 -Z0 -o "$@" "$<" && \
	if [ -f "$(basename $@).d" ]; then sed.exe -r  -e 's/\b(.+\.o)\b/top\/\1/g' -e 's/\\/\//g' -e 's/\/\//\//g' -e 's/"//g' -e 's/([a-zA-Z]:\/)/\L\1/g' -e 's/\d32:/@TARGET_DELIMITER@/g; s/\\\d32/@ESCAPED_SPACE@/g; s/\d32/\\\d32/g; s/@ESCAPED_SPACE@/\\\d32/g; s/@TARGET_DELIMITER@/\d32:/g' "$(basename $@).d" > "$(basename $@).d_sed" && cp "$(basename $@).d_sed" "$(basename $@).d" && rm -f "$(basename $@).d_sed" 2>/dev/null; else echo 'No dependency file to process';fi
	@echo 'Finished building: $<'
	@echo ' '

top/crc_wrapper.o: ./top/crc_wrapper.src top/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

top/spi_wrapper.o: ./top/spi_wrapper.src top/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

top/status.o: ./top/status.src top/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

top/usb_wrapper.o: ./top/usb_wrapper.src top/subdir.mk
	@echo 'Building file: $<'
	@echo 'Invoking: TASKING Assembler'
	astc -Og -Os --no-warnings= --error-limit=42 -o  "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


clean: clean-top

clean-top:
	-$(RM) ./top/crc_wrapper.d ./top/crc_wrapper.o ./top/crc_wrapper.src ./top/spi_wrapper.d ./top/spi_wrapper.o ./top/spi_wrapper.src ./top/status.d ./top/status.o ./top/status.src ./top/usb_wrapper.d ./top/usb_wrapper.o ./top/usb_wrapper.src

.PHONY: clean-top

