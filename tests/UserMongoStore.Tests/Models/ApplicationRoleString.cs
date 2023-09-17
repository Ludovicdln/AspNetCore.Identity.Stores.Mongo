using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

namespace UserMongoStore.Tests.Models;

public sealed class ApplicationRoleString : MongoIdentityRole<string>
{
    public override string Id { get; set; } = Guid.NewGuid().ToString();
}