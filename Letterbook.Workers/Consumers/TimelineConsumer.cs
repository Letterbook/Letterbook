using Letterbook.Workers.Contracts;
using MassTransit;

namespace Letterbook.Workers.Consumers
{
	public class TimelineConsumer :
        IConsumer<PostEvent>
    {
        public Task Consume(ConsumeContext<PostEvent> context)
        {
            return Task.CompletedTask;
        }
    }
}