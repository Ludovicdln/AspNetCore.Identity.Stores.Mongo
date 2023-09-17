using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

namespace UserMongoStore.Tests;

public sealed class ModelTests
{
    [Test]
    public void CreateIdentityUserAsGuid_WithConstructor_ShouldSuccess()
    {
        var userNameExpected = "myName";
        
        var user = new MongoIdentityUserAsGuid(userNameExpected);
        
        Assert.That(user.UserName, Is.EqualTo(userNameExpected));
    }
    
    [Test]
    public void CreateIdentityUserAsObjectId_WithConstructor_ShouldSuccess()
    {
        var userNameExpected = "myName";
        
        var user = new MongoIdentityUserAsObjectId(userNameExpected);
        
        Assert.That(user.UserName, Is.EqualTo(userNameExpected));
    }
    
    [Test]
    public void CreateIdentityUser_WithConstructor_ShouldSuccess()
    {
        var userNameExpected = "myName";

        var user = new MongoIdentityUser<string>(userNameExpected);
        
        Assert.That(user.UserName, Is.EqualTo(userNameExpected));
    }
    
    [Test]
    public void CreateIdentityRoleAsGuid_WithConstructor_ShouldSuccess()
    {
        var roleNameExpected = "myName";
        
        var role = new MongoIdentityRoleAsGuid(roleNameExpected);
        
        Assert.That(role.Name, Is.EqualTo(roleNameExpected));
    }
    
    [Test]
    public void CreateIdentityRoleAsObjectId_WithConstructor_ShouldSuccess()
    {
        var roleNameExpected = "myName";
        
        var role = new MongoIdentityRoleAsObjectId(roleNameExpected);
        
        Assert.That(role.Name, Is.EqualTo(roleNameExpected));
    }
    
    [Test]
    public void CreateIdentityRole_WithConstructor_ShouldSuccess()
    {
        var roleNameExpected = "myName";
        
        var role = new MongoIdentityRole<string>(roleNameExpected);
        
        Assert.That(role.Name, Is.EqualTo(roleNameExpected));
    }
}