using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Stores;
using MongoDB.Bson;
using MongoDB.Driver;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests.Stores;

public sealed class RoleMongoStoreStringTests
{
    private RoleMongoStore<ApplicationRoleString> _roleMongoStore;

    private const string ConnectionString = "mongodb://localhost:27017";

    [SetUp]
    public void Setup()
    {
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        
        var client =
            new MongoClient(ConnectionString);

        var database = client.GetDatabase("RoleMongoStoreStringTests");
        
        var roleCollection = database.GetCollection<ApplicationRoleString>("ApplicationRole");

        _roleMongoStore =
            new RoleMongoStore<ApplicationRoleString>(new IdentityErrorDescriber(), roleCollection);
    }
    
    [TearDown]
    public async Task TearDown()
    {
        var client =
            new MongoClient(ConnectionString);

        var database = client.GetDatabase("RoleMongoStoreStringTests");

        var collections = (await database.ListCollectionNamesAsync()).ToEnumerable();
        
        foreach (var collection in collections)
        {
            await database.DropCollectionAsync(collection);
        }
    }

    [Test]
    public async Task CreateRole_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test",
            NormalizedName = "role_test".ToUpper(),
        };

        var identityResult = await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        Assert.That(identityResult.Succeeded, Is.True);
    }
    
    [Test]
    public async Task FindById_ShouldReturnRole()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_2",
            NormalizedName = "role_test_2".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id, new CancellationToken());
        
        Assert.That(roleExist, Is.Not.Null);
        
        Assert.That(roleExist.Id.Equals(role.Id), Is.True);
    }
    
    [Test]
    public async Task FindByNormalizedName_ShouldReturnRole()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_3",
            NormalizedName = "role_test_3".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        var normalizedName = "role_test_3".ToUpper();

        var roleExist = await _roleMongoStore.FindByNameAsync(normalizedName, new CancellationToken());
        
        Assert.That(roleExist, Is.Not.Null);
        
        Assert.That(roleExist.NormalizedName.Equals(normalizedName), Is.True);
    }
    
    [Test]
    public async Task Update_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_4",
            NormalizedName = "role_test_4".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());

        role.NormalizedName = "new_role_name".ToUpper();
        
        role.Name = "new_role_name";

        await _roleMongoStore.UpdateAsync(role, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id, new CancellationToken());
        
        Assert.Multiple(() =>
        {
            Assert.That(roleExist.NormalizedName.Equals(role.NormalizedName), Is.True);
            Assert.That(roleExist.Name.Equals(role.Name), Is.True);
        });
    }
    
    [Test]
    public async Task Delete_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_5",
            NormalizedName = "role_test_5".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        var deleteResult = await _roleMongoStore.DeleteAsync(role, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id, new CancellationToken());
        
        Assert.Multiple(() =>
        {
            Assert.That(deleteResult.Succeeded, Is.True);
            Assert.That(roleExist, Is.Null);
        });
    }
    
    [Test]
    public async Task GetRoleId_ShouldReturnIdToString()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_id",
            NormalizedName = "role_test_id".ToUpper(),
        };

        var roleIdString = await _roleMongoStore.GetRoleIdAsync(role, new CancellationToken());
        
        Assert.That(roleIdString, Is.EqualTo(role.Id.ToString()));
    }
    
    [Test]
    public async Task GetRoleName_ShouldReturnName()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_name",
            NormalizedName = "role_test_name".ToUpper(),
        };

        var roleName = await _roleMongoStore.GetRoleNameAsync(role, new CancellationToken());
        
        Assert.That(roleName, Is.EqualTo(role.Name));
    }
    
    [Test]
    public async Task GetNormalizedRoleName_ShouldReturnNormalizedName()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_name_1",
            NormalizedName = "role_test_name_1".ToUpper(),
        };

        var normalizedRoleName = await _roleMongoStore.GetNormalizedRoleNameAsync(role, new CancellationToken());
        
        Assert.That(normalizedRoleName, Is.EqualTo(role.NormalizedName));
    }
    
    [Test]
    public async Task SetRoleName_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_name_1",
            NormalizedName = "role_test_name_1".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());

        var roleNameExpected = "new_role_name";

        await _roleMongoStore.SetRoleNameAsync(role, roleNameExpected, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id, new CancellationToken());
        
        Assert.That(roleExist.Name, Is.EqualTo(roleNameExpected));
    }
    
    [Test]
    public async Task SetNormalizedRoleName_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleString()
        {
            Name = "role_test_name_1",
            NormalizedName = "role_test_name_1".ToUpper(),
        };
        
        await _roleMongoStore.CreateAsync(role, new CancellationToken());

        var roleNameExpected = "new_role_name".ToUpper();

        await _roleMongoStore.SetNormalizedRoleNameAsync(role, roleNameExpected, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id, new CancellationToken());
        
        Assert.That(roleExist.NormalizedName, Is.EqualTo(roleNameExpected));
    }
}