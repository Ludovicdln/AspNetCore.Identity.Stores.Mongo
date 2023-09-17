using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Extensions;

public static class TokenExtensions
{
    public static IdentityUserToken<TKey> ToIdentityUserToken<TKey>(this MongoToken mongoToken, TKey userId)
        where TKey : IEquatable<TKey>
        => new() { LoginProvider = mongoToken.LoginProvider, Name = mongoToken.Name, Value = mongoToken.Value, UserId = userId };
}