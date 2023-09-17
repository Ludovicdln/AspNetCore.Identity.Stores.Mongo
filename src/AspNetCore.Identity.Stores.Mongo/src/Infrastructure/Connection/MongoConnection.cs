using AspNetCore.Identity.Stores.Mongo.Infrastructure.Connection.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Connection;

public sealed class MongoConnection : IMongoConnection
{
    private readonly IMongoClient _mongoClient;

    private MongoConnection(IMongoClient mongoClient) => 
        _mongoClient = mongoClient ?? throw new ArgumentNullException(nameof(mongoClient));
    
    public IMongoDatabase GetDatabase(string databaseName)
    {
        return _mongoClient.GetDatabase(databaseName);
    }

    /*public static MongoConnection Connect(string connectionString, MongoOptions options)
    {
        var client = CreateClient(connectionString);
        
        return new MongoConnection(client!);
    }*/
    
    public static MongoConnection Connect(string connectionString)
    {
        var client = CreateClient(connectionString);
        
        return new MongoConnection(client!);
    }

    public static Task<MongoConnection> ConnectAsync(string connectionString)
    {
        var client = CreateClient(connectionString);
        
        return Task.FromResult(new MongoConnection(client!));
    }
    
    /*public static Task<MongoConnection> ConnectAsync(string connectionString, MongoOptions options)
    {
        var client = CreateClient(connectionString);
        
        return Task.FromResult(new MongoConnection(client!));
    }*/

    private static IMongoClient? CreateClient(string connectionString)
    {
        return new MongoClient(connectionString);
    }
}