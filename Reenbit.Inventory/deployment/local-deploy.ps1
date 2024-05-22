$environment = $args[0]
$resourceGroup = 'rg-reenbit-dapr-' + $environment
$tag = (Get-Date).ToString("yyyy-MM-dd-HHmmss")

docker build . -t reenbittest.azurecr.io/app-reenbit-inventory:$tag -f ./Reenbit.Inventory.Api/Dockerfile && docker push reenbittest.azurecr.io/app-reenbit-inventory:$tag

az group create -n $resourceGroup -l westeurope

az deployment group create -g $resourceGroup -f ./deployment/main-inventory.bicep -p environmentName=$environment -p containerImage=app-reenbit-inventory:$tag