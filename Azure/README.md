# Scripts to create Azure Web App for the project

## Setup
1) Copy <i>azure_setup.sample.conf</i> and name it <i>azure_setup.conf</i>.
2) In the config file, set the <i>ProjectName</i> variable to the name of the site:

    `ProjectName=CameraStreaming`

    <i>Note: spaces around the '=' are not allowed.</i>

## Create Azure resources for the website
Run the Bash script `run_azure_setup.sh` from a Linux shell:

`$ ./run_azure_setup.sh`

<i>Note: On Windows, install WSL to open a Linux shell.</i>

Output is logged to `azure_setup.log`, where you can find the website address and other information.

## Start logging
When the project is running in Azure, logging can be started by the  `start_azure_logging.sh` script in a Linux shell:

`$ start_azure_logging.sh`
