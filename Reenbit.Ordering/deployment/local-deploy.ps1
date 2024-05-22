$environment = $args[0]
$resourceGroup = 'rg-reenbit-dapr-' + $environment
$tag = (Get-Date).ToString("yyyy-MM-dd-HHmmss")

docker build . -t reenbittest.azurecr.io/app-reenbit-ordering:$tag -f ./Reenbit.ordering.Api/Dockerfile && docker push reenbittest.azurecr.io/app-reenbit-ordering:$tag

az group create -n $resourceGroup -l westeurope

az deployment group create -g $resourceGroup -f ./deployment/main-ordering.bicep -p environmentName=$environment -p containerImage=app-reenbit-ordering:$tag