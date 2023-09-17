using AspNetCore.Identity.Stores.Mongo.Domain.Contracts;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Claim;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

/// <summary>
/// Represents a role in the identity system
/// </summary>
/// <typeparam name="TKey">The type used for the primary key for the role.</typeparam>
public class MongoIdentityRole<TKey> : IdentityRole<TKey>, IClaimable where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityRole{TKey}"/>.
    /// </summary>
    public MongoIdentityRole()
    {
        InitializeManagers();
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityRole{TKey}"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    public MongoIdentityRole(string roleName) : base(roleName)
    {
    }
    
    protected void InitializeManagers()
    {
        ClaimManager = new ClaimManager<TKey>(this);
    }

    public override string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The claims that role has.
    /// </summary>
    public List<MongoClaim> Claims { get; set; } = new ();
    
    /// <summary>
    /// The manager of <see cref="MongoClaim"/>s
    /// </summary>
    internal IClaimManager<TKey> ClaimManager { get; private set; }
}

public class MongoIdentityRoleAsGuid : MongoIdentityRole<Guid>
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityRole{TKey}"/>.
    /// </summary>
    public MongoIdentityRoleAsGuid()
    {
        InitializeManagers();
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityRole{TKey}"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    public MongoIdentityRoleAsGuid(string roleName) : base(roleName)
    {
    }

    public override Guid Id { get; set; } = Guid.NewGuid();
}

public class MongoIdentityRoleAsObjectId : MongoIdentityRole<ObjectId>
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityRole{TKey}"/>.
    /// </summary>
    public MongoIdentityRoleAsObjectId()
    {
        InitializeManagers();
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityRole{TKey}"/>.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    public MongoIdentityRoleAsObjectId(string roleName) : base(roleName)
    {
    }
    
    public override ObjectId Id { get; set; } = ObjectId.GenerateNewId();
}