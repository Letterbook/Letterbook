using Letterbook.Api.Dto;
using Letterbook.Core;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Letterbook.Api.Controllers.ActivityPub;

public static class Activity
{
    public static async Task<IActionResult> Undo(Guid id, AsAp.Activity activity, IProfileService profile, ILogger logger)
    {
        if (activity.Object.Count > 1)
            return new BadRequestObjectResult(new ErrorMessage(
                ErrorCodes.None.With((int)ActivityPubErrorCodes.UnknownSemantics), "Cannot Undo multiple Activities"));
        if (activity.Object.SingleOrDefault() is not AsAp.Activity subject)
            return new BadRequestObjectResult(new ErrorMessage(
                ErrorCodes.None.With((int)ActivityPubErrorCodes.UnknownSemantics), "Object of an Undo must be another Activity"));
        var activityType = Enum.Parse<ActivityType>(subject.Type);
        switch (activityType)
        {
            case ActivityType.Announce:
                throw new NotImplementedException();
            case ActivityType.Block:
                throw new NotImplementedException();
            case ActivityType.Follow:
                if ((activity.Actor.SingleOrDefault() ?? subject.Actor.SingleOrDefault()) is not AsAp.Actor actor)
                    return new BadRequestObjectResult(new ErrorMessage(
                        ErrorCodes.None.With((int)ActivityPubErrorCodes.UnknownSemantics),
                        "Exactly one Actor can unfollow at a time"));
                if (actor.Id is null)
                    return new BadRequestObjectResult(new ErrorMessage(ErrorCodes.InvalidRequest,
                        "Actor ID is required to unfollow"));
                
                await profile.RemoveFollower(id, actor.Id);
                
                return new OkResult();
            case ActivityType.Like:
                throw new NotImplementedException();
            default:
                logger.LogInformation("Ignored unknown Undo target {ActivityType}", activityType);
                return new AcceptedResult();
        }
    }
}