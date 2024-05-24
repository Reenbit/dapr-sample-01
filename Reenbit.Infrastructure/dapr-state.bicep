param scopes array = []
param redisCachePrimaryKey string
param redisCacheHost string
param containerAppEnvName string


resource stateDaprComponent 'Microsoft.App/managedEnvironments/daprComponents@2022-10-01' = {
  name: '${containerAppEnvName}/statestore'
  properties: {
    componentType: 'state.redis'
    version: 'v1'
    secrets: [
      {
        name: 'redis-password'
        value: redisCachePrimaryKey
      }
    ]
    metadata: [
      {
        name: 'redisHost'
        value: '${redisCacheHost}:6379'
      }
      {
        name: 'actorStateStore'
        value: 'true'
      }
      {
        name: 'redisPassword'
        secretRef: 'redis-password'
      }
    ]
    scopes: scopes
  }
}
