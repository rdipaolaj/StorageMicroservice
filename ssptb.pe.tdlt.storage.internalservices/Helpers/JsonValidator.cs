using ssptb.pe.tdlt.storage.internalservices.Helpers.Interfaces;
using System.Text.Json;

namespace ssptb.pe.tdlt.storage.internalservices.Helpers;
public class JsonValidator : IJsonValidator
{
    public bool IsValidJson(string jsonContent)
    {
        try
        {
            JsonDocument.Parse(jsonContent);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
