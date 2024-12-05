# Serial Communication Test Framework

## Overview

This project provides a PyTest-based test environment for Hardware-in-the-Loop testing for MCU firmware by sending and receiving messages via COM port using PyTest and PySerial. Tests are separated into files based on the area of tested
functionality (Watchdog tests etc.) and are marked with custom markers (see pytest.ini for markers description).
Tests can be ran altogether, separately, or by marker (see Running test paragraph).

## Setup

1. Install dependencies for running tests:
    ```bash
    pip install -r environment_dependensies.txt
    ```

2. Configure your ShieldBuddy COM port name in `Tests/fixtures/serial_fixtures.py`.
3. HW setup:
    1) ShieldBuddy is powered up and connected to PC;
    2) AB15(12) is powered and connected to ShieldBuddy;
    3) Adequate firmware is flashed into ShieldBuddy.
4. Change working directory to .\firmware\AB15_MCU.Tests\Tests
5. (Optional) Configure VS code for pytest integration:
    1) Install the VS Code Python extension from Microsoft (search for 'python' in Extensions);
    2) Open any pytest test file; Testing icon should appear on Activity bar;
    3) Tests now can be run from graphic interface of Test Explorer

## Running Tests

- To run all tests:
    ```bash
    pytest
    ```

- To run tests from specific file:
    ```bash
    pytest <file_name>
    ```

- To run tests with a specific marker (e.g., serial tests):
    ```bash
    pytest -m serial
    ```

## Test Reports

Test results are generated in HTML, XML, and JSON formats.