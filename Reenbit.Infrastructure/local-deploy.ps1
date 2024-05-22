[CmdletBinding()]
param(
    [Parameter(Position=0,mandatory=$true)]
    [string] $environment
)

$resourceGroup = 'rg-reenbit-dapr-' + $environment

az group create -n $resourceGroup -l westeurope

az deployment group create -g $resourceGroup -f ./main-infrastructure.bicep -p environmentName=$environment 