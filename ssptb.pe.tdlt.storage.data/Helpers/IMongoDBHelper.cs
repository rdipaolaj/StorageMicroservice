using MongoDB.Driver;

namespace ssptb.pe.tdlt.storage.data.Helpers;
public interface IMongoDBHelper
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
}
