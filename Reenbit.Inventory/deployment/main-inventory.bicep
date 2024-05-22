param location string = resourceGroup().location
param environmentName string
param containerRegistry string
param containerImage string

var containerAppEnvironmentName = 'env-reenbit-${environmentName}'
var inventoryServiceName = 'app-inventory-${environmentName}'

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: 'ai-reenbit-${environmentName}'
}

// Inventory Service
module inventoryService '../../Reenbit.Infrastructure/container-http.bicep' = {
  name: inventoryServiceName
  params: {
    enableIngress: true
    isExternalIngress: true
    location: location
    environmentName: containerAppEnvironmentName
    containerImage: '${containerRegistry}.azurecr.io/${containerImage}'
    containerAppName: inventoryServiceName
    enableDapr: true
    containerPort: 6012
    minReplicas: 1
    maxReplicas: 1
    containerRegistry: containerRegistry
    env: [
      { name: 'APPINSIGHTS_INSTRUMENTATIONKEY', value: appInsights.properties.InstrumentationKey }
    ]
  }
}

// Add Inventory Service to API Management
module apimInveentory '../../Reenbit.Infrastructure/api-management-api.bicep' = {
  name: 'apim-${inventoryServiceName}'
  params: {
    apimName: 'apim-reenbit-${environmentName}'
    apiName: inventoryServiceName
    apiUrl: 'https://${inventoryService.outputs.fqdn}'
    apiPath: 'inventory'
    apiResourceId: inventoryService.outputs.resourceId
  }
}
