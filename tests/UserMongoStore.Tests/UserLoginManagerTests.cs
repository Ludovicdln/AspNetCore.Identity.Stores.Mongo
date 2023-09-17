using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.UserLogin;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests;

public class UserLoginManagerTests
{
    [Test]
    public void HasWithProperties_ShouldReturnOutObject()
    {
        var user = new ApplicationUserGuid();

        var userLoginInfo = new MongoUserLoginInfo("login1", "key1", "name1");
        
        var userLoginInfo2 = new MongoUserLoginInfo("login2", "key2", "name2");
        
        user.Logins.Add(userLoginInfo);
        
        user.Logins.Add(userLoginInfo2);
        
        var userLoginManager = new UserLoginManager<Guid>(user);
        
        userLoginManager.Has(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName, out var loginInfo);

        userLoginManager.Has(userLoginInfo2.LoginProvider, userLoginInfo2.ProviderKey, userLoginInfo2.ProviderDisplayName, out var loginInfo2);
        
        Assert.Multiple(() =>
        {
            Assert.That(loginInfo.LoginProvider, Is.EqualTo(userLoginInfo.LoginProvider));

            Assert.That(loginInfo2.LoginProvider, Is.EqualTo(userLoginInfo2.LoginProvider));
        });
    }
    
    [Test]
    public void HasWithProperties_ShouldReturnTrue()
    {
        var user = new ApplicationUserGuid();

        var userLoginInfo = new MongoUserLoginInfo("login1", "key1", "name1");
        
        var userLoginInfo2 = new MongoUserLoginInfo("login2", "key2", "name2");
        
        user.Logins.Add(userLoginInfo);
        
        user.Logins.Add(userLoginInfo2);
        
        var userLoginManager = new UserLoginManager<Guid>(user);
        
        var result1 = userLoginManager.Has(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);

        var result2 = userLoginManager.Has(userLoginInfo2.LoginProvider, userLoginInfo2.ProviderKey, userLoginInfo2.ProviderDisplayName);
        
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.True);

            Assert.That(result2, Is.True);
        });
    }
    
    [Test]
    public void TryAdd_ShouldReturnTrue()
    {
        var userLoginManager = new UserLoginManager<Guid>(new ApplicationUserGuid());

        var userLoginInfo = new MongoUserLoginInfo("login1", "key1", "name1");

        var result = userLoginManager.TryAdd(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);
        
        Assert.That(result, Is.True);

        var userLoginExist = userLoginManager.Has(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);
        
        Assert.That(userLoginExist, Is.True);
    }
    
    [Test]
    public void TryAddWithoutDisplayName_ShouldReturnTrue()
    {
        var userLoginManager = new UserLoginManager<Guid>(new ApplicationUserGuid());

        var userLoginInfo = new MongoUserLoginInfo("login1", "key1", null);

        var result = userLoginManager.TryAdd(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);
        
        Assert.That(result, Is.True);

        var userLoginExist = userLoginManager.Has(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
        
        Assert.That(userLoginExist, Is.True);
    }
    
    [Test]
    public void TryAddAlreadyExist_ShouldReturnFalse()
    {
        var userLoginManager = new UserLoginManager<Guid>(new ApplicationUserGuid());

        var userLoginInfo = new MongoUserLoginInfo("login1", "key1", "name1");

        var result = userLoginManager.TryAdd(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);
        
        Assert.That(result, Is.True);

        var userLoginExist = userLoginManager.Has(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);
        
        Assert.That(userLoginExist, Is.True);

        var resultFalse = userLoginManager.TryAdd(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);
        
        Assert.That(resultFalse, Is.False);
    }
    
    [Test]
    public void TryRemove_ShouldReturnTrue()
    {
        var userLoginManager = new UserLoginManager<Guid>(new ApplicationUserGuid());

        var userLoginInfo = new MongoUserLoginInfo("login1", "key1", "name1");

        var result = userLoginManager.TryAdd(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey, userLoginInfo.ProviderDisplayName);
        
        Assert.That(result, Is.True);

        var deleteResult = userLoginManager.TryRemove(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
        
        Assert.That(deleteResult, Is.True);
        
        var userLoginExist = userLoginManager.Has(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
        
        Assert.That(userLoginExist, Is.False);
    }
    
    [Test]
    public void TryRemoveNotExist_ShouldReturnFalse()
    {
        var userLoginManager = new UserLoginManager<Guid>(new ApplicationUserGuid());
        
        var deleteResult = userLoginManager.TryRemove("NOT_EXIST", "NOT_EXIST");
        
        Assert.That(deleteResult, Is.False);
    }
}