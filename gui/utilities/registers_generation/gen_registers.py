import sys, os
from jinja2 import Environment, FileSystemLoader
import argparse
from lxml import etree
import re

def command_line_parser() -> argparse.Namespace:
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

def list_source_files(path_gui_dir: str) -> list:
    '''Create list of source files in GUI project that may contain register usage'''

    # Variable to store result
    output_list = []

    # List of ignored directories
    ignored_dirs = ["bin", "obj", "Registers"]

    # Walk through all directories in GUI project and store list of source files that may contain register usage
    for root, dirs, files in os.walk(path_gui_dir):
        for file in files:
            if file.endswith(".cs") and not any(ignored_dir in root for ignored_dir in ignored_dirs) :
                output_list.append(os.path.join(root, file))

    return output_list

def list_generated_registers(path_output_dir: str) -> list:
    '''Create list of already generated registers in GUI project'''

    # Prefix of generated register files
    prefix = "Reg_"

    # Variable to store result
    output_list = []

    # Walk through generated files directory
    for root, dirs, files in os.walk(path_output_dir):
        for file in files:
            if file.endswith(".cs"):
                output_list.append(file.removeprefix(prefix))

    return output_list

def clear_output_directory(path_output_dir: str):
    '''Removes all files from output directory'''

    # Get list of files in output directory
    filelist = [ f for f in os.listdir(path_output_dir) ]

    # Delete files
    for file in filelist:
        os.remove(os.path.join(path_output_dir, file))

def list_used_registers(sorce_files: list) -> list:
    '''Gets list of registers used in GUI project.
    Used registers are determined by pattern matching. No dublicates in output'''

    # Output list
    registers = []

    # Pattern for register
    pattern = r'Reg_(\w+?)(?=\s|\()'

    # Delete files
    for file_path in sorce_files:
        # Variable to store file content
        file_content = ""
        # Open file and read its content line by line
        with open(file_path, 'r') as file:
            # Read file content to variable
            file_content = file.read()

        # Check if registers were found in file  
        matches = re.findall(pattern, file_content)

        # Append only new registers to list
        for match in matches:
            if match not in registers:
                registers.append(match)

    return registers

def generate_register_files(path_to_regmap: str, register_list: list, path_to_output_dir: str):
    '''Generate files for register handling in GUI based on XML regmap'''

    # Load XML regmap
    regmap_tree = etree.parse(path_to_regmap)
    regmap_root = regmap_tree.getroot()

    # Load namespaces
    xml_spirit_namespaces = regmap_root.nsmap

    # Get reference to memory map tag
    memory_map_root = regmap_root.find(".//spirit:memoryMap", namespaces=xml_spirit_namespaces)

    # # Dictionary format
    # ref_dict = {
    #     "registers" : [
    #         {
    #             "register_name" : "string",
    #             "reset_value" : 0,
    #             "register_address" : 0,
    #             "register_description" : "string",
    #             "register_access" : "string",
    #             "fields" : [
    #                 {
    #                     "name" : "string",
    #                     "description" : "string",
    #                     "access" : "string",
    #                     "bit_offset" : 0,
    #                     "bit_width" : 0,
    #                     "enumerated_values" : [
    #                         {
    #                             "key" : "string",
    #                             "value" : 0
    #                         },
    #                         {
    #                             # ...
    #                         }
    #                     ]
    #                 },
    #                 {
    #                     # ...
    #                 }
    #             ]
    #         },
    #         {
    #             # ...
    #         }
    #     ]
    # }

    # Dictionary with configuration for template for code generation
    template_parameters_list = {"registers" : []}

    # Loop through all registers in memory map
    for register in memory_map_root.iter(f"{{{xml_spirit_namespaces['spirit']}}}register"):

        # Get register name
        reg_name = register.find(".//spirit:name", namespaces=xml_spirit_namespaces).text
    
        # Stop processing if generation for this register is not requested
        if (reg_name not in register_list):
            continue

        # Get address offset of address block (base address)
        base_address = int(register.getparent().find(".//spirit:baseAddress", namespaces=xml_spirit_namespaces).text, 16)

        # Get register offset in address block
        reg_offset = int(register.find(".//spirit:addressOffset", namespaces=xml_spirit_namespaces).text, 16)

        # Calculate absolute address and store as a hex string
        abs_address = f"0x{(base_address+reg_offset):04x}"

        # Get reset value. Handle reset field without mask
        try:
            reset_value = int(register.find(".//spirit:reset", namespaces=xml_spirit_namespaces) \
                                .find(".//spirit:value", namespaces=xml_spirit_namespaces).text, 16)

            reset_mask = int(register.find(".//spirit:reset", namespaces=xml_spirit_namespaces) \
                                    .find(".//spirit:mask", namespaces=xml_spirit_namespaces).text, 16)

            reset_value = f"0x{(reset_value & reset_mask):04x}"
        except AttributeError:
            reset_value = "0x0000"
            
        # Get description
        reg_description = register.find(".//spirit:description", namespaces=xml_spirit_namespaces).text

        # Handle description
        reg_description = reg_description.replace('"', "'")     # " -> '
        reg_description = reg_description.replace('\n', " ")    # \n -> space
        reg_description = reg_description.replace('\t', " ")    # \t -> space

        # Get fields data
        fields_parameters = []
        for field in register.iter(f"{{{xml_spirit_namespaces['spirit']}}}field"):
            # Get field data
            name = field.find(".//spirit:name", namespaces=xml_spirit_namespaces).text
            description = field.find(".//spirit:description", namespaces=xml_spirit_namespaces).text
            access = field.find(".//spirit:access", namespaces=xml_spirit_namespaces).text
            bit_offset = field.find(".//spirit:bitOffset", namespaces=xml_spirit_namespaces).text
            bit_width = field.find(".//spirit:bitWidth", namespaces=xml_spirit_namespaces).text

            # Handle description
            description = description.replace('"', "'")     # " -> '
            description = description.replace('\n', " ")    # \n -> space
            description = description.replace('\t', " ")    # \t -> space

            # Get enumerated values
            enumerated_values_tag = field.find(".//spirit:enumeratedValues", namespaces=xml_spirit_namespaces)

            enumerated_values = []
            if (enumerated_values_tag is not None):
                # Loop through enum values and store them to dictionary
                for enum_value in enumerated_values_tag.iter(f"{{{xml_spirit_namespaces['spirit']}}}enumeratedValue"):
                    key = enum_value.find(".//spirit:name", namespaces=xml_spirit_namespaces).text
                    value = enum_value.find(".//spirit:value", namespaces=xml_spirit_namespaces).text
                    enumerated_values.append({"key" : key, "value" : value})

            # Store data to dictionary
            field_dict = {
                "name" : name,
                "description" : description,
                "access" : access,
                "bit_offset" : bit_offset,
                "bit_width" : bit_width,
            }

            # If enumerated values exist, add them to dictionary
            if enumerated_values:
                field_dict["enumerated_values"] = enumerated_values

            # Store data to list
            fields_parameters.append(field_dict)

        # Get access for register
        # Priority: read-write > read-only
        access = "read-only"
        for field in fields_parameters:
            if (field["access"] == "read-write"):
                access = "read-write"
                break

        # Store data to dictionary
        register_parameters = {
            "register_name" : reg_name,
            "reset_value" : reset_value,
            "register_address" : abs_address,
            "register_description" : reg_description,
            "register_access" : access,
            "fields" : fields_parameters
        }

        # Append to output list
        template_parameters_list["registers"].append(register_parameters)

    # Generate files
    environment = Environment(loader=FileSystemLoader("."))
    templ = environment.get_template("Register_Template.txt")

    for register_parameters in template_parameters_list["registers"]:
        # Content of the file
        file_content = templ.render(register_parameters)

        # Path to file that will be generated
        generated_file_path = os.path.join(path_to_output_dir, f"Reg_{register_parameters['register_name']}.cs")

        # Write content to file
        with open(generated_file_path, 'w') as file:
            file.write(file_content)
            
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

    # Clear directory with generated register files if requested
    if (args.clear):
        clear_output_directory(path_output_dir)

    # Generate list of source files in GUI project (only .cs)
    source_files = list_source_files(path_gui_dir)

    # Generate list of register types used in GUI project
    full_register_list = list_used_registers(source_files)

    # List of already generated registers
    generated_registers = []
    if (not args.regenerate_all):
        generated_registers = list_generated_registers(path_output_dir)

    # Construct list for generation. Filter out already generated registers
    register_list = [itm for itm in full_register_list if itm not in generated_registers]

    # Generate files
    generate_register_files(path_xml_regmap, register_list, path_output_dir)

    # Report finish
    print("Generation was finished.")




# Program entry point
if __name__ == "__main__":
    main()