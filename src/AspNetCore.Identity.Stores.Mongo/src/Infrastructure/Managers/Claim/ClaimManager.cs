using AspNetCore.Identity.Stores.Mongo.Domain.Contracts;
using AspNetCore.Identity.Stores.Mongo.Domain.Models;
using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;
using AspNetCore.Identity.Stores.Mongo.Infrastructure.Extensions;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Claim;

public sealed class ClaimManager<TKey> : IClaimManager<TKey> where TKey : IEquatable<TKey>
{
    private readonly IClaimable _claimsHolder;

    public ClaimManager(IClaimable claimsHolder) => _claimsHolder = claimsHolder;

    public IEnumerable<MongoClaim> FindBy(string type, string value)
        => _claimsHolder.Claims.Where(x => x.Type.Equals(type) && x.Value.Equals(value)).ToList();

    public bool Has(string type, string value)
        => _claimsHolder.Claims.Any(x => x.Type.Equals(type) && x.Value.Equals(value));

    public bool Has(string type, string value, out MongoClaim? claim)
    {
        claim = _claimsHolder.Claims.FirstOrDefault(x => x.Type.Equals(type) && x.Value.Equals(value));

        return claim is not null;
    }

    public bool TryAdd(string value, string type, string valueType, string issuer)
    {
        if (Has(type, value))
            return false;
        
        var mongoClaim = new MongoClaim(type, value, valueType, issuer);
        
        _claimsHolder.Claims.Add(mongoClaim);
        
        return true;
    }

    public bool TryAdd(IEnumerable<System.Security.Claims.Claim> claims)
    {
        var result = false;
        foreach (var claim in claims)
        {
            if (TryAdd(claim.Value, claim.Type, claim.ValueType, claim.Issuer))
                result = true;
        }

        return result;
    }
    
    public bool TryRemove(IEnumerable<System.Security.Claims.Claim> claims)
    {
        var deletedCount = _claimsHolder.Claims.RemoveAll(x => claims.Any(y => y.Value.Equals(x.Value) && y.Type.Equals(x.Type)));

        return deletedCount > 0;
    }
    
    public bool TryRemove(System.Security.Claims.Claim claim)
    {
        var deletedCount = _claimsHolder.Claims.RemoveAll(x => claim.Value.Equals(x.Value) && claim.Type.Equals(x.Type));

        return deletedCount > 0;
    }

    public bool TryReplace(System.Security.Claims.Claim claim, System.Security.Claims.Claim newClaim)
    {
        var mClaims = FindBy(claim.Type, claim.Value);

        if (!mClaims.Any()) return false;
        
        foreach (var mClaim in mClaims)
        {
            mClaim.Type = newClaim.Type;
            mClaim.Value = newClaim.Value;
            mClaim.Issuer = newClaim.Issuer;
            mClaim.ValueType = newClaim.ValueType;
        }

        return true;
    }

    public IList<System.Security.Claims.Claim> GetAll() => _claimsHolder.Claims.ToClaims();
}