using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.UserLogin;

public sealed class UserLoginManager<TKey> : IUserLoginManager<TKey> where TKey : IEquatable<TKey>
{
    private readonly MongoIdentityUser<TKey> _mongoIdentityUser;

    public UserLoginManager(MongoIdentityUser<TKey> mongoIdentityUser) => _mongoIdentityUser = mongoIdentityUser;

    public bool Has(string loginProvider, string providerKey)
        => _mongoIdentityUser.Logins.Any(x => x.LoginProvider.Equals(loginProvider) 
                                              && x.ProviderKey.Equals(providerKey));

    public bool Has(string loginProvider, string providerKey, out MongoUserLoginInfo? loginInfo)
    {
        loginInfo = _mongoIdentityUser.Logins.FirstOrDefault(x => x.LoginProvider.Equals(loginProvider) 
                                                                  && x.ProviderKey.Equals(providerKey));

        return loginInfo is not null;
    }

    public bool Has(string loginProvider, string providerKey, string? displayName)
        => _mongoIdentityUser.Logins.Any(x => x.LoginProvider.Equals(loginProvider) 
                                                   && x.ProviderKey.Equals(providerKey) && x.ProviderDisplayName == displayName);

    public bool Has(string loginProvider, string providerKey, string? displayName, out MongoUserLoginInfo? loginInfo)
    {
        loginInfo = _mongoIdentityUser.Logins.FirstOrDefault(x => x.LoginProvider.Equals(loginProvider) 
                                          && x.ProviderKey.Equals(providerKey) && x.ProviderDisplayName == displayName);

        return loginInfo is not null;
    }

    public bool TryAdd(string loginProvider, string providerKey, string? displayName)
    {
        if (string.IsNullOrEmpty(displayName) 
                ? Has(loginProvider, providerKey) 
                : Has(loginProvider, providerKey, displayName)) return false;
        
        var loginInfo = new MongoUserLoginInfo(loginProvider, providerKey, displayName);
        
        _mongoIdentityUser.Logins.Add(loginInfo);

        return true;
    }

    public bool TryRemove(string loginProvider, string providerKey)
    {
        if (!Has(loginProvider, providerKey, out var loginInfo)) return false;
        
        _mongoIdentityUser.Logins.Remove(loginInfo!);
        
        return true;

    }
}