using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public sealed class FakeProfile : Faker<Profile>
{
    public FakeProfile() : this(new Uri(new Faker().Internet.UrlWithPath()))
    {
        RuleFor(p => p.LocalId, f => f.Random.Guid());
    }

    public FakeProfile(string authority) : this(new Uri($"http://{authority}/{new Faker().Internet.UserName()}"))
    {
        RuleFor(p => p.LocalId, f => f.Random.Guid());
    }

    public FakeProfile(Uri uri)
    {
        CustomInstantiator(f => new Profile(uri));

        RuleFor(p => p.FollowersCollection,
            (f, p) => new ObjectCollection<Profile>() { Id = new Uri(p.Id, "/followers") });
        RuleFor(p => p.DisplayName, (f) => f.Internet.UserName());
        RuleFor(p => p.Description, (f) => f.Lorem.Paragraph());
        RuleFor(p => p.CustomFields,
            (f) => new CustomField[] { new() { Label = "UUID", Value = $"{f.Random.Guid()}" } });
    }
}