// See https://aka.ms/new-console-template for more information

var httpClient = new HttpClient();
while (true)
{
    var result = await httpClient.GetStringAsync("https://app-workshop-dnazghbicep-dev.azurewebsites.net/api/version");
    Console.WriteLine($"{DateTime.Now:h:mm:ss tt} - {result}");
    await Task.Delay(1_000);
}