using ssptb.pe.tdlt.storage.common.Secrets;

namespace ssptb.pe.tdlt.storage.secretsmanager.Services;
public interface ISecretManagerService
{
    Task<CouchBaseSecrets?> GetCouchBaseSecrets();
    Task<CloudinarySecrets?> GetCloudinarySecrets();
}
