using AspNetCore.Identity.Stores.Mongo.Domain.Models;

namespace AspNetCore.Identity.Stores.Mongo.Domain.Contracts;

public interface IClaimable
{
    public List<MongoClaim> Claims { get; }
}