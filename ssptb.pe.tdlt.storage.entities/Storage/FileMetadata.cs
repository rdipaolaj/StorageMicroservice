namespace ssptb.pe.tdlt.storage.entities.Storage;
public class FileMetadata
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public int SchemaVersion { get; set; }
}
