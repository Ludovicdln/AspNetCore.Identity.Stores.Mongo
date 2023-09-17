using System.Security.Claims;

namespace AspNetCore.Identity.Stores.Mongo.Domain.Models;

/// <summary>
/// A class representing the claims a <see cref="MongoIdentityUser{TKey}"/> can have.
/// </summary>
public sealed class MongoClaim
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoClaim"/>.
    /// </summary>
    public MongoClaim(string type, string value, string valueType, string issuer) =>
        (Type, Value, ValueType, Issuer) = (type, value, valueType, issuer);
    
    /// <summary>
    /// The type of the claim.
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// The value of the claim.
    /// </summary>
    public string Value { get; set; }
    /// <summary>
    /// The valueType of the claim.
    /// </summary>
    public string ValueType { get; set; }
    /// <summary>
    /// The issuer of the claim.
    /// </summary>
    public string Issuer { get; set; }
}