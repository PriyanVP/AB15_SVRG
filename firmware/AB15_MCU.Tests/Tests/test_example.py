import os
import pytest
# import serial
from time import sleep
# from fixtures.serial_fixtures import SerialWrapper
import package_helper as pkg

def setup_module(module):
    print('\nsetup_module()')

def teardown_module(module):
    print('\nteardown_module()')

class TestExample:
    ''''''

    @classmethod
    def setup_class(cls):
        cls.some_var = "My VAR"
        print ('\nsetup_class()')

    @classmethod 
    def teardown_class(cls):
        print ('\nteardown_class()')


    def test_one(self):
        '''description 1'''

        # TODO: check if approach works

        # Arrange

        # Act
        print("Test 1")
        print(f"Var: {self.some_var}")

        # Assert
        assert False, "Some arbitrary msg"

    def test_two(self):
        '''description 2'''

        # TODO: check if approach works

        # Arrange

        # Act
        print("Test 2")

        # Assert
        assert True

class TestExample2:
    ''''''

    def test_three(self):
        '''description 3'''

        # TODO: check if approach works

        # Arrange

        # Act
        print("Test 1")

        # Assert
        assert True

    def test_four(self):
        '''description 4'''

        # TODO: check if approach works

        # Arrange

        # Act
        print("Test 2")

        # Assert
        assert True
