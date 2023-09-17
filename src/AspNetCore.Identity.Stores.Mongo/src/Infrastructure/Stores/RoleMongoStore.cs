using System.Security.Claims;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Stores;

/// <summary>
/// Creates a new instance of a persistence store for roles.
/// </summary>
/// <typeparam name="TRole">The type of the class representing a role.</typeparam>
public class RoleMongoStoreAsObjectId<TRole> : RoleMongoStore<TRole, ObjectId>
    where TRole : MongoIdentityRole<ObjectId>
{
    public RoleMongoStoreAsObjectId(IdentityErrorDescriber describer, IMongoCollection<TRole> roleCollection) 
        : base(describer, roleCollection)
    {
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var result = await RoleCollection.ReplaceOneAsync(new BsonDocumentFilterDefinition<TRole>(new BsonDocument("_id",role.Id)), role, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "UpdateError", Description = "Failed to update role" });
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ArgumentNullException.ThrowIfNull(role);
        
        var result = await RoleCollection.DeleteOneAsync(
            new BsonDocumentFilterDefinition<TRole>(new BsonDocument("_id", role.Id)), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "DeleteError", Description = $"Failed to delete role {role.Id}" });
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateRolePropertyAsync(TRole role, string property, string? value)
    {
        var matchDocument = new BsonDocument("_id", role.Id);
        
        var setDocument = new BsonDocument("$set", new BsonDocument(property, value));

        return await RoleCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TRole>(matchDocument),
            new BsonDocumentUpdateDefinition<TRole>(setDocument));
    }

    /// <inheritdoc/>
    public override async Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await RoleCollection.FindAsync(new BsonDocumentFilterDefinition<TRole>(
                new BsonDocument("_id", ObjectId.Parse(roleId))), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateClaimsAsync(TRole role, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", role.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Claims", new BsonArray(role.Claims.Select(x => x.ToBsonDocument()))));

        return await RoleCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TRole>(matchDocument),
            new BsonDocumentUpdateDefinition<TRole>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Creates a new instance of a persistence store for roles.
/// </summary>
/// <typeparam name="TRole">The type of the class representing a role.</typeparam>
public class RoleMongoStoreAsGuid<TRole> : RoleMongoStore<TRole, Guid>
    where TRole : MongoIdentityRole<Guid>
{
    public RoleMongoStoreAsGuid(IdentityErrorDescriber describer, IMongoCollection<TRole> roleCollection) 
        : base(describer, roleCollection)
    {
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var result = await RoleCollection.ReplaceOneAsync(new BsonDocumentFilterDefinition<TRole>(new BsonDocument("_id",role.Id)), role, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "UpdateError", Description = "Failed to update role" });
    }

    /// <inheritdoc/>
    public override async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ArgumentNullException.ThrowIfNull(role);
        
        var result = await RoleCollection.DeleteOneAsync(
            new BsonDocumentFilterDefinition<TRole>(new BsonDocument("_id", role.Id)), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "DeleteError", Description = $"Failed to delete role {role.Id}" });
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateRolePropertyAsync(TRole role, string property, string? value)
    {
        var matchDocument = new BsonDocument("_id", role.Id);
        
        var setDocument = new BsonDocument("$set", new BsonDocument(property, value));

        return await RoleCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TRole>(matchDocument),
            new BsonDocumentUpdateDefinition<TRole>(setDocument));
    }

    /// <inheritdoc/>
    public override async Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await RoleCollection.FindAsync(new BsonDocumentFilterDefinition<TRole>(
                new BsonDocument("_id", Guid.Parse(roleId))), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    protected override async Task<UpdateResult> UpdateClaimsAsync(TRole role, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", role.Id);

        var setDocument = new BsonDocument("$set", new BsonDocument("Claims", new BsonArray(role.Claims.Select(x => x.ToBsonDocument()))));

        return await RoleCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TRole>(matchDocument),
            new BsonDocumentUpdateDefinition<TRole>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}

/// <summary>
/// Creates a new instance of a persistence store for roles.
/// </summary>
/// <typeparam name="TRole">The type of the class representing a role.</typeparam>
public class RoleMongoStore<TRole> : RoleMongoStore<TRole, string>
    where TRole : MongoIdentityRole<string>
{
    public RoleMongoStore(IdentityErrorDescriber describer, IMongoCollection<TRole> roleCollection) 
        : base(describer, roleCollection)
    {
    }
}

/// <summary>
/// Creates a new instance of a persistence store for roles.
/// </summary>
/// <typeparam name="TRole">The type of the class representing a role.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
public class RoleMongoStore<TRole, TKey> :
    IRoleClaimStore<TRole>
    where TRole : MongoIdentityRole<TKey>
    where TKey : IEquatable<TKey>
{
    
    /// <summary>
    /// Get the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
    /// </summary>
    protected IdentityErrorDescriber ErrorDescriber { get; }
    
    protected IMongoCollection<TRole> RoleCollection { get; }

    private bool _disposed;
    
    public RoleMongoStore(IdentityErrorDescriber describer, IMongoCollection<TRole> roleCollection)
    {
        ErrorDescriber = describer;
        RoleCollection = roleCollection;
    }
    
    /// <summary>
    /// Throws if this class has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    /// <summary>
    /// Dispose the store
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
    }

    /// <summary>
    /// Creates a new role in a store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to create in the store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
    public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);

        await RoleCollection.InsertOneAsync(role, null, cancellationToken).ConfigureAwait(false);
        
        return IdentityResult.Success;
    }

    /// <summary>
    /// Updates a role in a store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to update in the store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
    public virtual async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        var result = await RoleCollection.ReplaceOneAsync(new BsonDocumentFilterDefinition<TRole>(new BsonDocument("_id", 
            ConvertIdToString(role.Id))), role, cancellationToken: cancellationToken).ConfigureAwait(false);

        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "UpdateError", Description = "Failed to update role" });
    }

    /// <summary>
    /// Deletes a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to delete from the store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
    public virtual async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);
        
        var result = await RoleCollection.DeleteOneAsync(
            new BsonDocumentFilterDefinition<TRole>(new BsonDocument("_id", ConvertIdToString(role.Id))), cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return result.IsAcknowledged
            ? IdentityResult.Success
            : IdentityResult.Failed(new IdentityError()
                { Code = "DeleteError", Description = $"Failed to delete role {role.Id}" });
    }

    /// <summary>
    /// Gets the ID for a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose ID should be returned.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
    public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);
        
        return Task.FromResult(ConvertIdToString(role.Id)!);
    }

    /// <summary>
    /// Gets the name of a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose name should be returned.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
    public Task<string?> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);
        
        return Task.FromResult(role.Name);
    }

    /// <summary>
    /// Sets the name of a role in the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose name should be set.</param>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task SetRoleNameAsync(TRole role, string? roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);

        if (string.IsNullOrEmpty(role.Name) || !role.Name.Equals(roleName))
        {
            role.Name = roleName;
            
            await UpdateRolePropertyAsync(role, "Name", roleName);
        }
    }

    protected virtual async Task<UpdateResult> UpdateRolePropertyAsync(TRole role, string property, string? value)
    {
        var matchDocument = new BsonDocument("_id", ConvertIdToString(role.Id));
        
        var setDocument = new BsonDocument("$set", new BsonDocument(property, value));

        return await RoleCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TRole>(matchDocument),
            new BsonDocumentUpdateDefinition<TRole>(setDocument));
    }

    /// <summary>
    /// Get a role's normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose normalized name should be retrieved.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
    public Task<string?> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);
        
        return Task.FromResult(role.NormalizedName);
    }

    /// <summary>
    /// Set a role's normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose normalized name should be set.</param>
    /// <param name="normalizedName">The normalized name to set</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task SetNormalizedRoleNameAsync(TRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);

        if (string.IsNullOrEmpty(role.NormalizedName) || !role.NormalizedName.Equals(normalizedName))
        {
            role.NormalizedName = normalizedName;
            
            await UpdateRolePropertyAsync(role, "NormalizedName", normalizedName);
        }
    }

    /// <summary>
    /// Finds the role who has the specified ID as an asynchronous operation.
    /// </summary>
    /// <param name="roleId">The role id to look for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
    public virtual async Task<TRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await RoleCollection.FindAsync(new BsonDocumentFilterDefinition<TRole>(
                new BsonDocument("_id", roleId)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        return (await RoleCollection.FindAsync(new BsonDocumentFilterDefinition<TRole>(
                new BsonDocument("NormalizedName", normalizedRoleName)), cancellationToken: cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(role);

        return Task.FromResult(role.ClaimManager.GetAll());
    }

    /// <inheritdoc/>
    public async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(claim);
        
        ArgumentNullException.ThrowIfNull(role);

        if (role.ClaimManager.TryAdd(claim.Value, claim.Type, claim.ValueType, claim.Issuer))
        {
            await UpdateClaimsAsync(role, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken = new CancellationToken())
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        ThrowIfDisposed();
        
        ArgumentNullException.ThrowIfNull(claim);
        
        ArgumentNullException.ThrowIfNull(role);

        if (role.ClaimManager.TryRemove(claim))
        {
            await UpdateClaimsAsync(role, cancellationToken);
        }
    }
    
    protected virtual async Task<UpdateResult> UpdateClaimsAsync(TRole role, CancellationToken cancellationToken)
    {
        var matchDocument = new BsonDocument("_id", ConvertIdToString(role.Id));

        var setDocument = new BsonDocument("$set", new BsonDocument("Claims", new BsonArray(role.Claims.Select(x => x.ToBsonDocument()))));

        return await RoleCollection.UpdateOneAsync(new BsonDocumentFilterDefinition<TRole>(matchDocument),
            new BsonDocumentUpdateDefinition<TRole>(setDocument), cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Converts the provided <paramref name="id"/> to its string representation.
    /// </summary>
    /// <param name="id">The id to convert.</param>
    /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
    protected virtual string? ConvertIdToString(TKey id)
    {
        if (object.Equals(id, default(TKey)))
        {
            return null;
        }
        return id.ToString();
    }
}