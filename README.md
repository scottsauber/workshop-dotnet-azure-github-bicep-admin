# Azure Admin

This console app sets up the App Registration, Service Principal, Resource Groups, and the user for an Azure, GitHub Actions, and Bicep Workshop

Outcomes of running this:
- Invite user to Azure Tenant
- For each environment (dev + prod)
  - Creates App Registration
  - Creates Service Principal
  - Creates resource groups
  - Add the user as a reader on the base RGs (for viewing the live demo)
  - Add the user as a contributor on the created RGs
  - Add the SP as a contributor on the RGs
  - Add a Federated Credential

# Usage

```
dotnet run -- "...subid here..." "scottsauberlt" "workshop-dotnet-azure-github-bicep" "someemailhere@test.com"
```