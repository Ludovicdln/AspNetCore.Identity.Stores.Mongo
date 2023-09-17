using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Role;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Token;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests;

public class TokenManagerTests
{
    [Test]
    public void HasWithProperties_ShouldReturnOutObject()
    {
        var user = new ApplicationUserGuid();

        var mongoToken = new MongoToken("login1", "name1", "value1");
        
        var mongoToken1 = new MongoToken("login2", "name2", "value2");
        
        user.Tokens.Add(mongoToken);
        
        user.Tokens.Add(mongoToken1);
        
        var tokenManager = new TokenManager<Guid>(user);
        
        tokenManager.Has(mongoToken.LoginProvider, mongoToken.Name, out var token);

        tokenManager.Has(mongoToken1.LoginProvider, mongoToken1.Name, out var token2);
        
        tokenManager.Has(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value, out var token3);

        tokenManager.Has(mongoToken1.LoginProvider, mongoToken1.Name, mongoToken1.Value, out var token4);
        
        Assert.Multiple(() =>
        {
            Assert.That(token.LoginProvider, Is.EqualTo(mongoToken.LoginProvider));
            Assert.That(token.Name, Is.EqualTo(mongoToken.Name));
            
            Assert.That(token2.LoginProvider, Is.EqualTo(mongoToken1.LoginProvider));
            Assert.That(token2.Name, Is.EqualTo(mongoToken1.Name));
            
            Assert.That(token3.LoginProvider, Is.EqualTo(mongoToken.LoginProvider));
            Assert.That(token3.Value, Is.EqualTo(mongoToken.Value));
            Assert.That(token3.Name, Is.EqualTo(mongoToken.Name));
            
            Assert.That(token4.LoginProvider, Is.EqualTo(mongoToken1.LoginProvider));
            Assert.That(token4.Value, Is.EqualTo(mongoToken1.Value));
            Assert.That(token4.Name, Is.EqualTo(mongoToken1.Name));
        });
    }
    
    [Test]
    public void HasWithProperties_ShouldReturnTrue()
    {
        var user = new ApplicationUserGuid();

        var mongoToken = new MongoToken("login1", "name1", "value1");
        
        var mongoToken1 = new MongoToken("login2", "name2", "value2");
        
        user.Tokens.Add(mongoToken);
        
        user.Tokens.Add(mongoToken1);
        
        var tokenManager = new TokenManager<Guid>(user);
        
        var result = tokenManager.Has(mongoToken.LoginProvider, mongoToken.Name);

        var result2 = tokenManager.Has(mongoToken1.LoginProvider, mongoToken1.Name);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(result2, Is.True);
        });
    }
    
    [Test]
    public void TryAdd_ShouldReturnTrue()
    {
        var tokenManager = new TokenManager<Guid>(new ApplicationUserGuid());

        var mongoToken = new MongoToken("login1", "name1", "value1");
        
        var result = tokenManager.TryAdd(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(result, Is.True);

        var tokenExist = tokenManager.Has(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(tokenExist, Is.True);
    }
    
    [Test]
    public void TryAddAlreadyExist_ShouldReturnFalse()
    {
        var tokenManager = new TokenManager<Guid>(new ApplicationUserGuid());

        var mongoToken = new MongoToken("login1", "name1", "value1");
        
        var result = tokenManager.TryAdd(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(result, Is.True);

        var tokenExist = tokenManager.Has(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(tokenExist, Is.True);

        var resultFalse = tokenManager.TryAdd(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(resultFalse, Is.False);
    }
    
    [Test]
    public void TryRemove_ShouldReturnTrue()
    {
        var tokenManager = new TokenManager<Guid>(new ApplicationUserGuid());

        var mongoToken = new MongoToken("login1", "name1", "value1");
        
        var result = tokenManager.TryAdd(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(result, Is.True);

        var deleteResult = tokenManager.TryRemove(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(deleteResult, Is.True);
        
        var tokenExist = tokenManager.Has(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(tokenExist, Is.False);
    }
    
    [Test]
    public void TryReplace_ShouldReturnTrue()
    {
        var tokenManager = new TokenManager<Guid>(new ApplicationUserGuid());

        var mongoToken = new MongoToken("login1", "name1", "value1");
        
        var result = tokenManager.TryAdd(mongoToken.LoginProvider, mongoToken.Name, mongoToken.Value);
        
        Assert.That(result, Is.True);

        var replaceResult = tokenManager.TryReplace(mongoToken.LoginProvider, mongoToken.Name, "newValueToken");
        
        Assert.That(replaceResult, Is.True);
    }
    
    [Test]
    public void TryReplace_ShouldReturnFalse()
    {
        var tokenManager = new TokenManager<Guid>(new ApplicationUserGuid());

        var mongoToken = new MongoToken("login1", "name1", "value1");
        
        var result = tokenManager.TryAdd("Not_exist", mongoToken.Name, mongoToken.Value);
        
        Assert.That(result, Is.True);

        var replaceResult = tokenManager.TryReplace(mongoToken.LoginProvider, mongoToken.Name, "newValueToken");
        
        Assert.That(replaceResult, Is.False);
    }
    
    [Test]
    public void TryRemoveNotExist_ShouldReturnFalse()
    {
        var tokenManager = new TokenManager<Guid>(new ApplicationUserGuid());
        
        var deleteResult = tokenManager.TryRemove("NULL", "NULL", "NULL");
        
        Assert.That(deleteResult, Is.False);
    }
}