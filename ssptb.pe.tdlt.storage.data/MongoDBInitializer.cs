using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ssptb.pe.tdlt.storage.common.Settings;
using ssptb.pe.tdlt.storage.entities.Storage;

namespace ssptb.pe.tdlt.storage.data;
public class MongoDBInitializer
{
    private readonly IMongoClient _client;
    private readonly MongoDbSettings _settings;

    public MongoDBInitializer(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
    }

    public void InitializeDatabase()
    {
        var database = _client.GetDatabase("Storage");

        // Verifica si la colección 'file_metadata' existe
        var collections = database.ListCollectionNames().ToList();
        if (!collections.Contains("file_metadata"))
        {
            // Crear la colección si no existe
            database.CreateCollection("file_metadata");

            // Opcional: Crear índices para mejorar el rendimiento
            var fileMetadataCollection = database.GetCollection<FileMetadata>("file_metadata");
            var indexKeys = Builders<FileMetadata>.IndexKeys.Ascending(f => f.UploadedBy);
            fileMetadataCollection.Indexes.CreateOne(new CreateIndexModel<FileMetadata>(indexKeys));

            Console.WriteLine("La colección 'file_metadata' fue creada en MongoDB.");
        }
        else
        {
            Console.WriteLine("La colección 'file_metadata' ya existe en MongoDB.");
        }
    }
}
