using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Token;

public sealed class TokenManager<TKey> : ITokenManager<TKey> where TKey : IEquatable<TKey>
{
    private readonly MongoIdentityUser<TKey> _mongoIdentityUser;

    public TokenManager(MongoIdentityUser<TKey> mongoIdentityUser) => _mongoIdentityUser = mongoIdentityUser;

    public bool Has(string loginProvider, string name, string value)
        => _mongoIdentityUser.Tokens.Any(x => x.LoginProvider == loginProvider && x.Name == name && x.Value == value);

    public bool Has(string loginProvider, string name)
        => _mongoIdentityUser.Tokens.Any(x => x.LoginProvider == loginProvider && x.Name == name);
    
    public bool Has(string loginProvider, string name, out MongoToken? token)
    {
        token = _mongoIdentityUser.Tokens.FirstOrDefault(x => x.LoginProvider == loginProvider 
                                                              && x.Name == name);
        return token is not null;
    }

    public bool Has(string loginProvider, string name, string value, out MongoToken? token)
    {
        token = _mongoIdentityUser.Tokens.FirstOrDefault(x => x.LoginProvider == loginProvider 
                                                              && x.Name == name && x.Value == value);
        return token is not null;
    }

    public bool TryAdd(string loginProvider, string name, string? value)
    {
        if (string.IsNullOrEmpty(value) ? Has(loginProvider, name) : Has(loginProvider, name, value)) return false;
        
        var mongoToken = new MongoToken(loginProvider, name, value);
        
        _mongoIdentityUser.Tokens.Add(mongoToken);
        
        return true;
    }

    public bool TryRemove(string loginProvider, string name, string? value)
    {
        if (string.IsNullOrEmpty(value) 
                ? !Has(loginProvider, name, out var token) 
                : !Has(loginProvider, name, value, out token)) return false;
            
        _mongoIdentityUser.Tokens.Remove(token!);
            
        return true;
    }

    public bool TryReplace(string loginProvider, string name, string? newValue)
    {
        if (!Has(loginProvider, name, out var token)) return false;
        
        token!.Value = newValue;
        
        return true;

    }
}