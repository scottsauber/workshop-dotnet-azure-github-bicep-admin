using System.Text.Json.Serialization;

namespace AzureAdmin;

public class AzAppCreateResult
{
    [JsonPropertyName("appId")]
    public string AppId { get; set; } = "";
}