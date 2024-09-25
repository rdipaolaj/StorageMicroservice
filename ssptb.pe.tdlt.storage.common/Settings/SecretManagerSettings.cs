namespace ssptb.pe.tdlt.storage.common.Settings;
public class SecretManagerSettings
{
    public bool Local { get; set; }
    public string Region { get; set; } = string.Empty;
    public string ArnCouchBaseSecrets { get; set; } = string.Empty;
    public string ArnCloudinarySecrets { get; set; } = string.Empty;
}
