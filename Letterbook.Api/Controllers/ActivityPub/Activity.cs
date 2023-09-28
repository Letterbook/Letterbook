using Letterbook.ActivityPub;

namespace Letterbook.Api.Controllers.ActivityPub;

// TODO: Move to Letterbook.ActivityPub
// There's more complexity here than is reasonable to handle so casually
public static class Activity
{
    public static AsAp.Activity AcceptActivity(string activityType, CompactIri? subjectId = null)
    {
        var subject = new AsAp.Activity()
        {
            Id = subjectId,
            Type = activityType
        };
        return new AsAp.Activity()
        {
            Type = "Accept",
            Object = new List<AsAp.IResolvable>() { subject }
        };
    }
    public static AsAp.Activity AcceptObject(string objectType, CompactIri? subjectId = null)
    {
        var subject = new AsAp.Object()
        {
            Id = subjectId,
            Type = objectType
        };
        return new AsAp.Activity()
        {
            Type = "Accept",
            Object = new List<AsAp.IResolvable>() { subject }
        };
    }
    public static AsAp.Activity RejectObject(string objectType, CompactIri? subjectId = null)
    {
        var subject = new AsAp.Object()
        {
            Id = subjectId,
            Type = objectType
        };
        return new AsAp.Activity()
        {
            Type = "Reject",
            Object = new List<AsAp.IResolvable>() { subject }
        };
    }
    public static AsAp.Activity RejectActivity(string activityType, CompactIri? subjectId = null)
    {
        var subject = new AsAp.Activity()
        {
            Id = subjectId,
            Type = activityType
        };
        return new AsAp.Activity()
        {
            Type = "Reject",
            Object = new List<AsAp.IResolvable>() { subject }
        };
    }
    public static AsAp.Activity TentativeAcceptObject(string objectType, CompactIri? subjectId = null)
    {
        var subject = new AsAp.Object()
        {
            Id = subjectId,
            Type = objectType
        };
        return new AsAp.Activity()
        {
            Type = "TentativeAccept",
            Object = new List<AsAp.IResolvable>() { subject }
        };
    }
    public static AsAp.Activity TentativeAcceptActivity(string activityType, CompactIri? subjectId = null)
    {
        var subject = new AsAp.Activity()
        {
            Id = subjectId,
            Type = activityType
        };
        return new AsAp.Activity()
        {
            Type = "TentativeAccept",
            Object = new List<AsAp.IResolvable>() { subject }
        };
    }
}