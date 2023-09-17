namespace AspNetCore.Identity.Stores.Mongo.Domain.Models;

/// <summary>
/// A class representing the roles a <see cref="MongoIdentityUser{TKey}"/> can have.
/// </summary>
public class MongoRole<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes a new instance of <see cref="MongoRole"/>.
    /// </summary>
    public MongoRole(TKey id, string? name, string? normalizedName) =>
        (Id, Name, NormalizedName) = (id, name, normalizedName);
    
    /// <summary>
    /// Gets or sets the primary key for this role.
    /// </summary>
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the name for this role.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the normalized name for this role.
    /// </summary>
    public string? NormalizedName { get; set; }
}