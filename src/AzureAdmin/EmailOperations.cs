using SendGrid;
using SendGrid.Helpers.Mail;

namespace AzureAdmin;

public static class EmailOperations
{
    public static async Task SendEmailAsync(string toAddress, List<AzureCredentials> credentials)
    {
        Console.WriteLine("Sending Email");
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        var fromEmail = new EmailAddress("scott@saubersoftware.com", "Scott Sauber");
        var subject = "Azure Workshop Credentials";
        var toEmail = new EmailAddress(toAddress);
        var htmlContent = "As part of the Azure Workshop you signed up for, here are your credentials that you will be using to store in a GitHub Secret" +
                          "<br>";

        foreach (var credential in credentials)
        {
            htmlContent += $"<strong>Your {credential.Environment} Credentials are:</strong><br>" +
                           $"Tenant ID: {credential.TenantId}<br>" +
                           $"Subscription ID: {credential.SubscriptionId}<br>" +
                           $"Client ID: {credential.ClientId}<br><br>";
        }
            
        var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, "", htmlContent);
        var response = await client.SendEmailAsync(msg);
        Console.WriteLine("Email response status code" + response.StatusCode);
    }
}