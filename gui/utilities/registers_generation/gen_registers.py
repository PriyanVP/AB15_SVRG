import sys, os
from jinja2 import Environment, FileSystemLoader
import xml.etree.ElementTree as et
import argparse
from lxml import etree

# Color sequences for colored output
class ColorSequences:
    '''Class to hold code sequences for formatting and coloring output'''
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'


# env = Environment(loader=FileSystemLoader("."))
# templ = env.get_template("Register_Template.txt")

# liverpool = {"register_name": "Liverpool"}
# print(templ.render(liverpool))


# Plan:
# 1. Command line arguments parser: path to xml, path to gui root, clear output dir flag, regenerate all
# 2. File parser - should return list with all registers used in .cs files
# 3. Genereted items - function to return list of available registers in output directory
# 4. List for genereation: returns list with register names that should be generated
# 5. XML loader: load xml content to variable (maybe remove unused portion)
# 6. Template configuration generator: create config in suitable fot template format by register name
# 7. Template render
# 8. Sava data to file: new class for register access is created

def command_line_parser():
    '''Configure command line options and return parsed data'''

    # Configure parser for command line arguments
    parser = argparse.ArgumentParser(description="Generate classes for working with registers")

    # Add options to clear output directory and to force all classes regeneration
    parser.add_argument("-c", "--clear", action="store_true", help="clear output directory")
    parser.add_argument("-r", "--regenerate_all", action="store_true", help="regenerate all items even if they are already generated previously")

    # Add path to file/folder options
    parser.add_argument("-xp", "--xml_path", type=str, help="Path to xml file with regmap")
    parser.add_argument("-dp", "--dir_path", type=str, help="Path to GUI project root")

    # Parse command line arguments
    return parser.parse_args()



def main():
    '''Main function of script. All code execution starts from here'''

    # Change working directory to script directory
    os.chdir(os.path.dirname(__file__))
    
    # Pathes initialization (default options)
    path_gui_dir = "..\\..\\AB15_GUI.WPF"
    path_xml_regmap = "..\\..\\..\\resources\\AB15\\tap_AddrMap_10.2.xml"

    # Parse command line arguments
    args = command_line_parser()

    # Modify pathes if command line options were provided
    if (args.xml_path is not None):
        path_xml_regmap = args.xml_path

    if (args.dir_path is not None):
        path_gui_dir = args.dir_path

    # Construct path to output directory
    path_output_dir = os.path.join(path_gui_dir, "Models\\Generated\\Registers")

    #







# Program entry point
if __name__ == "__main__":
    main()