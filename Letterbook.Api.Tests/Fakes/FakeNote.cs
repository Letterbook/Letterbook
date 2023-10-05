using Bogus;
using Letterbook.ActivityPub;

namespace Letterbook.Api.Tests.Fakes;

public class FakeNote : Faker<AsAp.Object>
{
    public FakeNote(AsAp.Actor actor)
    {
        var audiences = new []
        {
            new AsAp.Link("https://www.w3.org/ns/activitystreams#Public"),
            new AsAp.Link(FollowersUri(actor).ToString()),
        };
        RuleFor(o => o.Type, f => "Note");
        RuleFor(o => o.Name, (f, o) => null);
        RuleFor(o => o.Type, f => "Note");
        RuleFor(o => o.Id, (f, o) => new CompactIri(actor.Id! + $"/{f.Random.Int()}"));
        RuleFor(o => o.Attachment, () => null);
        RuleFor(o => o.Audience, (faker) => new List<AsAp.IResolvable>() { faker.PickRandom(audiences) });
        RuleFor(o => o.Content, f => f.Lorem.Sentences(f.Random.Number(10)));
        RuleFor(o => o.Context, () => null);
        RuleFor(o => o.Generator, () => null);
        RuleFor(o => o.Preview, () => null);
    }

    public FakeNote() : this(new FakeActor().Generate())
    {
    }

    private static Uri FollowersUri(AsAp.Actor actor)
    {
        return actor.Followers?.Id ?? new Uri(actor.Id ?? new Uri("https://letterbook.example/users/0"), "/followers");
    }

    private static string RandomActorId()
    {
        var r = new Randomizer();
        return $"https://{(r.Bool() ? "letterbook.example" : "mastodon.example")}/users/{r.Number()}";
    }
}