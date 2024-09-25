using ssptb.pe.tdlt.storage.internalservices.Helpers.Interfaces;

namespace ssptb.pe.tdlt.storage.internalservices.Helpers;
public class FileNameGenerator : IFileNameGenerator
{
    public string GenerateFileName(string baseFileName)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
        return $"{baseFileName}_{timestamp}.json";
    }
}
