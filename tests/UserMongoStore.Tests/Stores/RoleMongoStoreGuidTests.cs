using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Stores;
using MongoDB.Bson;
using MongoDB.Driver;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests.Stores;

public class RoleMongoStoreGuidTests
{
    private RoleMongoStoreAsGuid<ApplicationRoleGuid> _roleMongoStore;

    private const string ConnectionString = "mongodb://localhost:27017";
    
    [SetUp]
    public void Setup()
    {
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        
        var client =
            new MongoClient(ConnectionString);

        var database = client.GetDatabase("RoleMongoStoreGuidTests");
        
        var roleCollection = database.GetCollection<ApplicationRoleGuid>("ApplicationRole");

        _roleMongoStore =
            new RoleMongoStoreAsGuid<ApplicationRoleGuid>(new IdentityErrorDescriber(), roleCollection);
    }
    
    [TearDown]
    public async Task TearDown()
    {
        var client =
            new MongoClient(ConnectionString);

        var database = client.GetDatabase("RoleMongoStoreGuidTests");

        var collections = (await database.ListCollectionNamesAsync()).ToEnumerable();
        
        foreach (var collection in collections)
        {
            await database.DropCollectionAsync(collection);
        }
    }

    [Test]
    public async Task CreateRole_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleGuid()
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
        var role = new ApplicationRoleGuid()
        {
            Name = "role_test_2",
            NormalizedName = "role_test_2".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());
        
        Assert.That(roleExist, Is.Not.Null);
        
        Assert.That(roleExist.Id.Equals(role.Id), Is.True);
    }
    
    [Test]
    public async Task FindByNormalizedName_ShouldReturnRole()
    {
        var role = new ApplicationRoleGuid()
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
        var role = new ApplicationRoleGuid()
        {
            Name = "role_test_4",
            NormalizedName = "role_test_4".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());

        role.NormalizedName = "new_role_name".ToUpper();
        
        role.Name = "new_role_name";

        await _roleMongoStore.UpdateAsync(role, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());
        
        Assert.Multiple(() =>
        {
            Assert.That(roleExist.NormalizedName.Equals(role.NormalizedName), Is.True);
            Assert.That(roleExist.Name.Equals(role.Name), Is.True);
        });
    }
    
    [Test]
    public async Task Delete_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleGuid()
        {
            Name = "role_test_5",
            NormalizedName = "role_test_5".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        var deleteResult = await _roleMongoStore.DeleteAsync(role, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());
        
        Assert.Multiple(() =>
        {
            Assert.That(deleteResult.Succeeded, Is.True);
            Assert.That(roleExist, Is.Null);
        });
    }
    
    [Test]
    public async Task GetRoleId_ShouldReturnIdToString()
    {
        var role = new ApplicationRoleGuid()
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
        var role = new ApplicationRoleGuid()
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
        var role = new ApplicationRoleGuid()
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
        var role = new ApplicationRoleGuid()
        {
            Name = "role_test_name_1",
            NormalizedName = "role_test_name_1".ToUpper(),
        };

        await _roleMongoStore.CreateAsync(role, new CancellationToken());

        var roleNameExpected = "new_role_name";

        await _roleMongoStore.SetRoleNameAsync(role, roleNameExpected, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());
        
        Assert.That(roleExist.Name, Is.EqualTo(roleNameExpected));
    }
    
    [Test]
    public async Task SetNormalizedRoleName_ShouldReturnSuccess()
    {
        var role = new ApplicationRoleGuid()
        {
            Name = "role_test_name_1",
            NormalizedName = "role_test_name_1".ToUpper(),
        };
        
        await _roleMongoStore.CreateAsync(role, new CancellationToken());

        var roleNameExpected = "new_role_name".ToUpper();

        await _roleMongoStore.SetNormalizedRoleNameAsync(role, roleNameExpected, new CancellationToken());
        
        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());
        
        Assert.That(roleExist.NormalizedName, Is.EqualTo(roleNameExpected));
    }
    
    [Test]
    public async Task AddClaim_ShouldSuccess()
    {
        var role = new ApplicationRoleGuid()
        {
            Name = "role_claim_1",
            NormalizedName = "role_claim_1".ToUpper(),
        };
        
        await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        var claim = new Claim("type_1", "value_1", "value_type_1"); 

        await _roleMongoStore.AddClaimAsync(role, claim);

        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());
        
        Assert.That(roleExist.Claims, Has.Count.EqualTo(1));
        
        Assert.Multiple(() =>
        {
            Assert.That(roleExist.Claims[0].ValueType, Is.EqualTo(claim.ValueType));
            Assert.That(roleExist.Claims[0].Value, Is.EqualTo(claim.Value));
            Assert.That(roleExist.Claims[0].Type, Is.EqualTo(claim.Type));
            Assert.That(roleExist.Claims[0].Issuer, Is.EqualTo(claim.Issuer));
        });
    }
    
    [Test]
    public async Task GetClaims_ShouldReturnListOfClaims()
    {
        var role = new ApplicationRoleGuid()
        {
            Name = "role_claim_2",
            NormalizedName = "role_claim_2".ToUpper(),
        };

        var claim1 = new MongoClaim("type_1", "value_1", "value_type_1", "issuer_1");
        var claim2 = new MongoClaim("type_2", "value_2", "value_type_2", "issuer_2");
        
        role.Claims.Add(claim1);
        
        role.Claims.Add(claim2);

        await _roleMongoStore.CreateAsync(role, new CancellationToken());

        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());
        
        var claims = await _roleMongoStore.GetClaimsAsync(roleExist);

        Assert.That(claims, Has.Count.EqualTo(2));
        
        Assert.Multiple(() =>
        {
            Assert.That(claims[0].ValueType, Is.EqualTo(claim1.ValueType));
            Assert.That(claims[0].Value, Is.EqualTo(claim1.Value));
            Assert.That(claims[0].Type, Is.EqualTo(claim1.Type));
            Assert.That(claims[0].Issuer, Is.EqualTo(claim1.Issuer));
            
            Assert.That(claims[1].ValueType, Is.EqualTo(claim2.ValueType));
            Assert.That(claims[1].Value, Is.EqualTo(claim2.Value));
            Assert.That(claims[1].Type, Is.EqualTo(claim2.Type));
            Assert.That(claims[1].Issuer, Is.EqualTo(claim2.Issuer));
        });
    }
    
    [Test]
    public async Task RemoveClaim_ShouldReturnListOfClaims()
    {
        var role = new ApplicationRoleGuid()
        {
            Name = "role_claim_2",
            NormalizedName = "role_claim_2".ToUpper(),
        };

        var claim1 = new MongoClaim("type_1", "value_1", "value_type_1", "issuer_1");
        var claim2 = new MongoClaim("type_2", "value_2", "value_type_2", "issuer_2");
        
        role.Claims.Add(claim1);
        
        role.Claims.Add(claim2);

        await _roleMongoStore.CreateAsync(role, new CancellationToken());
        
        await _roleMongoStore.RemoveClaimAsync(role, new Claim("type_1", "value_1", "value_type_1", "issuer_1"));

        var roleExist = await _roleMongoStore.FindByIdAsync(role.Id.ToString(), new CancellationToken());

        var claims = await _roleMongoStore.GetClaimsAsync(roleExist);
        
        Assert.That(claims, Has.Count.EqualTo(1));
        
        Assert.Multiple(() =>
        {
            Assert.That(claims[0].ValueType, Is.EqualTo(claim2.ValueType));
            Assert.That(claims[0].Value, Is.EqualTo(claim2.Value));
            Assert.That(claims[0].Type, Is.EqualTo(claim2.Type));
            Assert.That(claims[0].Issuer, Is.EqualTo(claim2.Issuer));
        });
    }
}