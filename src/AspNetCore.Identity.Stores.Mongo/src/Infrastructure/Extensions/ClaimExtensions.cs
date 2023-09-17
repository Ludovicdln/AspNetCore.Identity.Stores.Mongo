using System.Security.Claims;
using AspNetCore.Identity.Stores.Mongo.Domain.Models;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Extensions;

public static class ClaimExtensions
{
    private static Claim ToClaim(this MongoClaim mongoClaim) =>
        new (mongoClaim.Type, mongoClaim.Value, mongoClaim.ValueType, mongoClaim.Issuer);
    
    public static IList<Claim> ToClaims(this IEnumerable<MongoClaim> mongoClaims)
    {
        return mongoClaims.Select(x => x.ToClaim()).ToList();
    }
}