using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests;

public sealed class IdentityBuilderTests
{
    private const string CONNECTION_STRING = "mongodb://localhost:27017";

    [Test]
    public void AddUserMongoStore_ShouldSuccess()
    {
        var serviceCollection = new ServiceCollection();

        var identityBuilder = new IdentityBuilder(typeof(MyUser), serviceCollection);

        identityBuilder.AddMongoDbStores<MyUser>(
            CONNECTION_STRING, "testDb");

        var userStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IUserStore<MyUser>));
        
        Assert.That(userStoreService, Is.Not.Null);
    }
    
    [Test]
    public void AddUserMongoStore_WithOptions_ShouldSuccess()
    {
        var serviceCollection = new ServiceCollection();

        var identityBuilder = new IdentityBuilder(typeof(MyUser), serviceCollection);

        identityBuilder.AddMongoDbStores<MyUser>(
            CONNECTION_STRING, "testDb", options =>
            {
                options.UserCollectionName = "TestUser";
            });

        var userStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IUserStore<MyUser>));
        
        var userMongoServiceObj = identityBuilder.Services.BuildServiceProvider().GetService<IMongoCollection<MyUser>>();
        
        Assert.Multiple(() =>
        {
            Assert.That(userStoreService, Is.Not.Null);
            Assert.That(userMongoServiceObj, Is.Not.Null);
            Assert.That(userMongoServiceObj.CollectionNamespace.CollectionName, Is.EqualTo("TestUser"));
        });
    }

    [Test]
    public void AddUserRoleMongoStore_ShouldSuccess()
    {
        var serviceCollection = new ServiceCollection();

        var identityBuilder = new IdentityBuilder(typeof(MyUser), serviceCollection);

        identityBuilder.AddMongoDbStores<MyUser, MyRole>(
            CONNECTION_STRING, "testDb");

        var userStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IUserStore<MyUser>));
        
        var roleStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRoleStore<MyRole>));
        
        Assert.Multiple(() =>
        {
            Assert.That(userStoreService, Is.Not.Null);
            Assert.That(roleStoreService, Is.Not.Null);
        });
    }
    
    [Test]
    public void AddUserRoleMongoStore_WithOptions_ShouldSuccess()
    {
        var serviceCollection = new ServiceCollection();

        var identityBuilder = new IdentityBuilder(typeof(MyUser), serviceCollection);

        identityBuilder.AddMongoDbStores<MyUser, MyRole>(
            CONNECTION_STRING, "testDb", options =>
            {
                options.UserCollectionName = "TestUser";
                options.RoleCollectionName = "TestRole";
            });

        var userStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IUserStore<MyUser>));
        
        var userMongoServiceObj = identityBuilder.Services.BuildServiceProvider().GetService<IMongoCollection<MyUser>>();
        
        var roleMongoServiceObj = identityBuilder.Services.BuildServiceProvider().GetService<IMongoCollection<MyRole>>();
        
        Assert.Multiple(() =>
        {
            Assert.That(userStoreService, Is.Not.Null);
            
            Assert.That(userMongoServiceObj, Is.Not.Null);
            Assert.That(userMongoServiceObj.CollectionNamespace.CollectionName, Is.EqualTo("TestUser"));
            
            Assert.That(roleMongoServiceObj, Is.Not.Null);
            Assert.That(roleMongoServiceObj.CollectionNamespace.CollectionName, Is.EqualTo("TestRole"));
        });
        
    }
    
    [Test]
    public void AddUserRoleMongoStore_WithKey_ShouldSuccess()
    {
        var serviceCollection = new ServiceCollection();

        var identityBuilder = new IdentityBuilder(typeof(MyUser), serviceCollection);

        identityBuilder.AddMongoDbStores<MyUser, MyRole, Guid>(
            CONNECTION_STRING, "testDb");

        var userStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IUserStore<MyUser>));
        
        var roleStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRoleStore<MyRole>));
        
        Assert.Multiple(() =>
        {
            Assert.That(userStoreService, Is.Not.Null);
            Assert.That(roleStoreService, Is.Not.Null);
        });
    }
    
    [Test]
    public void AddUserRoleMongoStore_WithKey_AndOptions_ShouldSuccess()
    {
        var serviceCollection = new ServiceCollection();

        var identityBuilder = new IdentityBuilder(typeof(MyUser), serviceCollection);

        identityBuilder.AddMongoDbStores<MyUser, MyRole, Guid>(
            CONNECTION_STRING, "testDb", options =>
            {
                options.UserCollectionName = "TestUser";
                options.RoleCollectionName = "TestRole";
            });

        var userStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IUserStore<MyUser>));
        
        var roleStoreService = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IRoleStore<MyRole>));
        
        var userMongoServiceObj = identityBuilder.Services.BuildServiceProvider().GetService<IMongoCollection<MyUser>>();
        
        var roleMongoServiceObj = identityBuilder.Services.BuildServiceProvider().GetService<IMongoCollection<MyRole>>();
        
        Assert.Multiple(() =>
        {
            Assert.That(userStoreService, Is.Not.Null);
            Assert.That(roleStoreService, Is.Not.Null);
            
            Assert.That(userMongoServiceObj, Is.Not.Null);
            Assert.That(userMongoServiceObj.CollectionNamespace.CollectionName, Is.EqualTo("TestUser"));
            
            Assert.That(roleMongoServiceObj, Is.Not.Null);
            Assert.That(roleMongoServiceObj.CollectionNamespace.CollectionName, Is.EqualTo("TestRole"));
        });
    }
}