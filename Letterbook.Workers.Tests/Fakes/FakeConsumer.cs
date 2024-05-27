using MassTransit;

namespace Letterbook.Workers.Tests.Fakes;

/// <summary>
/// A minimal consumer, used to verify the message bus and test harness are working
/// </summary>
public class FakeConsumer : IConsumer<FakeEvent>
{
	public Task Consume(ConsumeContext<FakeEvent> context) => Task.CompletedTask;
}