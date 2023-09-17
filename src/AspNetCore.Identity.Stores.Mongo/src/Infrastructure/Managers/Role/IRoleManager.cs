using AspNetCore.Identity.Stores.Mongo.Domain.Models;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Role;

public interface IRoleManager<TKey> where TKey : IEquatable<TKey>
{
    bool Has(TKey id, out MongoRole<TKey>? role);
    
    bool Has(TKey id);
    
    bool Has(string normalizedName, out MongoRole<TKey>? role);
    
    bool Has(string normalizedName);

    IList<string> GetNames();

    bool TryRemove(string normalizedName);

    bool TryAdd(TKey id, string? normalizedName, string? name);
}