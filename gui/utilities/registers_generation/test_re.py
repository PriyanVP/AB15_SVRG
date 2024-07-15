import re

pattern = r'Reg_(\w+?)(?=\s|\()'

# Example usage:
test_strings = [
    "Reg_something Reg_other(",
    "Some text Reg_something2 ( and Reg_another one",
    "Reg_first Reg_second Reg_third("
]

for string in test_strings:
    matches = re.findall(pattern, string)
    for match in matches:
        print(match)