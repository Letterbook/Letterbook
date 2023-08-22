using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public sealed class FakeProfile : Faker<Profile>
{
    public FakeProfile() : this(new Uri(new Faker().Internet.UrlWithPath()))
    {}

    public FakeProfile(string authority) : this(new Uri($"http://{authority}/{new Faker().Internet.UserName()}"))
    {}

    public FakeProfile(Uri uri)
    {
        CustomInstantiator(f => new Profile(uri));

        RuleFor(p => p.FollowersCollection,
            (f, p) => new ObjectCollection<Profile>() { Id = new Uri(p.Id, "/followers") });
    }
}