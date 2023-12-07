using System.Diagnostics;
using System.Text.Json;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Process = System.Diagnostics.Process;

namespace AzureAdmin;

public static class AzureOperations
{
    public static AzAppCreateResult CreateAppRegistration(string gitHubUsername, string environment)
    {
        Console.WriteLine("Creating App Registration");
        var result = RunCommandLine($"az ad app create --display-name sp-workshop-dnazghbicep-{gitHubUsername}-{environment}");
        return JsonSerializer.Deserialize<AzAppCreateResult>(result)!;
    }

    public static void CreateServicePrincipal(string appId)
    {
        Console.WriteLine("Creating Service Principal");
        RunCommandLine($"az ad sp create --id {appId}");
    }

    public static void CreateResourceGroup(string resourceGroupName)
    {
        Console.WriteLine("Creating Resource Group");
        RunCommandLine($"az group create --name {resourceGroupName} --location centralus");
    }

    public static void AddServicePrincipalAsContributorOnResourceGroup(string subscriptionId, string appId, string resourceGroupName)
    {
        Console.WriteLine("Adding SP as Contributor to RG");
        RunCommandLine($"az role assignment create --role contributor --subscription {subscriptionId} --assignee-object-id {appId} --assignee-principal-type ServicePrincipal --scope /subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}");
    }
    
    public static async Task<Invitation?> InviteUserAsync(string email)
    {
        Console.WriteLine("Sending invite to user");
        var requestBody = new Invitation
        {
            InvitedUserEmailAddress = email,
            InviteRedirectUrl = "https://github.com/scottsauber/workshop-dotnet-azure-github-bicep",
            SendInvitationMessage = true,
        };
        var graphClient = new GraphServiceClient(new AzureCliCredential());
        
        return await graphClient.Invitations.PostAsync(requestBody);
    }

    public static void AddUserAsContributorOnResourceGroup(string subscriptionId, string userObjectId, string resourceGroupName)
    {
        Console.WriteLine("Adding User as Contributor to RG");

        RunCommandLine(
            $"az role assignment create --role contributor --subscription {subscriptionId} --assignee-object-id {userObjectId} --assignee-principal-type User --scope /subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}");
    }

    public static void AddUserAsReaderOnMyResourceGroup(string subscriptionId, string userObjectId, string resourceGroupName)
    {
        Console.WriteLine("Adding User as Reader to my RG");

        RunCommandLine($"az role assignment create --role reader --subscription {subscriptionId} --assignee-object-id {userObjectId} --assignee-principal-type User --scope /subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}");
    }

    public static void AddFederatedCredential(string appId, string gitHubUsername, string gitHubRepo, string environment, bool isPrVerify)
    {
        Console.WriteLine("Adding Federated Credential");
        
        var federatedCredentialName = $"fc-workshop-dnazghbicep-{gitHubUsername}-{environment}";
        if (isPrVerify)
            federatedCredentialName += "-pr";
        
        var federatedCredentialParameters = new
        {
            name = federatedCredentialName,
            issuer = "https://token.actions.githubusercontent.com/",
            subject = isPrVerify 
                ? $"repo:{gitHubUsername}/{gitHubRepo}:pull_request" 
                : $"repo:{gitHubUsername}/{gitHubRepo}:ref:refs/heads/main",
            description = $"Repo for authing to {gitHubUsername}/{gitHubRepo}",
            audiences = new[]{  "api://AzureADTokenExchange" }
        };

        var serializedParameters = JsonSerializer.Serialize(federatedCredentialParameters).Replace("\"", "\\\"");
        RunCommandLine($"az ad app federated-credential create --id {appId} --parameters \"{serializedParameters}\"");
    }

    private static string RunCommandLine(string arguments)
    {
        var processStartInfo =
            new ProcessStartInfo("CMD.exe", $"/c {arguments}")
            {
                RedirectStandardOutput = true
            };
        var process = Process.Start(processStartInfo);

        return process!.StandardOutput.ReadToEnd();
    }
}