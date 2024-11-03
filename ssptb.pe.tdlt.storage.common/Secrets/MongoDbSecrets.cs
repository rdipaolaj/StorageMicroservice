using ssptb.pe.tdlt.storage.common.Settings;
using System.Text.Json.Serialization;

namespace ssptb.pe.tdlt.storage.common.Secrets;

public class MongoDbSecrets : ISecret
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; } = string.Empty;
}

