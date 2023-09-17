using System.Runtime.CompilerServices;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Connection;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Connection.Options;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Extensions;

public static class IdentityMongoDbBuilderExtensions
{
    /// <summary>
    /// Add an MongoDb implementation of identity stores.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <param name="identityBuilder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    /// <param name="configureOptions"></param>
    public static IdentityBuilder AddMongoDbStores<TUser>(this IdentityBuilder identityBuilder,
        string connectionString, string databaseName, Action<MongoOptions> configureOptions)
        where TUser : class, new()
    {
        if (!TryGetGenericType(typeof(TUser), typeof(MongoIdentityUser<>), out var userBaseType))
        {
            throw new InvalidOperationException(
                $"{typeof(TUser).Name} is not derived of {typeof(MongoIdentityUser<>).Name}");
        }
        
        TryGetGenericTypeArgument(userBaseType!, out var userArgType);

        identityBuilder.TryAddMongoServices<TUser>(connectionString, databaseName, configureOptions);

        var onlyUserStoreType = GetOnlyUserStoreType(typeof(TUser), userArgType);
        
        identityBuilder.InvokeUserGenericStore(onlyUserStoreType);
        
        return identityBuilder;
    }
    
    /// <summary>
    /// Add an MongoDb implementation of identity stores.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <param name="identityBuilder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    public static IdentityBuilder AddMongoDbStores<TUser>(this IdentityBuilder identityBuilder,
        string connectionString, string databaseName)
        where TUser : class, new()
    {
        if (!TryGetGenericType(typeof(TUser), typeof(MongoIdentityUser<>), out var userBaseType))
        {
            throw new InvalidOperationException(
                $"{typeof(TUser).Name} is not derived of {typeof(MongoIdentityUser<>).Name}");
        }
        
        TryGetGenericTypeArgument(userBaseType!, out var userArgType);

        identityBuilder.TryAddMongoServices<TUser>(connectionString, databaseName);

        var onlyUserStoreType = GetOnlyUserStoreType(typeof(TUser), userArgType);
        
        identityBuilder.InvokeUserGenericStore(onlyUserStoreType);
        
        return identityBuilder;
    }
    
    /// <summary>
    /// Add an MongoDb implementation of identity stores.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <param name="identityBuilder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    /// <param name="configureOptions"></param>
    public static IdentityBuilder AddMongoDbStores<TUser, TRole>(this IdentityBuilder identityBuilder,
        string connectionString, string databaseName, Action<MongoOptions> configureOptions)
        where TUser : class, new()
        where TRole : class, new()
    {
        if (!TryGetGenericType(typeof(TUser), typeof(MongoIdentityUser<>), out var userBaseType))
        {
            throw new InvalidOperationException(
                $"{typeof(TUser).Name} is not derived of {typeof(MongoIdentityUser<>).Name}");
        }

        if (!TryGetGenericType(typeof(TRole), typeof(MongoIdentityRole<>), out var roleBaseType))
        {
            throw new InvalidOperationException(
                $"{typeof(TRole).Name} is not derived of {typeof(MongoIdentityRole<>).Name}");
        }

        TryGetGenericTypeArgument(userBaseType!, out var userArgType);
        
        TryGetGenericTypeArgument(roleBaseType!, out var roleArgType);

        if (userArgType != roleArgType)
            throw new InvalidOperationException($"{userArgType.Name} TKey is different from {roleArgType.Name} TKey");

        identityBuilder.TryAddMongoServices<TUser, TRole>(connectionString, databaseName, configureOptions);

        var (userStoreType, roleStoreType) = GetUserRoleStoresType(typeof(TUser), typeof(TRole), userArgType);
        
        identityBuilder.AddRoles<TRole>();

        identityBuilder.InvokeUserGenericStore(userStoreType);
        
        identityBuilder.InvokeRoleGenericStore(roleStoreType);
        
        return identityBuilder;
    }
    
     /// <summary>
    /// Add an MongoDb implementation of identity stores.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <param name="identityBuilder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    public static IdentityBuilder AddMongoDbStores<TUser, TRole>(this IdentityBuilder identityBuilder,
        string connectionString, string databaseName)
        where TUser : class, new()
        where TRole : class, new()
    {
        if (!TryGetGenericType(typeof(TUser), typeof(MongoIdentityUser<>), out var userBaseType))
        {
            throw new InvalidOperationException(
                $"{typeof(TUser).Name} is not derived of {typeof(MongoIdentityUser<>).Name}");
        }

        if (!TryGetGenericType(typeof(TRole), typeof(MongoIdentityRole<>), out var roleBaseType))
        {
            throw new InvalidOperationException(
                $"{typeof(TRole).Name} is not derived of {typeof(MongoIdentityRole<>).Name}");
        }

        TryGetGenericTypeArgument(userBaseType!, out var userArgType);
        
        TryGetGenericTypeArgument(roleBaseType!, out var roleArgType);

        if (userArgType != roleArgType)
            throw new InvalidOperationException($"{userArgType.Name} TKey is different from {roleArgType.Name} TKey");

        identityBuilder.TryAddMongoServices<TUser, TRole>(connectionString, databaseName);

        var (userStoreType, roleStoreType) = GetUserRoleStoresType(typeof(TUser), typeof(TRole), userArgType);
        
        identityBuilder.AddRoles<TRole>();

        identityBuilder.InvokeUserGenericStore(userStoreType);
        
        identityBuilder.InvokeRoleGenericStore(roleStoreType);
        
        return identityBuilder;
    }
    
    /// <summary>
    /// Add an MongoDb implementation of identity stores.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key document.</typeparam>
    /// <param name="identityBuilder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    /// <param name="configureOptions"></param>
    public static IdentityBuilder AddMongoDbStores<TUser, TRole, TKey>(this IdentityBuilder identityBuilder,
        string connectionString, string databaseName, Action<MongoOptions> configureOptions) 
        where TUser : MongoIdentityUser<TKey>, new()
        where TRole : MongoIdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        identityBuilder.TryAddMongoServices<TUser, TRole>(connectionString, databaseName, configureOptions);
        
        var (userStoreType, roleStoreType) = GetUserRoleStoresType(typeof(TUser), typeof(TRole), typeof(TKey));
        
        identityBuilder.AddRoles<TRole>();
        
        identityBuilder.InvokeUserGenericStore(userStoreType);
        
        identityBuilder.InvokeRoleGenericStore(roleStoreType);
        
        return identityBuilder;
    }
    
    /// <summary>
    /// Add an MongoDb implementation of identity stores.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key document.</typeparam>
    /// <param name="identityBuilder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
    /// <param name="connectionString"></param>
    /// <param name="databaseName"></param>
    /// <param name="configureOptions"></param>
    public static IdentityBuilder AddMongoDbStores<TUser, TRole, TKey>(this IdentityBuilder identityBuilder,
        string connectionString, string databaseName) 
        where TUser : MongoIdentityUser<TKey>, new()
        where TRole : MongoIdentityRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        identityBuilder.TryAddMongoServices<TUser, TRole>(connectionString, databaseName);
        
        var (userStoreType, roleStoreType) = GetUserRoleStoresType(typeof(TUser), typeof(TRole), typeof(TKey));
        
        identityBuilder.AddRoles<TRole>();
        
        identityBuilder.InvokeUserGenericStore(userStoreType);
        
        identityBuilder.InvokeRoleGenericStore(roleStoreType);
        
        return identityBuilder;
    }
    
    private static bool TryAddMongoServices<TUser>(this IdentityBuilder identityBuilder, 
        string connectionString, string databaseName, Action<MongoOptions>? configureOptions = null) where TUser : class
    {
        MongoOptions mongoOptions = null;
        
        if (configureOptions is null 
                ? !TryAddConnection(identityBuilder, connectionString, databaseName) 
                : !TryAddConnection(identityBuilder, connectionString, databaseName, 
                configureOptions, out mongoOptions))
            return false;
        
        identityBuilder.AddUserCollection<TUser>(databaseName, mongoOptions);
        
        return true;
    }
    
    private static bool TryAddMongoServices<TUser, TRole>(this IdentityBuilder identityBuilder,
        string connectionString, string databaseName, Action<MongoOptions>? configureOptions = null)
        where TUser : class, new()
        where TRole : class, new()
    {
        MongoOptions mongoOptions = null;

        if (configureOptions is null 
                ? !TryAddConnection(identityBuilder, connectionString, databaseName) 
                : !TryAddConnection(identityBuilder, connectionString, databaseName, 
                    configureOptions, out mongoOptions))
            return false;
        
        identityBuilder.AddUserCollection<TUser>(databaseName, mongoOptions);
        
        identityBuilder.AddRoleCollection<TRole>(databaseName, mongoOptions);

        return true;
    }

    private static bool TryAddConnection(IdentityBuilder identityBuilder, string connectionString, 
        string databaseName, Action<MongoOptions> configureOptions, out MongoOptions mongoOptions)
    {
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
        
        if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException(nameof(databaseName));

        identityBuilder.Services.Configure(configureOptions);
        
        identityBuilder.Services.AddSingleton<IMongoConnection>(x 
            => MongoConnection.Connect(connectionString));
        
        mongoOptions = identityBuilder.GetMongoOptions()!.Value;
        
        return true;
    }
    
    private static bool TryAddConnection(IdentityBuilder identityBuilder, string connectionString, 
        string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
        
        if (string.IsNullOrEmpty(databaseName)) throw new ArgumentNullException(nameof(databaseName));
        
        identityBuilder.Services.AddSingleton<IMongoConnection>(x 
            => MongoConnection.Connect(connectionString));
        
        return true;
    }

    private static IOptions<MongoOptions>? GetMongoOptions(this IdentityBuilder identityBuilder)
        => identityBuilder.Services
            .BuildServiceProvider()
            .GetService<IOptions<MongoOptions>>();

    private static void AddRoleCollection<TRole>(this IdentityBuilder identityBuilder, 
        string databaseName, MongoOptions? mongoOptions) where TRole : class
    {
        var roleCollectionName = GetRoleCollectionName<TRole>(mongoOptions);

        identityBuilder.Services.AddScoped<IMongoCollection<TRole>>(x =>
        {
            var connection = x.GetService<IMongoConnection>();
            return connection!.GetDatabase(databaseName).GetCollection<TRole>(roleCollectionName);
        });
    }

    private static void AddUserCollection<TUser>(this IdentityBuilder identityBuilder, 
        string databaseName, MongoOptions? mongoOptions) where TUser : class
    {
        var userCollectionName = GetUserCollectionName<TUser>(mongoOptions);

        identityBuilder.Services.AddScoped<IMongoCollection<TUser>>(x =>
        {
            var connection = x.GetService<IMongoConnection>();
            return connection!.GetDatabase(databaseName).GetCollection<TUser>(userCollectionName);
        });
    }
    
    private static void InvokeUserGenericStore(this IdentityBuilder identityBuilder, Type userStoreType)
    {
        var addUserStoreMethod = typeof(IdentityBuilder)
            .GetMethod(nameof(identityBuilder.AddUserStore))!
            .MakeGenericMethod(userStoreType);
        
        addUserStoreMethod.Invoke(identityBuilder, Array.Empty<object>());
    }
    
    private static void InvokeRoleGenericStore(this IdentityBuilder identityBuilder, Type roleStoreType)
    {
        var addRoleStoreMethod = typeof(IdentityBuilder)
            .GetMethod(nameof(identityBuilder.AddRoleStore))!
            .MakeGenericMethod(roleStoreType);

        addRoleStoreMethod.Invoke(identityBuilder, Array.Empty<object>());
    }
    
    private static Type GetOnlyUserStoreType(Type userType, Type keyType) => keyType switch
    {
        _ when keyType == typeof(Guid) => typeof(UserOnlyMongoStoreAsGuid<>).MakeGenericType(userType),
        _ when keyType == typeof(ObjectId) => typeof(UserOnlyMongoStoreAsObjectId<>).MakeGenericType(userType),
        _ => typeof(UserOnlyMongoStore<,>).MakeGenericType(userType, keyType)
    };
    
    private static (Type userStoreType, Type roleStoreType) GetUserRoleStoresType(Type userType, Type roleType, Type keyType)
    {
        return keyType switch
        {
            _ when keyType == typeof(Guid) => (typeof(UserMongoStoreAsGuid<,>).MakeGenericType(userType, roleType),
                typeof(RoleMongoStoreAsGuid<>).MakeGenericType(roleType)),
            _ when keyType == typeof(ObjectId) => (
                typeof(UserMongoStoreAsObjectId<,>).MakeGenericType(userType, roleType),
                typeof(RoleMongoStoreAsObjectId<>).MakeGenericType(roleType)),
            _ => (typeof(UserMongoStore<,,>).MakeGenericType(userType, roleType, keyType),
                typeof(RoleMongoStore<>).MakeGenericType(roleType))
        };
    }
    
    // Lock for 10 derived type
    private static bool TryGetGenericType(Type currentType, Type matchedType, out Type? baseType, int counter = 0)
    {
        if (counter >= 10 || currentType.BaseType is null)
        {
            baseType = null;
            return false;
        }

        if (!currentType.BaseType.Name.Equals(matchedType.Name))
            return TryGetGenericType(currentType.BaseType, matchedType, out baseType, ++counter);
        
        baseType = currentType.BaseType;
        
        return true;
    }

    private static bool TryGetGenericTypeArgument(Type currentType, out Type? argumentType)
    {
        if (currentType.GenericTypeArguments.Length <= 0)
        {
            argumentType = null;
            return false;
        }

        argumentType = currentType.GenericTypeArguments[0];
        return true;
    }
    
    private static string GetUserCollectionName<TUser>(MongoOptions? mongoOptions) => 
        string.IsNullOrEmpty(mongoOptions?.UserCollectionName)
        ? typeof(TUser).Name
        : mongoOptions!.UserCollectionName;
    
    private static string GetRoleCollectionName<TRole>(MongoOptions? mongoOptions) => 
        string.IsNullOrEmpty(mongoOptions?.RoleCollectionName)
            ? typeof(TRole).Name
            : mongoOptions!.RoleCollectionName;
}