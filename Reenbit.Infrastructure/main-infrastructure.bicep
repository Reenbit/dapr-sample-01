param location string = resourceGroup().location
param environmentName string

var inventoryServiceName = 'app-inventory-${environmentName}'
var orderingServiceName = 'app-ordering-${environmentName}'
var containerRegistryName = replace('acrreenbitdapr${environmentName}', '-', '')

var daprComponentScopes = [
  orderingServiceName
  inventoryServiceName
]

// Container Registry
module containerRegistry 'container-registry.bicep' = {
  name: 'acr-reenbit-dapr-${environmentName}'
  params: {
    acrName: containerRegistryName
  }
}

// App Insights
module appInsights 'app-insights.bicep' = {
  name: 'appinsights-${environmentName}'
  params: {
    appInsightsName: 'ai-reenbit-${environmentName}'
    logAnalyticsWorkspaceName: 'log-reenbit-${environmentName}'
    location: location
  }
}

// Container Apps Environment 
module environment 'environment.bicep' = {
  name: 'env-reenbit-${environmentName}'
  dependsOn: [
    appInsights
  ]
  params: {
    environmentName: 'env-reenbit-${environmentName}'
    location: location
    appInsightsName: 'ai-reenbit-${environmentName}'
    logAnalyticsWorkspaceName: 'log-reenbit-${environmentName}'
  }
}

// Service Bus
module serviceBus 'service-bus.bicep' = {
  name: 'sb-reenbit-${environmentName}'
  params: {
    serviceBusNamespaceName: 'sb-reenbit-${environmentName}'
    location: location
  }
}

// API Management
module apim 'api-management.bicep' = {
  name: 'apim-reenbit-${environmentName}'
  params: {
    apimName: 'apim-reenbit-${environmentName}'
    publisherName: 'reenbit'
    publisherEmail: 'contact@reenbit.com'
    apimLocation: location
  }
}

// Redis for state management
module redisCache 'redis-cache.bicep' = {
  name: 'redis-samvirk-${environmentName}'
  params: {
    redisName: 'redis-samvirk-${environmentName}'
    location: location
  }
}

// Dapr state component
module daprStateComponent 'dapr-state.bicep' = {
  name: 'dapr-state-${environmentName}'
  dependsOn: [
    environment
    redisCache
  ]
  params: {
    scopes: daprComponentScopes
    containerAppEnvName: environment.name
    redisCacheHost: redisCache.outputs.host
    redisCachePrimaryKey: redisCache.outputs.primaryKey
  }
}

// Dapr pubsub component
resource pubSubDaprComponent 'Microsoft.App/managedEnvironments/daprComponents@2022-03-01' = {
  name: '${environment.name}/pubsub'
  dependsOn: [
    environment
    serviceBus
  ]
  properties: {
    componentType: 'pubsub.azure.servicebus'
    version: 'v1'
    secrets: [
      {
        name: 'sb-connection-string'
        value: serviceBus.outputs.connectionString
      }
    ]
    metadata: [
      {
        name: 'connectionString'
        secretRef: 'sb-connection-string'
      }
      {
        name: 'maxConcurrentHandlers'
        value: '3'
      }
    ]
    scopes: daprComponentScopes
  }
}
