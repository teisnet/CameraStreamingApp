# Scripts to create Azure Web App for the project
When the Azure Web App is created, the project can be published online through Visual Studio.

## Prerequisites
1) Either Linux or Windows with [Windows Subsystem for Linux](https://docs.microsoft.com/en-us/windows/wsl/install-win10) installed, including the [Ubuntu app](https://www.microsoft.com/en-us/p/ubuntu/9nblggh4msv6).
2) An [Azure](https://azure.microsoft.com/) account.
3) [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)  installed on the computer.

## Setup
1) Copy <i>azure_setup.sample.conf</i> and name it <i>azure_setup.conf</i>.
2) In the new file, set the <i>ProjectName</i> variable to the name of the site:

    `ProjectName=CameraStreaming`

    <i>Note: spaces around the '=' are not allowed.</i>

## Create Azure resources for the website
1) Log in to the Linux shell.</i>
2) Make sure you are logged in to your Azure account:

    `$ az login`

3) Run the command:

    `$ ./run_azure_setup.sh`



Output is logged to `azure_setup.log`, where you can find the website address and other useful information.

## Start logging
When the project is running in Azure, logging can be started by the command:

`$ start_azure_logging.sh`
