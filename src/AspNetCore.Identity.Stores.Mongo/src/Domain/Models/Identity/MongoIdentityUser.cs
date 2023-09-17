using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Stores.Mongo.Domain.Contracts;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Claim;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Role;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Token;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.UserLogin;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

/// <summary>
/// Represents a user in the identity system
/// </summary>
/// <typeparam name="TKey">The type used for the primary key for the user.</typeparam>
public class MongoIdentityUser<TKey> : IdentityUser<TKey>, IClaimable where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityUser{TKey}"/>.
    /// </summary>
    /// <param name="userName">The user name.</param>
    public MongoIdentityUser(string userName) : base(userName)
    {
        InitializeManagers();
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityUser{TKey}"/>.
    /// </summary>
    public MongoIdentityUser()
    {
        InitializeManagers();
    }

    protected void InitializeManagers()
    {
        RoleManager = new Infrastructure.Managers.Role.RoleManager<TKey>(this);
        ClaimManager = new ClaimManager<TKey>(this);
        UserLoginManager = new UserLoginManager<TKey>(this);
        TokenManager = new TokenManager<TKey>(this);
    }
    
    /// <summary>
    /// The list of <see cref="MongoRole{TKey}"/>s that this user has.
    /// </summary>
    public List<MongoRole<TKey>> Roles { get; set; } = new();
    
    /// <summary>
    /// The claims that user has.
    /// </summary>
    public List<MongoClaim> Claims { get; set; } = new ();
    
    /// <summary>
    /// The list of <see cref="MongoUserLoginInfo"/>s that this user has.
    /// </summary>
    public List<MongoUserLoginInfo> Logins { get; set; } = new ();

    /// <summary>
    /// The list of <see cref="MongoToken"/>s that this user has.
    /// </summary>
    public List<MongoToken> Tokens { get; set; } = new();
    
    /// <summary>
    /// The manager of <see cref="MongoRole{TKey}"/>s
    /// </summary>
    internal IRoleManager<TKey> RoleManager { get; private set; }
    
    /// <summary>
    /// The manager of <see cref="MongoClaim"/>s
    /// </summary>
    internal IClaimManager<TKey> ClaimManager { get; private set; }
    
    /// <summary>
    /// The manager of <see cref="MongoUserLoginInfo"/>s
    /// </summary>
    internal IUserLoginManager<TKey> UserLoginManager { get; private set; }
    
    /// <summary>
    /// The manager of <see cref="MongoToken"/>s
    /// </summary>
    internal ITokenManager<TKey> TokenManager { get; private set; } 
}


public class MongoIdentityUserAsGuid : MongoIdentityUser<Guid>
{
    public MongoIdentityUserAsGuid(string userName) : base(userName)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityUser{TKey}"/>.
    /// </summary>
    public MongoIdentityUserAsGuid()
    {
        base.InitializeManagers();
    }
    
    public override Guid Id { get; set; } = Guid.NewGuid();
}

public class MongoIdentityUserAsObjectId : MongoIdentityUser<ObjectId>
{
    public MongoIdentityUserAsObjectId(string userName) : base(userName)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoIdentityUser{TKey}"/>.
    /// </summary>
    public MongoIdentityUserAsObjectId()
    {
        base.InitializeManagers();
    }

    public override ObjectId Id { get; set; } = ObjectId.GenerateNewId();
}