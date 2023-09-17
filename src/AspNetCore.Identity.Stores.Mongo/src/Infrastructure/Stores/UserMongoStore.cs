using System.Collections.ObjectModel;
using System.Security.Claims;
using Amazon.Auth.AccessControlPolicy;
using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Extensions;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Claim;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Role;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Token;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.UserLogin;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Stores;

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TRole">The type representing a role.</typeparam>
public class UserMongoStoreAsObjectId<TUser, TRole> : UserMongoStore<TUser, TRole, ObjectId, IdentityUserClaim<ObjectId>,
    IdentityUserRole<ObjectId>, MongoIdentityUserLogin<ObjectId>, IdentityUserToken<ObjectId>, IdentityRoleClaim<ObjectId>>
    where TUser : MongoIdentityUser<ObjectId>, new()
    where TRole : MongoIdentityRole<ObjectId>, new()
{
    public UserMongoStoreAsObjectId(IdentityErrorDescriber describer, IMongoCollection<TUser> userCollection, IMongoCollection<TRole> roleCollection) 
        : base(describer, userCollection, roleCollection)
    {
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var result = await UserCollection.DeleteOneAsync(
            new BsonDocumentFilterDefinition<TUser>(new BsonDocument("_id", user.Id)), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "DeleteError", Description = $"Failed to delete user {user.Id}" });
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var result = await UserCollection.ReplaceOneAsync(new BsonDocumentFilterDefinition<TUser>(new BsonDocument("_id", 
            user.Id)), user, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "UpdateError", Description = "Failed to update user" });
    }

    /// <inheritdoc/>
    public override async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("_id", ObjectId.Parse(userId))), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<TUser?> FindUserAsync(ObjectId userId, CancellationToken cancellationToken)
    {
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("_id", userId)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }
    
    /// <inheritdoc/>
    protected override async Task<MongoIdentityUserLogin<ObjectId>?> FindUserLoginAsync(ObjectId userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();

        var matchUserIdDocument = new BsonDocument("$match", new BsonDocument("_id", userId));
        
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

        return (await UserCollection.AggregateAsync(
            new BsonDocumentStagePipelineDefinition<TUser, MongoIdentityUserLogin<ObjectId>>(new[]
                { matchUserIdDocument, unwindDocument, matchDocument, projectDocument }), cancellationToken: cancellationToken).ConfigureAwait(false)).FirstOrDefault(cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateLoginsAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Logins", new BsonArray(user.Logins.Select(x => x.ToBsonDocument()))));
        
        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Roles", new BsonArray(user.Roles.Select(x => x.ToBsonDocument()))));

        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateTokensAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Tokens", new BsonArray(user.Tokens.Select(x => x.ToBsonDocument()))));

        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateClaimsAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Claims", new BsonArray(user.Claims.Select(x => x.ToBsonDocument()))));

        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);

        if (roleEntity is null) return new List<TUser>();

        var matchDocument = new BsonDocument("Roles",
            new BsonDocument("$elemMatch",
                new BsonDocument("_id",roleEntity.Id)));

        return await (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            cancellationToken: cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken: cancellationToken);
    }
}

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TRole">The type representing a role.</typeparam>
public class UserMongoStoreAsGuid<TUser, TRole> : UserMongoStore<TUser, TRole, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>, MongoIdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>
    where TUser : MongoIdentityUser<Guid>, new()
    where TRole : MongoIdentityRole<Guid>, new()
{
    public UserMongoStoreAsGuid(IdentityErrorDescriber describer, IMongoCollection<TUser> userCollection, IMongoCollection<TRole> roleCollection) 
        : base(describer, userCollection, roleCollection)
    {
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var result = await UserCollection.DeleteOneAsync(
            new BsonDocumentFilterDefinition<TUser>(new BsonDocument("_id", user.Id)), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "DeleteError", Description = $"Failed to delete user {user.Id}" });
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var result = await UserCollection.ReplaceOneAsync(new BsonDocumentFilterDefinition<TUser>(new BsonDocument("_id", 
            user.Id)), user, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "UpdateError", Description = "Failed to update user" });
    }

    /// <inheritdoc/>
    public override async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("_id", Guid.Parse(userId))), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<TUser?> FindUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("_id", userId)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }
    
    /// <inheritdoc/>
    protected override async Task<MongoIdentityUserLogin<Guid>?> FindUserLoginAsync(Guid userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();

        var matchUserIdDocument = new BsonDocument("$match", new BsonDocument("_id", userId));
        
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

        return (await UserCollection.AggregateAsync(
            new BsonDocumentStagePipelineDefinition<TUser, MongoIdentityUserLogin<Guid>>(new[]
                { matchUserIdDocument, unwindDocument, matchDocument, projectDocument }), cancellationToken: cancellationToken).ConfigureAwait(false)).FirstOrDefault(cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateLoginsAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Logins", new BsonArray(user.Logins.Select(x => x.ToBsonDocument()))));
        
        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Roles", new BsonArray(user.Roles.Select(x => x.ToBsonDocument()))));

        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateTokensAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Tokens", new BsonArray(user.Tokens.Select(x => x.ToBsonDocument()))));

        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateClaimsAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", user.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Claims", new BsonArray(user.Claims.Select(x => x.ToBsonDocument()))));

        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);

        if (roleEntity is null) return new List<TUser>();

        var matchDocument = new BsonDocument("Roles",
            new BsonDocument("$elemMatch",
                new BsonDocument("_id",roleEntity.Id)));

        return await (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            cancellationToken: cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken: cancellationToken);
    }
}

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TRole">The type representing a role.</typeparam>
public class UserMongoStore<TUser, TRole> : UserMongoStore<TUser, TRole, string, IdentityUserClaim<string>,
    IdentityUserRole<string>, MongoIdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
    where TUser : MongoIdentityUser<string>, new()
    where TRole : MongoIdentityRole<string>, new()
{
    public UserMongoStore(IdentityErrorDescriber describer, IMongoCollection<TUser> userCollection, IMongoCollection<TRole> roleCollection) 
        : base(describer, userCollection, roleCollection)
    {
    }
}

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TRole">The type representing a role.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
public class UserMongoStore<TUser, TRole, TKey> : UserMongoStore<TUser, TRole, TKey, IdentityUserClaim<TKey>,
    IdentityUserRole<TKey>, MongoIdentityUserLogin<TKey>, IdentityUserToken<TKey>, IdentityRoleClaim<TKey>>
    where TUser : MongoIdentityUser<TKey>, new()
    where TRole : MongoIdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
{
    public UserMongoStore(IdentityErrorDescriber describer, IMongoCollection<TUser> userCollection, IMongoCollection<TRole> roleCollection) 
        : base(describer, userCollection, roleCollection)
    {
    }
}

/// <summary>
/// Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TRole">The type representing a role.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
/// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
/// <typeparam name="TUserRole">The type representing a user role.</typeparam>
/// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
/// <typeparam name="TUserToken">The type representing a user token.</typeparam>
/// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
public class UserMongoStore<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>
    : UserStoreBase<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> 
    where TUser : MongoIdentityUser<TKey>, new()
    where TRole : MongoIdentityRole<TKey>, new()
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>, new()
    where TUserRole : IdentityUserRole<TKey>, new()
    where TUserLogin : MongoIdentityUserLogin<TKey>, new()
    where TUserToken : IdentityUserToken<TKey>, new()
    where TRoleClaim : IdentityRoleClaim<TKey>, new()
{
    protected IMongoCollection<TUser> UserCollection { get; }
    
    protected IMongoCollection<TRole> RoleCollection { get; }

    protected UserMongoStore(IdentityErrorDescriber describer, IMongoCollection<TUser> userCollection, 
        IMongoCollection<TRole> roleCollection) : base(describer)
    {
        UserCollection = userCollection;
        RoleCollection = roleCollection;
    }

    /// <summary>
    /// Creates the specified <paramref name="user"/> in the user store.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
    public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);

        await UserCollection.InsertOneAsync(user, null, cancellationToken).ConfigureAwait(false);
        
        return IdentityResult.Success;
    }

    /// <summary>
    /// Updates the specified <paramref name="user"/> in the user store.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
    public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var result = await UserCollection.ReplaceOneAsync(new BsonDocumentFilterDefinition<TUser>(new BsonDocument("_id", 
            ConvertIdToString(user.Id))), user, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "UpdateError", Description = "Failed to update user" });
    }

    /// <summary>
    /// Deletes the specified <paramref name="user"/> from the user store.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
    public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);
        
        var result = await UserCollection.DeleteOneAsync(
            new BsonDocumentFilterDefinition<TUser>(new BsonDocument("_id", ConvertIdToString(user.Id))), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "DeleteError", Description = $"Failed to delete user {user.Id}" });
    }

    /// <summary>
    /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
    /// </returns>
    public override async Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("_id", userId)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Finds and returns a user, if any, who has the specified normalized user name.
    /// </summary>
    /// <param name="normalizedUserName">The normalized user name to search for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
    /// </returns>
    public override async Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("NormalizedUserName", normalizedUserName)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Return a user with the matching userId if it exists.
    /// </summary>
    /// <param name="userId">The user's id.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The user if it exists.</returns>
    protected override async Task<TUser?> FindUserAsync(TKey userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("_id", ConvertIdToString(userId))), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleOrDefault(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Return a user login with the matching userId, provider, providerKey if it exists.
    /// </summary>
    /// <param name="userId">The user's id.</param>
    /// <param name="loginProvider">The login provider name.</param>
    /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The user login if it exists.</returns>
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

    /// <summary>
    /// Return a user login with the matching provider, providerKey if it exists.
    /// </summary>
    /// <param name="loginProvider">The login provider name.</param>
    /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The user login if it exists.</returns>
    protected override async Task<TUserLogin?> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();

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
                { unwindDocument, matchDocument, projectDocument }), cancellationToken: cancellationToken)
            .ConfigureAwait(false)).FirstOrDefault(cancellationToken);
    }

    /// <inheritdoc/>
    public override Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);

        return Task.FromResult(user.ClaimManager.GetAll());
    }

    /// <inheritdoc/>
    public override async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(claims);
        
        ArgumentNullException.ThrowIfNull(user);

        if (user.ClaimManager.TryAdd(claims))
        {
            await UpdateClaimsAsync(user, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(claim);
        
        ArgumentNullException.ThrowIfNull(newClaim);
        
        ArgumentNullException.ThrowIfNull(user);

        if (user.ClaimManager.TryReplace(claim, newClaim))
        {
            await UpdateClaimsAsync(user, cancellationToken); 
        }
    }

    /// <inheritdoc/>
    public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(claims);
        
        ArgumentNullException.ThrowIfNull(user);

        if (user.ClaimManager.TryRemove(claims))
        {
            await UpdateClaimsAsync(user, cancellationToken);
        }
    }
    
    protected virtual async Task<UpdateResult> UpdateClaimsAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", ConvertIdToString(user.Id));

        var setDocument = new BsonDocument("$set", new BsonDocument("Claims", new BsonArray(user.Claims.Select(x => x.ToBsonDocument()))));

        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(claim);
        
        var matchDocument = new BsonDocument("Claims",
                new BsonDocument("$elemMatch",
                    new BsonDocument()
                    {
                        { "Value",claim.Value},
                        { "Type", claim.Type}
                    }));

        return await (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            cancellationToken: cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// Sets the token value for a particular user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="loginProvider">The authentication provider for the token.</param>
    /// <param name="name">The name of the token.</param>
    /// <param name="value">The value of the token.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public override async Task SetTokenAsync(TUser user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        ArgumentNullException.ThrowIfNull(user);

        var token = await FindTokenAsync(user, loginProvider, name, cancellationToken).ConfigureAwait(false);
        
        if (token == null)
        {
            await AddUserTokenAsync(CreateUserToken(user, loginProvider, name, value)).ConfigureAwait(false);
        }
        else
        {
            if (user.TokenManager.TryReplace(loginProvider, name, value))
            {
                await UpdateTokensAsync(user, cancellationToken);
            }
        }
    }

    protected override Task<TUserToken?> FindTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);
        
        return user.TokenManager.Has(loginProvider, name, out var token) 
            ? Task.FromResult<TUserToken?>((TUserToken)token!.ToIdentityUserToken(user.Id)) 
            : Task.FromResult<TUserToken?>(null);
    }

    protected override async Task AddUserTokenAsync(TUserToken token)
    {
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(token);

        var user = await FindUserAsync(token.UserId, default);

        if (user is null) throw new InvalidOperationException($"User {token.UserId} doesn't exist");

        if (user.TokenManager.TryAdd(token.LoginProvider, token.Name, token.Value))
        {
            await UpdateTokensAsync(user, default);
        }
    }

    protected override async Task RemoveUserTokenAsync(TUserToken token)
    {
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(token);

        var user = await FindUserAsync(token.UserId, default);

        if (user is null) throw new InvalidOperationException($"User {token.UserId} doesn't exist");

        if (user.TokenManager.TryRemove(token.LoginProvider, token.Name, token.Value))
        {
            await UpdateTokensAsync(user, default);
        }
    }
    
    protected virtual async Task<UpdateResult> UpdateTokensAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", ConvertIdToString(user.Id));

        var setDocument = new BsonDocument("$set", new BsonDocument("Tokens", new BsonArray(user.Tokens.Select(x => x.ToBsonDocument()))));
        
        return await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public override IQueryable<TUser> Users { get; }

    public override async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested(); 
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);

        ArgumentNullException.ThrowIfNull(login);

        if (user.UserLoginManager.TryAdd(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName))
        {
            await UpdateLoginsAsync(user, cancellationToken).ConfigureAwait(false);
        }
    }
    
    public override async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);

        if (user.UserLoginManager.TryRemove(loginProvider, providerKey))
        {
            await UpdateLoginsAsync(user, cancellationToken).ConfigureAwait(false);
        }
    }

    protected virtual async Task<UpdateResult> UpdateLoginsAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", ConvertIdToString(user.Id));
        
        var setDocument = new BsonDocument("$set", new BsonDocument("Logins", new BsonArray(user.Logins.Select(x => x.ToBsonDocument()))));

        return (await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false));
    }

    public override Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);
        
        return Task.FromResult<IList<UserLoginInfo>>(user.Logins.ConvertAll(x => (UserLoginInfo)x));
    }

    /// <summary>
    /// Gets the user, if any, associated with the specified, normalized email address.
    /// </summary>
    /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>
    /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
    /// </returns>
    public override async Task<TUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
    {
        return (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(
                new BsonDocument("NormalizedEmail", normalizedEmail)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    public override async Task<bool> IsInRoleAsync(TUser user, string normalizedRoleName,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        
        if (string.IsNullOrWhiteSpace(normalizedRoleName))
        {
            throw new ArgumentException(string.Empty, nameof(normalizedRoleName));
        }
        
        var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);

        return roleEntity is not null && user.RoleManager.Has(roleEntity.Id);
    }

    protected override async Task<TRole?> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return (await RoleCollection.FindAsync(new BsonDocumentFilterDefinition<TRole>(
                new BsonDocument("NormalizedName", normalizedRoleName)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .SingleOrDefault(cancellationToken: cancellationToken);
    }

    protected override async Task<TUserRole?> FindUserRoleAsync(TKey userId, TKey roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();

        var matchUserIdDocument = new BsonDocument("_id", ConvertIdToString(userId));
        
        var unwindDocument = new BsonDocument("$unwind",
            new BsonDocument
            {
                { "path", "$Roles" },
                { "preserveNullAndEmptyArrays", false }
            });

        var matchDocument = new BsonDocument("$match", new BsonDocument("Roles._id", ConvertIdToString(roleId)));

        var projectDocument = new BsonDocument("$project", new BsonDocument()
        {
            { "UserId", "_id"},
            { "RoleId", "$Roles._id"}
        });

        return (TUserRole)(await UserCollection.AggregateAsync(
            new BsonDocumentStagePipelineDefinition<TUser, IdentityUserRole<TKey>>(new[]
                { matchUserIdDocument, unwindDocument, matchDocument, projectDocument }),
            cancellationToken: cancellationToken).ConfigureAwait(false)).SingleOrDefault(cancellationToken: cancellationToken);
    }

    public override async Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);

        if (roleEntity is null) return new List<TUser>();

        var matchDocument = new BsonDocument("Roles",
                new BsonDocument("$elemMatch",
                    new BsonDocument("_id",
                        ConvertIdToString(roleEntity.Id))));

        return await (await UserCollection.FindAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            cancellationToken: cancellationToken).ConfigureAwait(false)).ToListAsync(cancellationToken: cancellationToken);
    }

    public override async Task AddToRoleAsync(TUser user, string normalizedRoleName,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);

        var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);

        if (roleEntity is null) throw new InvalidOperationException($"Role {normalizedRoleName} doesn't exist");

        if (user.RoleManager.TryAdd(roleEntity.Id, roleEntity.NormalizedName, roleEntity.Name))
        {
            await UpdateRolesAsync(user, cancellationToken).ConfigureAwait(false);
        }
    }

    public override async Task RemoveFromRoleAsync(TUser user, string normalizedRoleName,
        CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);

        if (user.RoleManager.TryRemove(normalizedRoleName))
        {
            await UpdateRolesAsync(user, cancellationToken).ConfigureAwait(false);
        }
    }

    public override Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(user);
        
        return Task.FromResult(user.RoleManager.GetNames());
    }
    
    protected virtual async Task UpdateRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", ConvertIdToString(user.Id));

        var setDocument = new BsonDocument("$set", new BsonDocument("Roles", new BsonArray(user.Roles.Select(x => x.ToBsonDocument()))));

        await UserCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TUser>(matchDocument),
            new BsonDocumentUpdateDefinition<TUser>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}