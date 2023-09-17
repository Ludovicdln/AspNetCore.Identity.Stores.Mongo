using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

public class MongoIdentityUserLogin<TKey> : IdentityUserLogin<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } 
}