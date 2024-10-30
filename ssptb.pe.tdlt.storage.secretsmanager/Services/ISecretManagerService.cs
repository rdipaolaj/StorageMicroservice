using ssptb.pe.tdlt.storage.common.Secrets;
using System.Runtime;

namespace ssptb.pe.tdlt.storage.secretsmanager.Services;
public interface ISecretManagerService
{
    Task<CouchBaseSecrets?> GetCouchBaseSecrets();
    Task<MongoDbSecrets?> GetMongoDbSecrets();
    Task<CloudinarySecrets?> GetCloudinarySecrets();
    Task<RedisSecrets?> GetRedisSecrets();
}
