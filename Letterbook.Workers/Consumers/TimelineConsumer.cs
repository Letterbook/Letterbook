using Letterbook.Core;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;

namespace Letterbook.Workers.Consumers
{
	public class TimelineConsumer : IConsumer<PostEvent>
	{
	    private readonly ILogger<TimelineConsumer> _logger;
	    private readonly ITimelineService _timelines;
	    private readonly IProfileService _profiles;

	    public TimelineConsumer(ILogger<TimelineConsumer> logger, ITimelineService timelines, IProfileService profiles)
	    {
		    _logger = logger;
		    _timelines = timelines;
		    _profiles = profiles;
	    }

        public async Task Consume(ConsumeContext<PostEvent> context)
        {
	        _logger.LogInformation("Handling PostEvent {EventType} for {PostId}", context.Message.Type, context.Message.Subject);
	        var post = context.Message.NextData;
	        switch (context.Message.Type)
	        {
		        case nameof(PostEventPublisher.Created):
			        return;
		        case nameof(PostEventPublisher.Deleted):
			        await _timelines.HandleDelete(post);
			        return;
		        case nameof(PostEventPublisher.Updated):
			        if (post.PublishedDate is null) return;
			        if (context.Message.PrevData != null)
				        await _timelines.HandleUpdate(post, context.Message.PrevData);
			        else
				        _logger.LogError("Couldn't update timeline for Post {PostId} due to unknown previous state", post.GetId25());
			        return;
		        case nameof(PostEventPublisher.Published):
			        await _timelines.HandlePublish(context.Message.NextData);
			        return;
		        case nameof(PostEventPublisher.Liked):
			        return;
		        case nameof(PostEventPublisher.Shared):
			        if (context.Message.Sender is { } senderId
			            && await _profiles.As(context.Message.Claims).LookupProfile(senderId) is { } sender)
				        await _timelines.HandleShare(post, sender);
			        else _logger.LogError("Couldn't update timeline for Post {PostId} due to missing sender info for {ProfileId}", post.GetId25(), context.Message.Sender);
			        return;
		        default:
			        _logger.LogError("Couldn't handle unknown PostEvent {EventType}", context.Message.Type);
			        return;
	        }
        }
	}
}