using Bogus;
using Letterbook.ActivityPub;

#pragma warning disable CS8603 // Possible null reference return.

namespace Letterbook.Api.Tests.Fakes;

public class FakeActivity : Faker<AsAp.Activity>
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
    
    public FakeActivity(string? activityType = null, AsAp.Actor? actor = null)
    {
        RuleFor(o => o.Type, (f) => activityType ?? f.PickRandom(CommonActivityTypes));
        RuleFor(o => o.Id, (f) => new CompactIri($"https://letterbook.example/{activityType}/{f.Random.Number()}"));
        RuleFor(o => o.Actor, () => new List<AsAp.IResolvable> { actor ?? new FakeActor().Generate() } );
        RuleFor(o => o.Attachment, () => null);
        RuleFor(o => o.Audience, () => null);
        RuleFor(o => o.To, () => null);
        RuleFor(o => o.Bto, () => null);
        RuleFor(o => o.Cc, () => null);
        RuleFor(o => o.Bcc, () => null);
        RuleFor(o => o.Name, () => null);
        RuleFor(o => o.Summary, () => null);
        RuleFor(o => o.Preview, () => null);
        RuleFor(o => o.InReplyTo, () => null);
        RuleFor(o => o.Object, () => new List<AsAp.IResolvable> { new FakeNote().Generate() });
    }
}