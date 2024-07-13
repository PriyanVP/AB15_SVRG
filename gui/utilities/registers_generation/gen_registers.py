from jinja2 import Environment, FileSystemLoader
import xml.etree.ElementTree as et


env = Environment(loader=FileSystemLoader("."))
templ = env.get_template("Register_Template.txt")

liverpool = {"register_name": "Liverpool"}
print(templ.render(liverpool))


# Plan:
# 1. Command line arguments parser: 