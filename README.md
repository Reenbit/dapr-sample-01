# dapr-sample-01
Simplifying .NET distributed application with Microsoft’s Dapr and Azure Container Apps (p.1)

# Installation

1. Create Azure Service Principal and save as a secret in github repository secrets

Using Azure shell run the following command and save the output as `AZURE_CREDENTIALS` secret in github repository
```
az ad sp create-for-rbac --name "github-actions-app" --role contributor --sdk-auth --scopes /subscriptions/{subscriptionId}
```
The output is in json format:
```
{
  "clientId": "********",
  "clientSecret": "********",
  "subscriptionId": "********",
  "tenantId": "********",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

2. Manually run [`infrastructure-deploy.yml`](https://github.com/Reenbit/dapr-sample-01/actions/workflows/infrastructure-deploy.yml) for the first time to provision the Azure infrastructure.