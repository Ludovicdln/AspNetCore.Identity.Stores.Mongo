# **AspNetCore.Identity.Stores.Mongo**

[![NuGet Badge](https://buildstats.info/nuget/AspNetCore.Identity.Stores.Mongo/)](https://www.nuget.org/packages/AspNetCore.Identity.Stores.Mongo/1.0.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![Code Coverage](https://camo.githubusercontent.com/ca187835076d969f42ffccabc5539a16e1db2dc65ea6ffa33a8824bdb2e25c61/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f436f6465253230436f7665726167652d38322532352d737563636573733f7374796c653d666c6174)


This extension simplifies the integration of Identity stores in .NET 7 with MongoDB. </br>

It provides a ready-to-use implementation of the [`IUserStore`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.iuserstore-1?view=aspnetcore-7.0) and [`IRoleStore`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.irolestore-1?view=aspnetcore-7.0) interfaces for working with users and roles stored in a MongoDB database.

## Features

- **MongoDB-Based Storage**: Replace the default SQL Server storage of ASP.NET Core Identity with MongoDB, a NoSQL database.

- **Customizable Options**: Customize MongoDB settings such as GUID representation and collection names to suit your application's needs.

- **Optimized Queries**: Utilize BsonDocument for optimized MongoDB queries to enhance performance.

## Installation

You can install this library via NuGet Package Manager:

```bash
dotnet add package AspNetCore.Identity.Stores.Mongo
```

## Getting Started

To start using this library, follow these steps:

### 1. Create Your Identity Models:

Create your own Identity models that inherit from `MongoIdentityUser<TKey>` and `MongoIdentityRole<TKey>` provided by this library. <br />

You can add custom properties and methods as needed.

````csharp
public class ApplicationUser : MongoIdentityUser<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    
    // Your custom properties and methods
}

public class ApplicationRole : MongoIdentityRole<Guid>
{
    public override Guid Id { get; set; } = Guid.NewGuid();
    
    // Your custom properties and methods
}
````

>*IMPORTANT :*
> if you don't want to override <b>Id</b> for `Guid` or `ObjectId` <b>Key</b>, you can use these class

#### <i>for GUID :</i> `MongoIdentityUserAsGuid` and `MongoIdentityRoleAsGuid`
#### <i>for ObjectId :</i> `MongoIdentityUserAsObjectId` and `MongoIdentityRoleAsObjectId`

### 2. Configure MongoDb Stores

```csharp
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole>("YourMongoDBConnectionString", "YourDatabaseName");

// only user store ? 

builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddMongoDbStores<ApplicationUser>("YourMongoDBConnectionString", "YourDatabaseName");
```

## Customizing MongoDB Options

You can customize MongoDB options by providing an action to the AddMongoDbStores method. </br>

For example, to change the <i>GUID representation</i> and <i>collection names</i> :

````csharp
builder.Services
    .AddIdentityCore<ApplicationUser>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole>(
        "YourMongoDBConnectionString",
        "YourDatabaseName",
        options =>
        {
            options.GuidRepresentation = GuidRepresentation.Standard;
            options.RoleCollectionName = "YourRoleCollectionName";
            options.UserCollectionName = "YourUserCollectionName";
        });
````

## Optimized Queries with BSONDocument

In this library, i use [`BsonDocument`](https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/data-formats/bson/) to optimize queries when interacting with MongoDB. 

For example, consider the FindUserLoginAsync method:

```csharp
protected override async Task<TUserLogin?> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();

        var matchUserIdDocument =new BsonDocument("$match", new BsonDocument("_id", ConvertIdToString(userId)));
        
        var unwindDocument = new BsonDocument("$unwind",
            new BsonDocument
            {
                { "path", "$Logins" },
                { "preserveNullAndEmptyArrays", false }
            });
        
        var matchDocument = new BsonDocument("$match",
            new BsonDocument("$and",
                new BsonArray
                {
                    new BsonDocument("Logins.LoginProvider", loginProvider),
                    new BsonDocument("Logins.ProviderKey", providerKey),
                }));

        var projectDocument = new BsonDocument("$project", new BsonDocument()
        {
            {"UserId", "$_id"},
            {"LoginProvider", 1},
            {"ProviderKey", 1},
            {"ProviderDisplayName", 1}
        });

        return (TUserLogin)(await UserCollection.AggregateAsync(
            new BsonDocumentStagePipelineDefinition<TUser, MongoIdentityUserLogin<TKey>>(new[]
                { matchUserIdDocument, unwindDocument, matchDocument, projectDocument }), cancellationToken: cancellationToken)
            .ConfigureAwait(false)).FirstOrDefault(cancellationToken);
    }
```

## Acknowledgements

I would like to thank the `ASP.NET Core Identity team` for their excellent work on Identity and the `MongoDB community` for the MongoDB .NET driver.




