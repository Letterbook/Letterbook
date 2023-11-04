using Bogus;
using Letterbook.ActivityPub;

namespace Letterbook.Api.Tests.Fakes;

public class FakeActor : Faker<AsAp.Actor>
{
    public static string[] ActorTypes = { "Application", "Group", "Organization", "Person", "Service" };
    public FakeActor(string? authority = null)
    {
        authority ??= new Faker().Internet.Url();
        RuleFor(actor => actor.Id, faker => new CompactIri(authority + faker.Internet.UrlRootedPath()));
        RuleFor(o => o.Type, f => f.PickRandom(ActorTypes));
        // RuleFor(o => o.PreferredUsername, f => f.Internet.UserName());
        // RuleFor(o => o.Id, (f, o) => new Uri($"https://letterbook.example/user/{o.PreferredUsername}"));
        RuleFor(o => o.Attachment, () => null);
        RuleFor(o => o.Followers, (f, o) => new AsAp.Collection{ Id = o.Id! + "/followers", Type = "Collection"});
        RuleFor(o => o.Following, (f, o) => new AsAp.Collection{ Id = o.Id! + "/following", Type = "Collection"});
        RuleFor(o => o.Inbox, (f, o) => new AsAp.Collection{ Id = o.Id! + "/inbox", Type = "Collection"});
        RuleFor(o => o.Outbox, (f, o) => new AsAp.Collection{ Id = o.Id! + "/outbox", Type = "Collection"});
        // RuleFor(o => o.Endpoints, () => null);
        RuleFor(o => o.Audience, () => null);
        RuleFor(o => o.To, () => null);
        RuleFor(o => o.Bto, () => null);
        RuleFor(o => o.Cc, () => null);
        RuleFor(o => o.Bcc, () => null);
        // RuleFor(o => o.Name, (f, o) => o.PreferredUsername);
        // RuleFor(o => o.Summary, (f, o) => f.Lorem.Sentences(3));
        RuleFor(o => o.Context, () => null);
        RuleFor(o => o.Generator, () => null);
        RuleFor(o => o.Preview, () => null);
        RuleFor(o => o.InReplyTo, () => null);
    }
}