using Couchbase;
using Couchbase.KeyValue;

namespace ssptb.pe.tdlt.storage.data.Helpers;
public interface ICouchbaseHelper
{
    Task<ICouchbaseCollection> GetCollectionAsync(string scopeName, string collectionName);
    Task<ICluster> GetClusterAsync();
}
