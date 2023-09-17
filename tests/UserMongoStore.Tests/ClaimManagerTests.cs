using System.Security.Claims;
using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Claim;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Role;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests;

public class ClaimManagerTests
{
    [Test]
    public void HasWithProperties_ShouldReturnOutObject()
    {
        var user = new ApplicationUserGuid();

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var claim2 = new MongoClaim("type2", "value2", "valueType2", "issuer2");
        
        user.Claims.Add(claim1);
        
        user.Claims.Add(claim2);
        
        var claimManager = new ClaimManager<Guid>(user);
        
        claimManager.Has(claim1.Type, claim1.Value, out var myClaim);

        claimManager.Has(claim2.Type, claim2.Value, out var myClaim2);
        
        Assert.Multiple(() =>
        {
            Assert.That(myClaim.Type, Is.EqualTo(claim1.Type));
            Assert.That(myClaim2.Type, Is.EqualTo(claim2.Type));
            
            Assert.That(myClaim.Value, Is.EqualTo(claim1.Value));
            Assert.That(myClaim2.Value, Is.EqualTo(claim2.Value));
            
            Assert.That(myClaim.ValueType, Is.EqualTo(claim1.ValueType));
            Assert.That(myClaim2.ValueType, Is.EqualTo(claim2.ValueType));
            
            Assert.That(myClaim.Issuer, Is.EqualTo(claim1.Issuer));
            Assert.That(myClaim2.Issuer, Is.EqualTo(claim2.Issuer));
        });
    }
    
    [Test]
    public void HasWithProperties_ShouldReturnTrue()
    {
        var user = new ApplicationUserGuid();
        
        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var claim2 = new MongoClaim("type2", "value2", "valueType2", "issuer2");
        
        user.Claims.Add(claim1);
        
        user.Claims.Add(claim2);
        
        var claimManager = new ClaimManager<Guid>(user);
        
        var result1 = claimManager.Has(claim1.Type, claim1.Value);

        var result2 = claimManager.Has(claim2.Type, claim2.Value);
        
        Assert.Multiple(() =>
        {
            Assert.That(result1, Is.True);

            Assert.That(result2, Is.True);
        });
    }
    
    [Test]
    public void TryAdd_ShouldReturnTrue()
    {
        var claimManager = new ClaimManager<Guid>(new ApplicationUserGuid());

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var result = claimManager.TryAdd(claim1.Value, claim1.Type, claim1.ValueType, claim1.Issuer);
        
        Assert.That(result, Is.True);

        var claimExist = claimManager.Has(claim1.Type, claim1.Value);
        
        Assert.That(claimExist, Is.True);
    }
    
    [Test]
    public void TryAddWithClaims_ShouldReturnTrue()
    {
        var claimManager = new ClaimManager<Guid>(new ApplicationUserGuid());

        var claims = new List<Claim>()
        {
            new("type1", "value1", "valueType1"),
            new("type2", "value2", "valueType2")
        };
        
        var result = claimManager.TryAdd(claims);
        
        var claimExist = claimManager.Has("type1", "value1");

        var claimExist2 = claimManager.Has("type2", "value2");
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);

            Assert.That(claimExist, Is.True);

            Assert.That(claimExist2, Is.True);
        });
    }

    [Test]
    public void TryAddAlreadyExist_ShouldReturnFalse()
    {
        var claimManager = new ClaimManager<Guid>(new ApplicationUserGuid());

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var result = claimManager.TryAdd(claim1.Value, claim1.Type, claim1.ValueType, claim1.Issuer);
        
        Assert.That(result, Is.True);

        var claimExist = claimManager.Has(claim1.Type, claim1.Value);
        
        Assert.That(claimExist, Is.True);

        var resultFalse = claimManager.TryAdd(claim1.Value, claim1.Type, claim1.ValueType, claim1.Issuer);
        
        Assert.That(resultFalse, Is.False);
    }
    
    [Test]
    public void TryRemove_ShouldReturnTrue()
    {
        var claimManager = new ClaimManager<Guid>(new ApplicationUserGuid());

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var result = claimManager.TryAdd(claim1.Value, claim1.Type, claim1.ValueType, claim1.Issuer);
        
        Assert.That(result, Is.True);

        var deleteResult = claimManager.TryRemove(new List<Claim>() { new ("type1", "value1", "valueType1")});
        
        Assert.That(deleteResult, Is.True);
        
        var claimExist = claimManager.Has(claim1.Type, claim1.Value);
        
        Assert.That(claimExist, Is.False);
    }
    
    [Test]
    public void TryRemoveClaim_ShouldReturnTrue()
    {
        var claimManager = new ClaimManager<Guid>(new ApplicationUserGuid());

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var result = claimManager.TryAdd(claim1.Value, claim1.Type, claim1.ValueType, claim1.Issuer);
        
        Assert.That(result, Is.True);

        var deleteResult = claimManager.TryRemove( new Claim("type1", "value1", "valueType1"));
        
        Assert.That(deleteResult, Is.True);
        
        var claimExist = claimManager.Has(claim1.Type, claim1.Value);
        
        Assert.That(claimExist, Is.False);
    }
    
    [Test]
    public void TryRemoveNotExist_ShouldReturnFalse()
    {
        var claimManager = new ClaimManager<Guid>(new ApplicationUserGuid());
        
        var deleteResult = claimManager.TryRemove(new List<Claim>() { new Claim("type1", "value2")});
        
        Assert.That(deleteResult, Is.False);
    }

    [Test]
    public void GetAll_ShouldReturnListOfClaims()
    {
        var user = new ApplicationUserGuid();

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var claim2 = new MongoClaim("type2", "value2", "valueType2", "issuer2");
        
        user.Claims.Add(claim1);
        
        user.Claims.Add(claim2);
        
        var claimManager = new ClaimManager<Guid>(user);

        var results = claimManager.GetAll();

        var resultsExpected = new List<Claim>()
        {
            new("type1", "value1", "valueType1", "issuer1"),
            new("type2", "value2", "valueType2", "issuer2"),
        };
        
        Assert.Multiple(() =>
        {
            Assert.That(results, Has.Count.EqualTo(resultsExpected.Count));

            for (var i = 0; i < resultsExpected.Count; i++)
            {
                var myClaim = results[i];
                var claimExpected = resultsExpected[i];
                
                Assert.That(myClaim.Type, Is.EqualTo(claimExpected.Type));
                Assert.That(myClaim.ValueType, Is.EqualTo(claimExpected.ValueType));
                Assert.That(myClaim.Value, Is.EqualTo(claimExpected.Value));
                Assert.That(myClaim.Issuer, Is.EqualTo(claimExpected.Issuer));
            }
        });
    }
    
    
    [Test]
    public void FindBy_ShouldReturnIEnumerable()
    {
        var user = new ApplicationUserGuid();

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var claim2 = new MongoClaim("type1", "value1", "valueType2", "issuer2");
        
        user.Claims.Add(claim1);
        
        user.Claims.Add(claim2);
        
        var claimManager = new ClaimManager<Guid>(user);

        var results = claimManager.FindBy("type1", "value1");
        
        var firstElem = results.ElementAt(0);

        var secondElem = results.ElementAt(1);
        
        Assert.Multiple(() =>
        {
            Assert.That(results, Is.InstanceOf<List<MongoClaim>>());

            Assert.That(results.Count(), Is.EqualTo(2));

            Assert.That(firstElem, Is.EqualTo(claim1));
            Assert.That(secondElem, Is.EqualTo(claim2));
        });
    }

    [Test]
    public void TryReplace_ShouldReturnTrue()
    {
        var user = new ApplicationUserGuid();

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var claim2 = new MongoClaim("type1", "value1", "valueType2", "issuer2");
        
        user.Claims.Add(claim1);
        
        user.Claims.Add(claim2);
        
        var claim = new Claim("type1", "value1");

        var newClaim = new Claim("type_x", "value_x", "valueType_x");
        
        var claimManager = new ClaimManager<Guid>(user);

        var result = claimManager.TryReplace(claim, newClaim);

        Assert.That(result, Is.True);

        var myClaims = claimManager.GetAll();
        
        foreach (var myClaim in myClaims)
        {
            Assert.Multiple(() =>
            {
                Assert.That(myClaim.ValueType, Is.EqualTo(newClaim.ValueType));
                Assert.That(myClaim.Value, Is.EqualTo(newClaim.Value));
                Assert.That(myClaim.Issuer, Is.EqualTo(newClaim.Issuer));
                Assert.That(myClaim.Type, Is.EqualTo(newClaim.Type));
            });
        }
    }
    
    [Test]
    public void TryReplace_ShouldReturnFalse()
    {
        var user = new ApplicationUserGuid();

        var claim1 = new MongoClaim("type1", "value1", "valueType1", "issuer1");
        
        var claim2 = new MongoClaim("type1", "value1", "valueType2", "issuer2");
        
        user.Claims.Add(claim1);
        
        user.Claims.Add(claim2);
        
        var claim = new Claim("type", "value");

        var newClaim = new Claim("type_x", "value_x", "valueType_x");
        
        var claimManager = new ClaimManager<Guid>(user);

        var result = claimManager.TryReplace(claim, newClaim);

        Assert.That(result, Is.False);
    }
}