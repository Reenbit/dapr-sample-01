param containerAppName string
param location string
param environmentName string
param containerImage string
param containerPort int
param isExternalIngress bool
param containerRegistry string
param enableIngress bool
param minReplicas int = 1
param maxReplicas int = 1
param env array = []
param enableDapr bool = false

resource environment 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: environmentName
}

resource registry 'Microsoft.ContainerRegistry/registries@2021-12-01-preview' existing = {
  name: containerRegistry
}

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      secrets: [
        {
          name: 'container-registry-password'
          value: registry.listCredentials().passwords[0].value
        }
      ]
      registries: [
        {
          server: '${containerRegistry}.azurecr.io'
          username: registry.listCredentials().username
          passwordSecretRef: 'container-registry-password'
        }
      ]
      ingress: enableIngress
        ? {
            external: isExternalIngress
            targetPort: containerPort
            transport: 'auto'
          }
        : null
      dapr: {
        enabled: enableDapr
        appPort: containerPort
        appId: containerAppName
      }
    }
    template: {
      containers: [
        {
          image: containerImage
          name: containerAppName
          env: env
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
  identity: {
    type: 'SystemAssigned'
  }
}

output fqdn string = enableIngress ? containerApp.properties.configuration.ingress.fqdn : 'Ingress not enabled'
output principalId string = containerApp.identity.principalId
output resourceId string = containerApp.id
