# Azure Key Vault Setup Instructions

This document describes how to set up Azure Key Vault to securely store your MongoDB connection string and database name.

## Prerequisites

1. An Azure account with an active subscription
2. Sufficient permissions to create Azure Key Vault resources
3. Azure CLI installed (or you can use Azure Portal)

## Steps to Set Up Azure Key Vault

### 1. Create an Azure Key Vault

```bash
# Login to Azure
az login

# Create a resource group (if you don't have one)
az group create --name YourResourceGroupName --location YourLocation

# Create a Key Vault
az keyvault create --name YourKeyVaultName --resource-group YourResourceGroupName --location YourLocation
```

### 2. Add MongoDB Secrets to Key Vault

```bash
# Add MongoDB connection string
az keyvault secret set --vault-name YourKeyVaultName --name "MongoDB--ConnectionString" --value "mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority"

# Add MongoDB database name
az keyvault secret set --vault-name YourKeyVaultName --name "MongoDB--DatabaseName" --value "YourDatabaseName"
```

### 3. Configure Application to Access Key Vault

#### Update the appsettings.json file

Make sure your appsettings.json contains the Key Vault URI:

```json
"KeyVault": {
  "Uri": "https://your-key-vault-name.vault.azure.net/"
}
```

### 4. Authentication Options

#### Option 1: Using Managed Identity (recommended for production)

If your application is deployed to Azure (App Service, VM, etc.), you can use Managed Identity:

1. Enable Managed Identity for your Azure service
2. Grant the Managed Identity access to Key Vault secrets:

```bash
az keyvault set-policy --name YourKeyVaultName --object-id <managed-identity-object-id> --secret-permissions get list
```

#### Option 2: Using Service Principal (for development or non-Azure environments)

1. Create a service principal:

```bash
az ad sp create-for-rbac --name "YourAppName" --skip-assignment
```

2. Grant the service principal access to Key Vault secrets:

```bash
az keyvault set-policy --name YourKeyVaultName --spn <service-principal-appId> --secret-permissions get list
```

3. Set environment variables for authentication:

```bash
setx AZURE_TENANT_ID "your-tenant-id"
setx AZURE_CLIENT_ID "your-client-id"
setx AZURE_CLIENT_SECRET "your-client-secret"
```

## Local Development with User Identity

For local development, you can use your own Azure user identity:

1. Install Azure CLI and log in:

```bash
az login
```

2. Grant your user access to Key Vault secrets:

```bash
az keyvault set-policy --name YourKeyVaultName --upn your.email@example.com --secret-permissions get list
```

## Troubleshooting

If you encounter authentication issues:

1. Verify that the Key Vault URI is correct
2. Check that the authentication credentials have proper permissions
3. For local development, ensure you're logged in with `az login`
4. Verify that the secret names match exactly what the application is looking for

## Security Best Practices

1. Rotate credentials regularly
2. Use the principle of least privilege when assigning permissions
3. Consider using Key Vault's advanced features like soft delete and purge protection
4. Monitor Key Vault access with Azure Monitor
