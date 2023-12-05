using AzureAdmin;

if (args.Length != 4)
{
    Console.WriteLine("You must pass 4 arguments. First one is subscription id, second one is GitHub username, third one is GitHub repo, and email.");
    return;
}

var subscriptionId = args[0];
var gitHubUsername = args[1];
var gitHubRepo = args[2];
var email = args[3];

var environments = new List<string> { "dev", "prod" };

var invite = await AzureOperations.InviteUserAsync(email);

var userObjectId = invite?.InvitedUser?.Id ?? throw new Exception("Invite failed bc of " + invite?.Status);
foreach (var environment in environments)
{
    Console.WriteLine($"Adding creds for {environment} environment");
    var appRegistration = AzureOperations.CreateAppRegistration(gitHubUsername, environment);
    AzureOperations.CreateServicePrincipal(appRegistration.AppId);
    
    var resourceGroup = $"rg-workshop-dnazghbicep-{gitHubUsername}-{environment}";
    AzureOperations.CreateResourceGroup(resourceGroup);
    AzureOperations.AddUserAsContributorOnResourceGroup(subscriptionId, userObjectId, resourceGroup);
    AzureOperations.AddServicePrincipalAsContributorOnResourceGroup(subscriptionId, appRegistration.AppId, resourceGroup);

    AzureOperations.AddFederatedCredential(appRegistration.AppId, gitHubUsername, gitHubRepo, environment);
    AzureOperations.AddUserAsReaderOnMyResourceGroup(subscriptionId, userObjectId, $"rg-workshop-dnazghbicep-{environment}");
}