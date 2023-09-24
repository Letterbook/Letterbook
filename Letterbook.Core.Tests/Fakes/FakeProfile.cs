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
        CustomInstantiator(f => Profile.CreateIndividual(new Uri(uri, ""), f.Internet.UserName()));

        RuleFor(p => p.FollowersCollection, (f, p) => ObjectCollection<FollowerRelation>.Followers(p.Id));
        RuleFor(p => p.DisplayName, (f) => f.Internet.UserName());
        RuleFor(p => p.Handle, (f, p) => p.Handle ?? $"@{f.Internet.UserName()}@{uri.Authority}");
        RuleFor(p => p.Description, (f) => f.Lorem.Paragraph());
        RuleFor(p => p.CustomFields,
            (f) => new CustomField[] { new() { Label = "UUID", Value = $"{f.Random.Guid()}" } });
    }

    public FakeProfile(Uri uri, Account owner) : this(uri)
    {
        CustomInstantiator(f =>
        {
            var profile = Profile.CreateIndividual(uri, $"{f.Hacker.Noun()}_{f.Random.Hexadecimal(4)}");
            profile.OwnedBy = owner;
            profile.RelatedAccounts.Add(new LinkedProfile(owner, profile, ProfilePermission.All));
            return profile;
        });
    }
    
    public FakeProfile(string authority, Account owner) : this(new Uri($"https://{authority}"), owner)
    {}
}