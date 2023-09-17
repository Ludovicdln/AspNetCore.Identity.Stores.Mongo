using AspNetCore.Identity.Stores.Mongo.Domain.Models;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.UserLogin;

public interface IUserLoginManager<TKey> where TKey : IEquatable<TKey>
{
    bool Has(string loginProvider, string providerKey);
    
    bool Has(string loginProvider, string providerKey, out MongoUserLoginInfo? loginInfo);
    
    bool Has(string loginProvider, string providerKey, string? displayName);
    
    bool Has(string loginProvider, string providerKey, string? displayName, out MongoUserLoginInfo? loginInfo);
    
    bool TryAdd(string loginProvider, string providerKey, string? displayName);

    bool TryRemove(string loginProvider, string providerKey);
}