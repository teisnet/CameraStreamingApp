#!/bin/bash

# Azure CLI commands for creating an Azure Service plan, a web app, a resource group, a sql server and a database.
#
# Install Azure CLI and run and login before use.
# Make sure Azure resources don't already exist.
#
# https://docs.microsoft.com/en-us/azure/app-service/tutorial-dotnetcore-sqldb-app?pivots=platform-linux
#

LogFileName="azure_setup.log"

set -eu

# Load config values
source azure_setup.conf

# Clear existing log file content
> $LogFileName

# Send command to both console and log file
function write {
	# Append to file:
	echo -e $1 2>&1 | tee -a $LogFileName
	# Overwrite file:
	# eval $1 2>&1 | tee setup.log
}

# Send command to both console and log file
function exec {
	# Append file:
	eval $1 2>&1 | tee -a $LogFileName
}

# Run command and format message
# Parameters: $1: message, $2: command
function run {
	write "\n\n\n$1\n"
	write "$ $2"
	exec "$2"
}

# Convert seconds to minutes and seconds
function seconds_to_min_sec () {
	num=$1
	min=0

	((sec=num%60))

	if((num>59)); then
		((min=num/60))
	else
		((sec=num))
	fi

	echo "$min"m "$sec"s
}

StartTime=$(date +%s)

write "Date: $(date -d @$StartTime '+%b %d %Y %H:%M')"
write "\nRunning Azure CLI commands for creating Azure Service plan, web app, resource group."

ResourceGroupName="${ProjectName}ResourceGroup"
ProjectNameLowerCase=$(echo $ProjectName | tr '[:upper:]' '[:lower:]')
AppServicePlanName="${ProjectName}AppServicePlan"
AppServiceName=$ProjectName #"${ProjectName}App"
AppServiceNameLowerCase=$ProjectNameLowerCase #"${ProjectNameLowerCase}App"

write "\n\n--------------------------------------------------------------------------------"
write "$ProjectName"
write "--------------------------------------------------------------------------------\n"

# write "Project name: $ProjectName"
# write "ProjectNameLowerCase: $ProjectNameLowerCase"
write "App Service name: $AppServiceName"
write "App Service plan name: $AppServicePlanName"
write "Resource group name: $ResourceGroupName"
write "Log filename: $LogFileName"

write "\n--------------------------------------------------------------------------------"



################ Execute Azure commands #######################

run \
"Creating resource group '$ResourceGroupName', location 'North Europe'." \
"az group create --name $ResourceGroupName --location 'North Europe'"

run \
"Creating App Service plan '$AppServicePlanName', free tier (F1:Free)." \
"az appservice plan create --name $AppServicePlanName --resource-group $ResourceGroupName --sku FREE"

# AppServiceName must be unique across Azure subscribtions, otherwise 'az webapp create' will fail
# Use "az webapp list-runtimes" for available runtimes
run \
"Creating a web app (App Service) '$AppServiceName'." \
"az webapp create --name $AppServiceName --resource-group $ResourceGroupName --plan $AppServicePlanName --runtime \"DOTNET|5.0\" --deployment-local-git"

run \
"Enable remote logging and set log level to 'Information'." \
"az webapp log config --name $AppServiceName --resource-group $ResourceGroupName --application-logging filesystem --level information"



################ Final comments ###############################

write "\n\n--------------------------------------------------------------------------------"
write "$AppServiceName"
write "--------------------------------------------------------------------------------\n"

ElapsedTime=$(($(date +%s)-$StartTime))
write "Date: $(date -d @$StartTime '+%b %d %Y %H:%M') (script completed in $(seconds_to_min_sec $ElapsedTime))"

write "\nWebsite url: https://${AppServiceNameLowerCase}.azurewebsites.net"
write "Website info: https://${AppServiceNameLowerCase}.scm.azurewebsites.net"

# Using 'declare' to store the multi-line json output in an array. 
declare -a GitUrl=($(az webapp deployment source config-local-git --resource-group $ResourceGroupName --name $AppServiceName | tr -d '\"'))
write "\nGit deployment repository: ${GitUrl[2]}"

write "\nAccess log output using command: $ az webapp log tail --name $AppServiceName --resource-group $ResourceGroupName"

write "\n--------------------------------------------------------------------------------"

write "\n\nAll commands completed sucessfully."

echo "See log file '$LogFileName'."
