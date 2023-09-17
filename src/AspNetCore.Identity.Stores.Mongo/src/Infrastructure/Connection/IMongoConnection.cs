using MongoDB.Driver;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Connection;

public interface IMongoConnection
{
    IMongoDatabase GetDatabase(string databaseName);
}