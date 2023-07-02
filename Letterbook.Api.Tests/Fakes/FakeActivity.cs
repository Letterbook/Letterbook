using Bogus;
using Fedodo.NuGet.ActivityPub.Model.ActorTypes;
using Fedodo.NuGet.ActivityPub.Model.CoreTypes;
using Fedodo.NuGet.ActivityPub.Model.JsonConverters.Model;
using Object = Fedodo.NuGet.ActivityPub.Model.CoreTypes.Object;

namespace Letterbook.Api.Tests.Fakes;

public class FakeActivity : Faker<Activity>
{
    public static string[] ActivityTypes = { 
        "Accept",
        "Add",
        "Announce",
        "Arrive",
        "Block",
        "Create",
        "Delete",
        "Dislike",
        "Flag",
        "Follow",
        "Ignore",
        "Invite",
        "Join",
        "Leave",
        "Like",
        "Listen",
        "Move",
        "Offer",
        "Question",
        "Reject",
        "Read",
        "Remove",
        "TentativeReject",
        "TentativeAccept",
        "Travel",
        "Undo",
        "Update",
        "View",
    };
    public static string[] CommonActivityTypes = { 
        "Accept",
        "Add",
        "Block",
        "Create",
        "Delete",
        "Follow",
        "Ignore",
        "Like",
        "Reject",
        "Remove",
        "Update",
    };
    
    public FakeActivity(string activityType, Object subject, Actor actor)
    {
        RuleFor(o => o.Type, f => activityType);
        RuleFor(o => o.Id, (f, o) => new Uri($"https://letterbook.example/{activityType}/{f.Random.Number()}"));
        RuleFor(o => o.Actor, (f, o) => new TripleSet<Object>
        {
            Objects = new []{actor}
        });
        RuleFor(o => o.Attachment, () => null);
        RuleFor(o => o.Audience, () => null);
        RuleFor(o => o.To, () => null);
        RuleFor(o => o.Bto, () => null);
        RuleFor(o => o.Cc, () => null);
        RuleFor(o => o.Bcc, () => null);
        RuleFor(o => o.Name, () => null);
        RuleFor(o => o.Summary, () => null);
        RuleFor(o => o.Context, () => null);
        RuleFor(o => o.Generator, () => null);
        RuleFor(o => o.Preview, () => null);
        RuleFor(o => o.InReplyTo, () => null);
        RuleFor(o => o.Object, () => new TripleSet<Object>
        {
            Objects = new []{subject}
        });
    }

    public FakeActivity(Object subject, Actor actor) : this("Create", subject, actor)
    {
    }

    public FakeActivity(Actor actor) : this(new FakeNote().Generate(), actor)
    {
    }

    public FakeActivity(Object subject) : this(subject, new FakeActor().Generate())
    {
    }
    public FakeActivity() : this(new FakeNote().Generate())
    {
    }
}