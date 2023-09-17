namespace AspNetCore.Identity.Stores.Mongo.Domain.Models;

/// <summary>
/// A class representing the tokens a <see cref="MongoIdentityUser{TKey}"/> can have.
/// </summary>
public sealed class MongoToken
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoToken"/>.
    /// </summary>
    public MongoToken(string loginProvider, string name, string? value) =>
        (LoginProvider, Name, Value) = (loginProvider, name, value);
    
    /// <summary>
    /// Initializes a new instance of <see cref="MongoToken"/>.
    /// </summary>
    public MongoToken(string loginProvider, string name) =>
        (LoginProvider, Name) = (loginProvider, name);
    
    /// <summary>
    /// Gets or sets the LoginProvider this token is from.
    /// </summary>
    public string LoginProvider { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the token. 
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    public string? Value { get; set; }
}