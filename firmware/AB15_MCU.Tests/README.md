# Serial Communication Test Framework

## Overview

This project provides a test framework for sending and receiving messages via a COM port using PyTest and PySerial.

## Setup

1. Install dependencies for running tests:
    ```bash
    pip install -r requirements.txt
    ```

2. Configure your COM port in `Tests/fixtures/serial_fixtures.py`.
3. HW setup:
    1) ShieldBuddy is powered up and connected to PC;
    2) AB15(12) is powered and connected to ShieldBuddy;
    3) Adequate firmware is flashed into ShieldBuddy.

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