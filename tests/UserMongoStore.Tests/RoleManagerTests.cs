using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Role;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests;

public class RoleManagerTests
{
    [Test]
    public void HasWithId_ShouldReturnOutObject()
    {
        var user = new ApplicationUserGuid();

        var mongoRole = new MongoRole<Guid>(Guid.NewGuid(), "role1", "ROLE1");
        
        var mongoRole2 = new MongoRole<Guid>(Guid.NewGuid(), "role2", "ROLE2");
        
        user.Roles.Add(mongoRole);
        
        user.Roles.Add(mongoRole2);
        
        var roleManager = new RoleManager<Guid>(user);
        
        roleManager.Has(mongoRole.Id, out var role);

        roleManager.Has(mongoRole2.Id, out var role2);
        
        Assert.Multiple(() =>
        {
            Assert.That(role.Id, Is.EqualTo(mongoRole.Id));

            Assert.That(role2.Id, Is.EqualTo(mongoRole2.Id));
        });
    }

    [Test]
    public void HasWithNormalizedName_ShouldReturnOutObject()
    {
        var user = new ApplicationUserGuid();

        var mongoRole = new MongoRole<Guid>(Guid.NewGuid(), "role1", "ROLE1");
        
        var mongoRole2 = new MongoRole<Guid>(Guid.NewGuid(), "role2", "ROLE2");
        
        user.Roles.Add(mongoRole);
        
        user.Roles.Add(mongoRole2);
        
        var roleManager = new RoleManager<Guid>(user);
        
        roleManager.Has(mongoRole.NormalizedName, out var role);

        roleManager.Has(mongoRole2.NormalizedName, out var role2);
        
        Assert.Multiple(() =>
        {
            Assert.That(role.Id, Is.EqualTo(mongoRole.Id));

            Assert.That(role2.Id, Is.EqualTo(mongoRole2.Id));
        });
    }

    [Test]
    public void HasWithId_ShouldReturnTrue()
    {
        var user = new ApplicationUserGuid();

        var mongoRole = new MongoRole<Guid>(Guid.NewGuid(), "role1", "ROLE1");
        
        var mongoRole2 = new MongoRole<Guid>(Guid.NewGuid(), "role2", "ROLE2");
        
        user.Roles.Add(mongoRole);
        
        user.Roles.Add(mongoRole2);
        
        var roleManager = new RoleManager<Guid>(user);
        
        var result1 = roleManager.Has(mongoRole.Id);

        var result2 = roleManager.Has(mongoRole2.Id);
        
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.True);

            Assert.That(result2, Is.True);
        });
    }

    [Test]
    public void TryAdd_ShouldReturnTrue()
    {
        var roleManager = new RoleManager<Guid>(new ApplicationUserGuid());

        var mongoRole = new MongoRole<Guid>(Guid.NewGuid(), "role1", "ROLE1");

        var result = roleManager.TryAdd(mongoRole.Id, mongoRole.NormalizedName, mongoRole.Name);
        
        Assert.That(result, Is.True);

        var roleExistById = roleManager.Has(mongoRole.Id);
        
        Assert.That(roleExistById, Is.True);
        
        var roleExistName = roleManager.Has(mongoRole.NormalizedName);
        
        Assert.That(roleExistName, Is.True);
    }
    
    [Test]
    public void TryAddAlreadyExist_ShouldReturnFalse()
    {
        var roleManager = new RoleManager<Guid>(new ApplicationUserGuid());

        var mongoRole = new MongoRole<Guid>(Guid.NewGuid(), "role1", "ROLE1");

        var result = roleManager.TryAdd(mongoRole.Id, mongoRole.NormalizedName, mongoRole.Name);
        
        Assert.That(result, Is.True);

        var resultFalse = roleManager.TryAdd(mongoRole.Id, mongoRole.NormalizedName, mongoRole.Name);
        
        Assert.That(resultFalse, Is.False);
    }

    [Test]
    public void TryRemove_ShouldReturnTrue()
    {
        var roleManager = new RoleManager<Guid>(new ApplicationUserGuid());

        var mongoRole = new MongoRole<Guid>(Guid.NewGuid(), "role1", "ROLE1");

        var result = roleManager.TryAdd(mongoRole.Id, mongoRole.NormalizedName, mongoRole.Name);
        
        Assert.That(result, Is.True);

        var deleteResult = roleManager.TryRemove(mongoRole.NormalizedName);
        
        Assert.That(deleteResult, Is.True);
        
        var roleExist = roleManager.Has(mongoRole.Id);
        
        Assert.That(roleExist, Is.False);
        
        var roleExistName = roleManager.Has(mongoRole.NormalizedName);
        
        Assert.That(roleExistName, Is.False);
    }
    
    [Test]
    public void TryRemoveNotExist_ShouldReturnFalse()
    {
        var roleManager = new RoleManager<Guid>(new ApplicationUserGuid());
        
        var deleteResult = roleManager.TryRemove("NOT_EXIST");
        
        Assert.That(deleteResult, Is.False);
    }
    
    [Test]
    public void GetNames_ShouldReturnCollectionOfString()
    {
        var roleManager = new RoleManager<Guid>(new ApplicationUserGuid());
        
        var mongoRole1 = new MongoRole<Guid>(Guid.NewGuid(), "role1", "ROLE1");

        roleManager.TryAdd(mongoRole1.Id, mongoRole1.NormalizedName, mongoRole1.Name);
        
        var mongoRole2 = new MongoRole<Guid>(Guid.NewGuid(), "role2", "ROLE2");

        roleManager.TryAdd(mongoRole2.Id, mongoRole2.NormalizedName, mongoRole2.Name);

        var rolesNames = roleManager.GetNames();
        
        Assert.That(rolesNames.Count, Is.EqualTo(2));

        var roleNamesExpected = new List<string>()
        {
            "role1", "role2"
        };
        
        Assert.That(rolesNames, Is.EqualTo(roleNamesExpected));
    }
}