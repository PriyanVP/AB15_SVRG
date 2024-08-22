from pytest_metadata.plugin import metadata_key 
import pytest
from os import environ
from py.xml import html

def pytest_html_report_title(report):  
    report.title = "Firmware HTML Testing Report"

def pytest_configure(config):  
    config.stash[metadata_key]["Project"] = "AB15 SW"
    config.stash[metadata_key]["Tester"] = environ.get('USERNAME')

@pytest.hookimpl(optionalhook=True)
def pytest_html_results_table_header(cells):
    # Add new column header in HTML report
    cells.insert(2, html.th('Test description'))

@pytest.hookimpl(optionalhook=True)
def pytest_html_results_table_row(report, cells):
    #  Add description to cell
    description = getattr(report, "test_description", "")
    cells.insert(2, html.td(description))

@pytest.hookimpl(hookwrapper=True)
def pytest_runtest_makereport(item, call):
    outcome = yield
    outcome._result.test_description = item.function.__doc__

# TODO: add hook for HTML to display docstring

# # Approach for adding images
# import base64  
# import os  
# import pytest  
# import pytest_html  
# from pytest_metadata.plugin import metadata_key  
   
# @pytest.hookimpl(hookwrapper=True)  
# def pytest_runtest_makereport(item):  
#     outcome = yield  
#     report = outcome.get_result()  
#     extra = getattr(report, "extra", [])  
#     if report.when == "call":  
#         # Assuming your screenshot is saved correctly at the specified path  
#         screenshot_path = os.getenv("SCREENSHOT_PATH", "./test_screenshot.png")  
#         with open(screenshot_path, "rb") as image_file:  
#             encoded_string = base64.b64encode(  
#                 image_file.read()  
#             ).decode()  # https://github.com/pytest-dev/pytest-html/issues/265  
#         extra.append(pytest_html.extras.png(encoded_string))  
#         report.extra = extra