using Letterbook.Core;
using Letterbook.Workers.Contracts;
using MassTransit;

namespace Letterbook.Workers.Consumers
{
	public class TimelineConsumer : IConsumer<PostEvent>
    {
	    private readonly ILogger<TimelineConsumer> _logger;
	    private readonly ITimelineService _timelines;

	    public TimelineConsumer(ILogger<TimelineConsumer> logger, ITimelineService timelines)
	    {
		    _logger = logger;
		    _timelines = timelines;
	    }

        public async Task Consume(ConsumeContext<PostEvent> context)
        {
	        switch (context.Message.EventType)
	        {
		        case PostEvent.PostEventType.Created:
			        // await _timelines.HandleCreate(context.Message.NextData);
			        break;
		        case PostEvent.PostEventType.Deleted:
			        break;
		        case PostEvent.PostEventType.Updated:
			        break;
		        case PostEvent.PostEventType.Published:
			        break;
		        case PostEvent.PostEventType.Liked:
			        break;
		        case PostEvent.PostEventType.Shared:
			        break;
		        default:
			        throw new ArgumentOutOfRangeException();
	        }
        }
    }
}