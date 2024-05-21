<!---

	Copyright (c) 2009, 2018 Robert Bosch GmbH and its subsidiaries.
	This program and the accompanying materials are made available under
	the terms of the Bosch Internal Open Source License v4
	which accompanies this distribution, and is available at
	http://bios.intranet.bosch.com/bioslv4.txt

-->

# Project Name  <!-- omit in toc -->

[![License: BIOSL v4](http://bios.intranet.bosch.com/bioslv4-badge.svg)](#license)

Add a brief description about the contents of this repository here.
[Link to documentation]()

## Table of Contents  <!-- omit in toc -->

- [Getting Started](#getting-started)
- [Building and Testing](#building-and-testing)
- [Contribution Guidelines](#contribution-guidelines)
- [Configure Git and correct EOL handling](#configure-Git-and-correct-EOL-handling)
- [Feedback](#feedback)
- [About](#about)
  - [Maintainers](#maintainers)
  - [Contributors](#contributors)
  - [3rd Party Licenses](#3rd-party-licenses)
  - [Used Encryption](#used-encryption)
  - [License](#license)

## Getting Started <a name="getting-started"></a>

* Request access to repository from [Dudnyk Oleksii](https://connect.bosch.com/profiles/html/profileView.do?key=7a5402e8-f7ce-4dd0-bc62-a9b66e4c6b9a#&tabinst=Updates)
* Clone this repository \
	`git clone ssh://git@sourcecode.socialcoding.bosch.com:7999/mitocotadev/mitocota.git`
* Install Aurix IDE
* Launch Aurix IDE workspace in firmware folder
* Select mcu_esw folder in firmware folder as project in Aurix IDE
* Make this project as active
* Rebuild MCU SW via button in Aurix IDE
* Connect ShieldBuddy TC375 MCU to USB port
* Flash MCU SW via button in Aurix IDE
* Launch GUI from application/gui folder (double-click on exe file) 

## Building and Testing <a name="building-and-testing"></a>

Building MCU SW:
* Launch Aurix IDE workspace in firmware folder (Aurix version 1.9.20)
* Select mcu_esw folder in firmware folder as project in Aurix IDE
* Make this project as active
* Rebuild MCU SW via button in Aurix IDE

Building GUI SW:
* Launch Visual Studio (testes with Microsoft Visual Studio Professional 2022 (SCCMProf22) (64-bit) - 17.9.7)
* Open Solution file (gui\AB15_GUI.sln)
* Choose relase or debug build type
* Build AB15_GUI.WPF project via Run button or context menu

Unit testing GUI SW:
* Launch Visual Studio (testes with Microsoft Visual Studio Professional 2022 (SCCMProf22) (64-bit) - 17.9.7)
* Open Solution file (gui\AB15_GUI.sln)
* Choose relase or debug build type
* Build AB15_GUI.Tests project via context menu
* Run tests in Test explorer (group AB15_GUI.Tests)

Regression testing GUI SW:
* Launch Visual Studio (testes with Microsoft Visual Studio Professional 2022 (SCCMProf22) (64-bit) - 17.9.7)
* Open Solution file (gui\AB15_GUI.sln)
* Choose relase or debug build type
* Build AB15_GUI.Regression project via context menu
* Run tests in Test explorer (group AB15_GUI.Regression)

## Contribution Guidelines <a name="contribution-guidelines"></a>

Use this section to describe or link to documentation which explaining how users can make contributions to the contents of this repository. Consider adopting the [BIOS way of facilitating contributions](http://bos.ch/ygF).

## Configure Git and correct EOL handling <a name="configure-Git-and-correct-EOL-handling"></a>
Here you can find the references for [Dealing with line endings](https://help.github.com/articles/dealing-with-line-endings/ "Wiki page from Social Coding"). 

Every time you press return on your keyboard you're actually inserting an invisible character called a line ending. Historically, different operating systems have handled line endings differently.
When you view changes in a file, Git handles line endings in its own way. Since you're collaborating on projects with Git and GitHub, Git might produce unexpected results if, for example, you're working on a Windows machine, and your collaborator has made a change in OS X.

To avoid problems in your diffs, you can configure Git to properly handle line endings. If you are storing the .gitattributes file directly inside of your repository, than you can asure that all EOL are manged by git correctly as defined.


## Feedback <a name="feedback"></a>

Consider using this section to describe how you would like other developers
to get in contact with you or provide feedback.

## About <a name="about"></a>

### Maintainers <a name="maintainers"></a>

* Developer: [Brinkmann Matthias](https://connect.bosch.com/profiles/html/profileView.do?key=385a0c29-acbe-4645-a323-dd0f930db781#&tabinst=Updates)
* Developer: [Dudnyk Oleksii](https://connect.bosch.com/profiles/html/profileView.do?key=7a5402e8-f7ce-4dd0-bc62-a9b66e4c6b9a#&tabinst=Updates)
* Developer: [Tkachenko Nikita](https://connect.bosch.com/profiles/html/profileView.do?key=60124c2b-b45f-4bb6-a004-9323e6ea1a9a#&tabinst=Updates)
* Developer: [Salziger Jan](https://connect.bosch.com/profiles/html/profileView.do?key=9a0923e5-6a55-4333-821e-a0001ec3c013#&tabinst=Updates)
* Developer: [Hoefflinger Jens](https://connect.bosch.com/profiles/html/profileView.do?key=64ef3baf-ecf6-4a9c-bc42-4382113200d3#&tabinst=Updates)


### Contributors <a name="contributors"></a>

Consider listing contributors in this section to give explicit credit. You could also ask contributors to add themselves in this file on their own.

### 3rd Party Licenses <a name="3rd-party-licenses"></a>

You must mention all 3rd party licenses (e.g. OSS) licenses used by your
project here. Example:

| Name | License | Type |
|------|---------|------|
| [Apache Felix](http://felix.apache.org/) | [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0.txt) | Dependency

### Used Encryption <a name="used-encryption"></a>

Declaration of the usage of any encryption (see BIOS Repository Policy §4.a).

### License <a name="license"></a>

[![License: BIOSL v4](http://bios.intranet.bosch.com/bioslv4-badge.svg)](#license)

> Copyright (c) 2009, 2018 Robert Bosch GmbH and its subsidiaries.
> This program and the accompanying materials are made available under
> the terms of the Bosch Internal Open Source License v4
> which accompanies this distribution, and is available at
> http://bios.intranet.bosch.com/bioslv4.txt

<!---

	Copyright (c) 2009, 2018 Robert Bosch GmbH and its subsidiaries.
	This program and the accompanying materials are made available under
	the terms of the Bosch Internal Open Source License v4
	which accompanies this distribution, and is available at
	http://bios.intranet.bosch.com/bioslv4.txt

-->
