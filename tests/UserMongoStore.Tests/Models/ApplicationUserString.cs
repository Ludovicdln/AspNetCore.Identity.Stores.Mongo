using AspNetCore.Identity.Stores.Mongo.Domain.Models.Identity;

namespace UserMongoStore.Tests.Models;

public class ApplicationUserString : MongoIdentityUser<string>
{
    public override string Id { get; set; } = Guid.NewGuid().ToString();
}