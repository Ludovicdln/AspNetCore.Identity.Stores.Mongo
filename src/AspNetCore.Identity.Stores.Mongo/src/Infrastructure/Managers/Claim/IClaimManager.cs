using AspNetCore.Identity.Stores.Mongo.Domain.Models;

namespace AspNetCore.Identity.Stores.Mongo.Infrastructure.Managers.Claim;

public interface IClaimManager<TKey> where TKey : IEquatable<TKey>
{
    IEnumerable<MongoClaim> FindBy(string type, string value);
    
    bool Has(string type, string value);
    
    bool Has(string type, string value, out MongoClaim? claim);
    
    bool TryAdd(string value, string type, string valueType, string issuer);

    bool TryAdd(IEnumerable<System.Security.Claims.Claim> claims);
         
    bool TryRemove(System.Security.Claims.Claim claim);
    
    bool TryRemove(IEnumerable<System.Security.Claims.Claim> claims);

    bool TryReplace(System.Security.Claims.Claim claim, System.Security.Claims.Claim newClaim);

    IList<System.Security.Claims.Claim> GetAll();
}