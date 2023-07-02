using Bogus;
using Fedodo.NuGet.ActivityPub.Model.ActorTypes;
using Fedodo.NuGet.ActivityPub.Model.JsonConverters.Model;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Api.Tests.Fakes;

public class FakeNote : Faker<Object>
{
    public FakeNote(Actor actor)
    {
        var audiences = new []
        {
            "https://www.w3.org/ns/activitystreams#Public",
            FollowersUri(actor).ToString(),
        };
        RuleFor(o => o.Type, f => "Note");
        RuleFor(o => o.Name, (f, o) => null);
        RuleFor(o => o.Type, f => "Note");
        RuleFor(o => o.Id, (f, o) => new Uri(actor.Id!, $"/{f.Random.Int()}"));
        RuleFor(o => o.Attachment, () => null);
        RuleFor(o => o.Audience, f => new TripleSet<Object>
        {
            StringLinks = f.Random.ArrayElements(audiences, f.Random.Number(2))
        });
        RuleFor(o => o.To, (f, o) => f.Random.Bool() ? new TripleSet<Object>
        {
            StringLinks = new []{RandomActorId()}
        } : null);
        RuleFor(o => o.Bto, (f) => f.Random.Bool() ? new TripleSet<Object>
        {
            StringLinks = new []{RandomActorId()}
        } : null);
        RuleFor(o => o.Cc, (f) => f.Random.Bool() ? new TripleSet<Object>
        {
            StringLinks = new []{RandomActorId()}
        } : null);
        RuleFor(o => o.Bcc, (f) => f.Random.Bool() ? new TripleSet<Object>
        {
            StringLinks = new []{RandomActorId()}
        } : null);
        RuleFor(o => o.Summary, (f, o) =>
        {
            var c = f.Random.Number(3);
            return c == 0 ? null : f.Random.Words(c);
        });
        RuleFor(o => o.Content, f => f.Lorem.Sentences(f.Random.Number(10)));
        RuleFor(o => o.Context, () => null);
        RuleFor(o => o.Generator, () => null);
        RuleFor(o => o.Preview, () => null);
        RuleFor(o => o.InReplyTo, (f) => f.Random.Bool() ? new TripleSet<Object>
        {
            StringLinks = new []{RandomActorId()}
        } : null);
    }

    public FakeNote() : this(new FakeActor().Generate())
    {
    }

    private static Uri FollowersUri(Actor actor)
    {
        return actor.Followers ?? new Uri(actor.Id ?? new Uri("https://letterbook.example/users/0"), "/followers");
    }

    private static string RandomActorId()
    {
        var r = new Randomizer();
        return $"https://{(r.Bool() ? "letterbook.example" : "mastodon.example")}/users/{r.Number()}";
    }
}