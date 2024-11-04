using Letterbook.Workers.Publishers;
using MassTransit;
using MassTransit.Middleware;

namespace Letterbook.Workers.Consumers;

public class OutboundPostConsumerDefinition :
	ConsumerDefinition<OutboundPostConsumer>
{
	protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpoint,
		IConsumerConfigurator<OutboundPostConsumer> consumer, IRegistrationContext context)
	{
		consumer.UseFilter(new ContextFilter<ConsumerConsumeContext<OutboundPostConsumer>>(async consumeContext =>
		{
			await Task.FromResult(0);
			return consumeContext.Headers.TryGetHeader(Headers.Event, out var header) &&
			       header switch
			       {
				       nameof(PostEventPublisher.Published) => true,
				       nameof(PostEventPublisher.Updated) => true,
				       nameof(PostEventPublisher.Deleted) => true,
				       nameof(PostEventPublisher.Shared) => true,
				       nameof(PostEventPublisher.Liked) => true,
				       _ => false
			       };
		}));
	}
}