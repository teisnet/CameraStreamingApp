# Load config values
source azure_setup.conf

ResourceGroupName="${ProjectName}ResourceGroup"

# Set log level
# accepted values: error, information, verbose, warning
# Note: Log level always seem to be 'Verbose' disregarding this command.
# Configure detailed error messages: --detailed-error-messages {false, true}
# az webapp log config --name $ProjectName --resource-group $ResourceGroupName --application-logging filesystem --level information

# Start logging
az webapp log tail --name $ProjectName --resource-group $ResourceGroupName
