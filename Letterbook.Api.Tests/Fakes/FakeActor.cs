using Bogus;
using Fedodo.NuGet.ActivityPub.Model.ActorTypes;

namespace Letterbook.Api.Tests.Fakes;

public class FakeActor : Faker<Actor>
{
    public static string[] ActorTypes = { "Application", "Group", "Organization", "Person", "Service" };
    public FakeActor()
    {
        RuleFor(o => o.Type, f => f.PickRandom(ActorTypes));
        RuleFor(o => o.PreferredUsername, f => f.Internet.UserName());
        RuleFor(o => o.Id, (f, o) => new Uri($"https://letterbook.example/user/{o.PreferredUsername}"));
        RuleFor(o => o.Attachment, () => null);
        RuleFor(o => o.Followers, (f, o) => new Uri(o.Id!, "/followers"));
        RuleFor(o => o.Following, (f, o) => new Uri(o.Id!, "/following"));
        RuleFor(o => o.Inbox, (f, o) => new Uri(o.Id!, "/inbox"));
        RuleFor(o => o.Outbox, (f, o) => new Uri(o.Id!, "/outbox"));
        RuleFor(o => o.Endpoints, () => null);
        RuleFor(o => o.Audience, () => null);
        RuleFor(o => o.To, () => null);
        RuleFor(o => o.Bto, () => null);
        RuleFor(o => o.Cc, () => null);
        RuleFor(o => o.Bcc, () => null);
        RuleFor(o => o.Name, (f, o) => o.PreferredUsername);
        RuleFor(o => o.Summary, (f, o) => f.Lorem.Sentences(3));
        RuleFor(o => o.Context, () => null);
        RuleFor(o => o.Generator, () => null);
        RuleFor(o => o.Preview, () => null);
        RuleFor(o => o.InReplyTo, () => null);
    }
}