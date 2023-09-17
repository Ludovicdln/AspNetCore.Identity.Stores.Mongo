using MongoDB.Bson;
using MongoDB.Driver;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Connection.Options;

public sealed class MongoOptions
{
    public GuidRepresentation GuidRepresentation
    {
        get => BsonDefaults.GuidRepresentation;
        set => BsonDefaults.GuidRepresentation = value;
    }
    
    public string? RoleCollectionName { get; set; }
    
    public string? UserCollectionName { get; set; }
}