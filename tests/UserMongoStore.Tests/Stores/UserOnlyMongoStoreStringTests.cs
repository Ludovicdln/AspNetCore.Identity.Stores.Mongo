using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Stores;
using MongoDB.Bson;
using MongoDB.Driver;
using UserMongoStore.Tests.Models;

namespace UserMongoStore.Tests.Stores;

public sealed class UserOnlyMongoStoreStringTests
{
    private UserOnlyMongoStore<ApplicationUserString> _userMongoStore;

    private const string ConnectionString = "mongodb://localhost:27017";

    [SetUp]
    public void Setup()
    {
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        
        var client =
            new MongoClient(ConnectionString);

        var database = client.GetDatabase("UserOnlyMongoStoreStringTests");

        var userCollection = database.GetCollection<ApplicationUserString>("ApplicationUser");

        _userMongoStore =
            new UserOnlyMongoStore<ApplicationUserString>(new IdentityErrorDescriber(), userCollection);
    }
    
    
    [TearDown]
    public async Task TearDown()
    {
        var client =
            new MongoClient(ConnectionString);

        var database = client.GetDatabase("UserOnlyMongoStoreStringTests");

        var collections = (await database.ListCollectionNamesAsync()).ToEnumerable();
        
        foreach (var collection in collections)
        {
            await database.DropCollectionAsync(collection);
        }
    }

    [Test]
    public async Task CreateUser_ShouldReturnSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test@gmail.com",
            NormalizedEmail = "test@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test",
            NormalizedUserName = "test".ToUpper()
        };

        var identityResult = await _userMongoStore.CreateAsync(user);

        Assert.That(identityResult.Succeeded, Is.True);
    }

    [Test]
    public async Task FindById_ShouldReturnUser()
    {
        var user = new ApplicationUserString()
        {
            Email = "test2@gmail.com",
            NormalizedEmail = "test2@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test2",
            NormalizedUserName = "test2".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var userExist = await _userMongoStore.FindByIdAsync(user.Id);

        Assert.That(userExist, Is.Not.Null);

        Assert.That(userExist.Id.Equals(user.Id), Is.True);
    }

    [Test]
    public async Task FindByNormalizedEmail_ShouldReturnUser()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_email@gmail.com",
            NormalizedEmail = "test_email@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test_email",
            NormalizedUserName = "test_email".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var normalizedEmail = "test_email@gmail.com".ToUpper();

        var userExist = await _userMongoStore.FindByEmailAsync(normalizedEmail);

        Assert.That(userExist, Is.Not.Null);

        Assert.That(userExist.NormalizedEmail.Equals(normalizedEmail), Is.True);
    }

    [Test]
    public async Task FindByName_ShouldReturnUser()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_name@gmail.com",
            NormalizedEmail = "test_name@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test_name",
            NormalizedUserName = "test_name".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var normalizedUserName = "test_name".ToUpper();

        var userExist = await _userMongoStore.FindByNameAsync(normalizedUserName);

        Assert.That(userExist, Is.Not.Null);

        Assert.That(userExist.NormalizedUserName.Equals(normalizedUserName), Is.True);
    }

    [Test]
    public async Task FindByLogin_ShouldReturnNull()
    {
        var userNotExist = await _userMongoStore.FindByLoginAsync(string.Empty, string.Empty);

        Assert.That(userNotExist, Is.Null);
    }

    [Test]
    public async Task FindByLogin_ShouldReturnSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_logins@gmail.com",
            NormalizedEmail = "test_logins@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_logins",
            NormalizedUserName = "test_logins".ToUpper()
        };

        var userLoginInfo = new MongoUserLoginInfo("login_provider_test", "key_provider_test", "login_name_test");

        var userLoginInfo2 =
            new MongoUserLoginInfo("login_provider_test_2", "key_provider_test_2", "login_name_test_2");

        user.Logins.Add(userLoginInfo);

        user.Logins.Add(userLoginInfo2);

        await _userMongoStore.CreateAsync(user);

        var userExist = await _userMongoStore.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);

        var userExist2 =
            await _userMongoStore.FindByLoginAsync(userLoginInfo2.LoginProvider, userLoginInfo2.ProviderKey);

        Assert.Multiple(() =>
        {
            Assert.That(userExist, Is.Not.Null);

            Assert.That(userExist.Id, Is.EqualTo(user.Id));

            Assert.That(userExist2, Is.Not.Null);

            Assert.That(userExist2.Id, Is.EqualTo(user.Id));
        });
    }

    [Test]
    public async Task GetEmail_ShouldReturnEmail()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_name_email@gmail.com",
            NormalizedEmail = "test_name_email@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test_name_email",
            NormalizedUserName = "test_name_email".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var expectedEmail = "test_name_email@gmail.com";

        var userExist = await _userMongoStore.FindByEmailAsync(expectedEmail.ToUpper());

        Assert.That(userExist, Is.Not.Null);

        var email = await _userMongoStore.GetEmailAsync(userExist);

        Assert.That(email, Is.EqualTo(expectedEmail));
    }

    [Test]
    public async Task GetId_ShouldReturnIdToString()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_id@gmail.com",
            NormalizedEmail = "test_id@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test_id",
            NormalizedUserName = "test_id".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var expectedEmail = "test_id@gmail.com";

        var userExist = await _userMongoStore.FindByEmailAsync(expectedEmail.ToUpper());

        Assert.That(userExist, Is.Not.Null);

        var idToString = await _userMongoStore.GetUserIdAsync(userExist);

        Assert.That(idToString, Is.EqualTo(userExist.Id.ToString()));
    }

    [Test]
    public async Task GetUserName_ShouldReturnUserName()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_user_name_1@gmail.com",
            NormalizedEmail = "test_user_name_1@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test_user_name_1",
            NormalizedUserName = "test_user_name_1".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var expectedEmail = "test_user_name_1@gmail.com";

        var userExist = await _userMongoStore.FindByEmailAsync(expectedEmail.ToUpper());

        Assert.That(userExist, Is.Not.Null);

        var userName = await _userMongoStore.GetUserNameAsync(userExist);

        Assert.That(userName, Is.EqualTo(userExist.UserName));
    }

    [Test]
    public void CreateIdentityUserAsGuid_ShouldGenerateGuid()
    {
        var user = new ApplicationUserString()
        {
            Email = "test2@gmail.com",
            NormalizedEmail = "test2@gmail.com".ToUpper(),
            PasswordHash = "123456",
            UserName = "test2",
            NormalizedUserName = "test2".ToUpper()
        };

        Assert.That(user.Id, Is.Not.Empty);

        Assert.That(user.Id, Is.Not.EqualTo(string.Empty));
    }

    [Test]
    public async Task UpdateAllProperties_ShouldReturnSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test4@gmail.com",
            NormalizedEmail = "test4@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test4",
            NormalizedUserName = "test4".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        user.UserName = "test4Updated";
        user.Roles = new List<MongoRole<string>>()
            { new(Guid.NewGuid().ToString(), "role1_test", "ROLE1_TEST"), new(Guid.NewGuid().ToString(), "role2_test", "ROLE2_TEST") };
        user.Claims = new List<MongoClaim>()
            { new("updated_type", "updated_value", "updated_valueType", "updated_issuer") };
        user.Logins = new List<MongoUserLoginInfo>()
            { new("login_test_updated", "key_test_provider", "name_updated") };
        user.Tokens = new List<MongoToken>() { new("test_provider_updated", "test_name_updated") };
        user.PhoneNumber = "3630Updated";
        user.NormalizedUserName = user.UserName.ToUpper();
        user.PasswordHash = "12345678Updated";
        user.Email = "test4@gmail.comUpdated";
        user.NormalizedEmail = user.Email.ToUpper();

        var updateResult = await _userMongoStore.UpdateAsync(user);

        Assert.That(updateResult.Succeeded, Is.True);

        var userExist = await _userMongoStore.FindByEmailAsync(user.NormalizedEmail);

        Assert.Multiple(() =>
        {
            Assert.That(userExist, Is.Not.Null);

            Assert.That(userExist.UserName, Is.EqualTo(user.UserName));

            Assert.That(userExist.PhoneNumber, Is.EqualTo(user.PhoneNumber));

            Assert.That(userExist.NormalizedUserName, Is.EqualTo(user.NormalizedUserName));

            Assert.That(userExist.PasswordHash, Is.EqualTo(user.PasswordHash));

            Assert.That(userExist.Email, Is.EqualTo(user.Email));

            Assert.That(userExist.Roles.Count == 2, Is.True);

            Assert.That(userExist.Tokens.Count == 1, Is.True);

            Assert.That(userExist.Claims.Count == 1, Is.True);

            Assert.That(userExist.Logins.Count == 1, Is.True);
        });
    }

    [Test]
    public async Task Delete_ShouldReturnSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test5@gmail.com",
            NormalizedEmail = "test5@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test5",
            NormalizedUserName = "test5".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var deleteResult = await _userMongoStore.DeleteAsync(user);

        Assert.That(deleteResult.Succeeded, Is.True);

        var userNotExist = await _userMongoStore.FindByEmailAsync(user.NormalizedEmail);

        Assert.That(userNotExist, Is.Null);
    }

    [Test]
    public async Task SetToken_ShouldCreateToken()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_token@gmail.com",
            NormalizedEmail = "test_token@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_token",
            NormalizedUserName = "test_token".ToUpper()
        };

        var tokenExpected = new MongoToken("token_provider", "token_name", "token_value");

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.SetTokenAsync(user, tokenExpected.LoginProvider, tokenExpected.Name, tokenExpected.Value,
            new CancellationToken());

        var userExist = await _userMongoStore.FindByIdAsync(user.Id.ToString());

        var stockedToken = await _userMongoStore.GetTokenAsync(userExist, tokenExpected.LoginProvider,
            tokenExpected.Name, new CancellationToken());

        Assert.Multiple(() =>
        {
            Assert.That(tokenExpected, Is.Not.Null);

            Assert.That(tokenExpected, Is.Not.Null);

            Assert.That(stockedToken, Is.EqualTo(tokenExpected.Value));
        });
    }

    [Test]
    public async Task SetToken_ShouldReplaceValue()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_token_2@gmail.com",
            NormalizedEmail = "test_token_2@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_token_2",
            NormalizedUserName = "test_token_2".ToUpper()
        };

        var tokenExpected = new MongoToken("token_provider", "token_name", "token_value");

        user.Tokens.Add(tokenExpected);

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.SetTokenAsync(user, tokenExpected.LoginProvider, tokenExpected.Name, "newValueOfToken",
            new CancellationToken());

        var userExist = await _userMongoStore.FindByIdAsync(user.Id.ToString());

        var stockedToken = await _userMongoStore.GetTokenAsync(userExist, tokenExpected.LoginProvider,
            tokenExpected.Name, new CancellationToken());

        Assert.Multiple(() =>
        {
            Assert.That(user.Tokens, Has.Count.EqualTo(1));

            Assert.That(stockedToken, Is.EqualTo("newValueOfToken"));
        });
    }

    [Test]
    public async Task RemoveToken_ShouldSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_token_2@gmail.com",
            NormalizedEmail = "test_token_2@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_token_2",
            NormalizedUserName = "test_token_2".ToUpper()
        };

        var tokenExpected = new MongoToken("token_provider", "token_name", "token_value");

        user.Tokens.Add(tokenExpected);

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.RemoveTokenAsync(user, tokenExpected.LoginProvider, tokenExpected.Name,
            new CancellationToken());

        var userExist = await _userMongoStore.FindByIdAsync(user.Id.ToString());

        var stockedToken = await _userMongoStore.GetTokenAsync(userExist, tokenExpected.LoginProvider,
            tokenExpected.Name, new CancellationToken());

        Assert.Multiple(() =>
        {
            Assert.That(userExist.Tokens, Has.Count.EqualTo(0));
            Assert.That(stockedToken, Is.Null);
        });
    }

    [Test]
    public async Task AddLogins_ShouldSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_logins@gmail.com",
            NormalizedEmail = "test_logins@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_logins",
            NormalizedUserName = "test_logins".ToUpper()
        };

        await _userMongoStore.CreateAsync(user);

        var userLoginInfo = new UserLoginInfo("login_provider", "key_provider", "login_name");

        await _userMongoStore.AddLoginAsync(user, userLoginInfo);

        var userExist = await _userMongoStore.FindByIdAsync(user.Id.ToString());

        var logins = await _userMongoStore.GetLoginsAsync(userExist);

        var firstLogin = logins.FirstOrDefault();

        Assert.Multiple(() =>
        {
            Assert.That(logins, Has.Count.EqualTo(1));

            Assert.That(firstLogin, Is.Not.Null);

            Assert.That(firstLogin.LoginProvider, Is.EqualTo(userLoginInfo.LoginProvider));

            Assert.That(firstLogin.ProviderKey, Is.EqualTo(userLoginInfo.ProviderKey));

            Assert.That(firstLogin.ProviderDisplayName, Is.EqualTo(userLoginInfo.ProviderDisplayName));
        });
    }

    [Test]
    public async Task AddLoginsAlreadyExist_ShouldNotSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_logins_2@gmail.com",
            NormalizedEmail = "test_logins_2@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_logins_2",
            NormalizedUserName = "test_logins_2".ToUpper()
        };

        var userLoginInfo = new UserLoginInfo("login_provider", "key_provider", "login_name");

        user.Logins.Add(new MongoUserLoginInfo(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey,
            userLoginInfo.ProviderDisplayName));

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.AddLoginAsync(user, userLoginInfo);

        var userExist = await _userMongoStore.FindByIdAsync(user.Id.ToString());

        var logins = await _userMongoStore.GetLoginsAsync(userExist);

        Assert.That(logins, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task RemoveLogins_ShouldSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_logins_2@gmail.com",
            NormalizedEmail = "test_logins_2@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_logins_2",
            NormalizedUserName = "test_logins_2".ToUpper()
        };

        var userLoginInfo = new UserLoginInfo("login_provider", "key_provider", "login_name");

        user.Logins.Add(new MongoUserLoginInfo(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey,
            userLoginInfo.ProviderDisplayName));

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.AddLoginAsync(user, userLoginInfo);

        await _userMongoStore.RemoveLoginAsync(user, userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);

        var userExist = await _userMongoStore.FindByIdAsync(user.Id.ToString());

        Assert.That(userExist.Logins, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task CreateClaims_ShouldSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_claims@gmail.com",
            NormalizedEmail = "test_claims@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_claims",
            NormalizedUserName = "test_claims".ToUpper()
        };

        var claims = new List<Claim>()
        {
            new("type_1", "value_1"),
            new("type_2", "value_2"),
            new("type_3", "value_3", "value_type_3")
        };

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.AddClaimsAsync(user, claims);

        var userExist = await _userMongoStore.FindByIdAsync(user.Id);

        Assert.That(userExist.Claims, Has.Count.EqualTo(3));

        for (var i = 0; i < userExist.Claims.Count; i++)
        {
            var currentClaim = userExist.Claims[i];
            var expectedClaim = claims[i];

            Assert.Multiple(() =>
            {
                Assert.That(currentClaim.Type, Is.EqualTo(expectedClaim.Type));
                Assert.That(currentClaim.Value, Is.EqualTo(expectedClaim.Value));
                Assert.That(currentClaim.ValueType, Is.EqualTo(expectedClaim.ValueType));
            });
        }
    }

    [Test]
    public async Task GetClaims_ShouldReturnCollectionOfClaims()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_claims@gmail.com",
            NormalizedEmail = "test_claims@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_claims",
            NormalizedUserName = "test_claims".ToUpper()
        };

        user.Claims = new List<MongoClaim>()
        {
            new("type_1", "value_1", "value_type_1", "issuer_1"),
            new("type_2", "value_2", "value_type_2", "issuer_2"),
            new("type_3", "value_3", "value_type_3", "issuer_3")
        };

        var claimsExpected = new List<Claim>()
        {
            new("type_1", "value_1", "value_type_1", "issuer_1"),
            new("type_2", "value_2", "value_type_2", "issuer_2"),
            new("type_3", "value_3", "value_type_3", "issuer_3")
        };

        await _userMongoStore.CreateAsync(user);

        var claims = await _userMongoStore.GetClaimsAsync(user);

        Assert.That(claims, Has.Count.EqualTo(3));

        for (var i = 0; i < claims.Count; i++)
        {
            var currentClaim = user.Claims[i];
            var expectedClaim = claims[i];

            Assert.Multiple(() =>
            {
                Assert.That(currentClaim.Type, Is.EqualTo(expectedClaim.Type));
                Assert.That(currentClaim.Value, Is.EqualTo(expectedClaim.Value));
                Assert.That(currentClaim.ValueType, Is.EqualTo(expectedClaim.ValueType));
            });
        }
    }

    [Test]
    public async Task RemoveClaims_ShouldSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_claims@gmail.com",
            NormalizedEmail = "test_claims@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_claims",
            NormalizedUserName = "test_claims".ToUpper()
        };

        var claimsExpected = new List<Claim>()
        {
            new("type_1", "value_1", "value_type_1", "issuer_1"),
            new("type_2", "value_2", "value_type_2", "issuer_2"),
            new("type_3", "value_3", "value_type_3", "issuer_3")
        };

        user.Claims = new List<MongoClaim>()
        {
            new("type_1", "value_1", "value_type_1", "issuer_1"),
            new("type_2", "value_2", "value_type_2", "issuer_2"),
            new("type_3", "value_3", "value_type_3", "issuer_3")
        };

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.RemoveClaimsAsync(user, claimsExpected);

        var userExist = await _userMongoStore.FindByIdAsync(user.Id);

        Assert.That(userExist.Claims, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetUsersForClaims_ShouldReturnUsers()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_claims@gmail.com",
            NormalizedEmail = "test_claims@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_claims",
            NormalizedUserName = "test_claims".ToUpper()
        };

        var user2 = new ApplicationUserString()
        {
            Email = "test_claims_2@gmail.com",
            NormalizedEmail = "test_claims_2@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_claims_2",
            NormalizedUserName = "test_claims_2".ToUpper()
        };

        user.Claims = new List<MongoClaim>()
        {
            new("type_1", "value_1", "value_type_1", "issuer_1"),
            new("type_2", "value_2", "value_type_2", "issuer_2"),
            new("type_3", "value_3", "value_type_3", "issuer_3")
        };

        user2.Claims = new List<MongoClaim>()
        {
            new("type_1", "value_1", "value_type_1", "issuer_1"),
            new("type_2", "value_2", "value_type_2", "issuer_2"),
            new("type_3", "value_3", "value_type_3", "issuer_3")
        };

        await _userMongoStore.CreateAsync(user);

        await _userMongoStore.CreateAsync(user2);

        var users = await _userMongoStore.GetUsersForClaimAsync(new Claim("type_1", "value_1", "value_type_1",
            "issuer_1"));

        Assert.That(users, Has.Count.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task ReplaceClaims_ShouldReturnSuccess()
    {
        var user = new ApplicationUserString()
        {
            Email = "test_claims@gmail.com",
            NormalizedEmail = "test_claims@gmail.com".ToUpper(),
            PasswordHash = "12345678",
            UserName = "test_claims",
            NormalizedUserName = "test_claims".ToUpper()
        };

        user.Claims = new List<MongoClaim>()
        {
            new("type_1", "value_1", "value_type_1", "issuer_1"),
            new("type_1", "value_1", "value_type_2", "issuer_2"),
            new("type_1", "value_1", "value_type_3", "issuer_3")
        };

        await _userMongoStore.CreateAsync(user);

        var claim = new Claim("type_1", "value_1", "value_type_1", "issuer_1");
        var newClaim = new Claim("type_1_new", "value_1_new", "value_type_1_new", "issuer_1_new");

        await _userMongoStore.ReplaceClaimAsync(user, claim, newClaim);

        var userExist = await _userMongoStore.FindByIdAsync(user.Id.ToString());

        for (var i = 0; i < userExist.Claims.Count; i++)
        {
            var currentClaim = userExist.Claims[i];

            Assert.Multiple(() =>
            {
                Assert.That(currentClaim.Type, Is.EqualTo(newClaim.Type));
                Assert.That(currentClaim.Value, Is.EqualTo(newClaim.Value));
                Assert.That(currentClaim.ValueType, Is.EqualTo(newClaim.ValueType));
                Assert.That(currentClaim.Issuer, Is.EqualTo(newClaim.Issuer));
            });
        }
    }
}