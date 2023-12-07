using AzureAdmin;

if (args.Length != 5)
{
    Console.WriteLine("You must pass 5 arguments.");
    return;
}

var subscriptionId = args[0];
var tenantId = args[1];
var gitHubUsername = args[2];
var gitHubRepo = args[3];
var email = args[4];

var environments = new List<string> { "dev", "prod" };

var invite = await AzureOperations.InviteUserAsync(email);

var userObjectId = invite?.InvitedUser?.Id ?? throw new Exception("Invite failed bc of " + invite?.Status);
var azureCredentials = new List<AzureCredentials>();
foreach (var environment in environments)
{
    Console.WriteLine($"Adding creds for {environment} environment");
    var appRegistration = AzureOperations.CreateAppRegistration(gitHubUsername, environment);
    AzureOperations.CreateServicePrincipal(appRegistration.AppId);
    
    var resourceGroup = $"rg-workshop-dnazghbicep-{gitHubUsername}-{environment}";
    AzureOperations.CreateResourceGroup(resourceGroup);
    AzureOperations.AddUserAsContributorOnResourceGroup(subscriptionId, userObjectId, resourceGroup);
    AzureOperations.AddServicePrincipalAsContributorOnResourceGroup(subscriptionId, appRegistration.AppId, resourceGroup);
    
    AzureOperations.AddFederatedCredential(appRegistration.AppId, gitHubUsername, gitHubRepo, environment, false);
    AzureOperations.AddFederatedCredential(appRegistration.AppId, gitHubUsername, gitHubRepo, environment, true);
    AzureOperations.AddUserAsReaderOnMyResourceGroup(subscriptionId, userObjectId, $"rg-workshop-dnazghbicep-{environment}");
    azureCredentials.Add(new AzureCredentials { ClientId = appRegistration.AppId, Environment = environment, SubscriptionId = subscriptionId, TenantId = tenantId});
}

await EmailOperations.SendEmailAsync(email, azureCredentials);