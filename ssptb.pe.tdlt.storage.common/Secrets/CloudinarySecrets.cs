using ssptb.pe.tdlt.storage.common.Settings;
using System.Text.Json.Serialization;

namespace ssptb.pe.tdlt.storage.common.Secrets;
public class CloudinarySecrets : ISecret
{
    [JsonPropertyName("CloudName")]
    public string CloudName { get; set; } = string.Empty;

    [JsonPropertyName("ApiKey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("ApiSecret")]
    public string ApiSecret { get; set; } = string.Empty;
}
