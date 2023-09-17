using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Mongo.Domain.Models;

/// <summary>
/// Represents login information and source for a user record.
/// </summary>
public class MongoUserLoginInfo : UserLoginInfo
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoUserLoginInfo"/>.
    /// </summary>
    public MongoUserLoginInfo(string loginProvider, string providerKey, string? displayName) 
        : base(loginProvider, providerKey, displayName)
    {
    }
}