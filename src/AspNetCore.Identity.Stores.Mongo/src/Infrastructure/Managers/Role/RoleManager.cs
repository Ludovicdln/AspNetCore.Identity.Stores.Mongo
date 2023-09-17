using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Role;

public sealed class RoleManager<TKey> : IRoleManager<TKey> where TKey : IEquatable<TKey>
{
    private readonly MongoIdentityUser<TKey> _mongoIdentityUser;

    public RoleManager(MongoIdentityUser<TKey> mongoIdentityUser) => _mongoIdentityUser = mongoIdentityUser;

    public bool Has(TKey id, out MongoRole<TKey>? role)
    {
        role = _mongoIdentityUser.Roles.FirstOrDefault(x => x.Id.Equals(id));

        return role is not null;
    }

    public bool Has(TKey id)
        => _mongoIdentityUser.Roles.Any(x => x.Id.Equals(id));
    
    public bool Has(string normalizedName, out MongoRole<TKey>? role)
    {
        role = _mongoIdentityUser.Roles.FirstOrDefault(x => x.NormalizedName!.Equals(normalizedName));
        
        return role is not null;
    }

    public bool Has(string normalizedName)
     => _mongoIdentityUser.Roles.Any(x => x.NormalizedName!.Equals(normalizedName));

    public IList<string> GetNames()
        => _mongoIdentityUser.Roles.Select(x => x.Name!).ToList();
    
    public bool TryAdd(TKey id, string? normalizedName, string? name)
    {
        if (Has(id))
        {
            return false;
        }

        var mongoRole = new MongoRole<TKey>(id, name, normalizedName);
        
        _mongoIdentityUser.Roles.Add(mongoRole);
        
        return true;
    }
    
    public bool TryRemove(string normalizedName)
    {
        if (!Has(normalizedName, out var role)) return false;
        
        _mongoIdentityUser.Roles.Remove(role!);
        
        return true;
    }
}