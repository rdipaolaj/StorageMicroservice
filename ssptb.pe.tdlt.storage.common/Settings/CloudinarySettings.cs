using System.Text.Json.Serialization;

namespace ssptb.pe.tdlt.storage.common.Settings;
public class CloudinarySettings
{
    public string CloudName { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;
}
