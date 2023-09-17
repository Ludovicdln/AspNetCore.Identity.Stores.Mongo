using AspNetCore.Identity.Stores.Mongo.Domain.Models;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Token;

public interface ITokenManager<TKey> where TKey : IEquatable<TKey>
{    
    bool Has(string loginProvider, string name);

    bool Has(string loginProvider, string name, out MongoToken? token);
    
    bool Has(string loginProvider, string name, string value);
    
    bool Has(string loginProvider, string name, string value, out MongoToken? token);
    
    bool TryAdd(string loginProvider, string name, string? value);

    bool TryRemove(string loginProvider, string name, string? value);

    bool TryReplace(string loginProvider, string name, string? newValue);
}