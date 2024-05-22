param location string = resourceGroup().location
param environmentName string
param containerRegistry string
param containerImage string

var containerAppEnvironmentName = 'env-reenbit-${environmentName}'
var orderingServiceName = 'app-ordering-${environmentName}'

resource appInsights 'Microsoft.Insights/components@2020-02-02' existing = {
  name: 'ai-reenbit-${environmentName}'
}

// Ordering Service
module orderingService '../../Reenbit.Infrastructure/container-http.bicep' = {
  name: orderingServiceName
  params: {
    enableIngress: true
    isExternalIngress: true
    location: location
    environmentName: containerAppEnvironmentName
    containerImage: '${containerRegistry}.azurecr.io/${containerImage}'
    containerAppName: orderingServiceName
    enableDapr: true
    containerPort: 6011
    minReplicas: 1
    maxReplicas: 1
    containerRegistry: containerRegistry
    env: [
      { name: 'APPINSIGHTS_INSTRUMENTATIONKEY', value: appInsights.properties.InstrumentationKey }
    ]
  }
}

// Add Ordering Service to API Management
module apimInventory '../../Reenbit.Infrastructure/api-management-api.bicep' = {
  name: 'apim-${orderingServiceName}'
  params: {
    apimName: 'apim-reenbit-${environmentName}'
    apiName: orderingServiceName
    apiUrl: 'https://${orderingService.outputs.fqdn}'
    apiPath: 'ordering'
    apiResourceId: orderingService.outputs.resourceId
  }
}
