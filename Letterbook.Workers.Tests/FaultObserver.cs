using MassTransit;

namespace Letterbook.Workers.Tests;

public class FaultObserver : IReceiveObserver
{
	public List<FaultSummary> Faults = new();

	public Task PreReceive(ReceiveContext context)
	{
		Assert.True(true);
		return Task.CompletedTask;
	}

	public Task PostReceive(ReceiveContext context)
	{
		Assert.True(true);
		return Task.CompletedTask;
	}

	public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
	{
		return Task.CompletedTask;
	}

	public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
		where T : class
	{
		Faults.Add(new FaultSummary("Consume", context.ReceiveContext.GetMessageTypes(),
			context.ReceiveContext.GetMessageId() ?? Guid.Empty, exception));
		return Task.CompletedTask;
	}

	public Task ReceiveFault(ReceiveContext context, Exception exception)
	{
		Faults.Add(new FaultSummary("Receive", context.GetMessageTypes(), context.GetMessageId() ?? Guid.Empty, exception));
		return Task.CompletedTask;
	}

	public record FaultSummary
	{
		public FaultSummary(string stage, string[] messageTypes, Guid messageId, Exception exception)
		{
			Stage = stage;
			MessageTypes = messageTypes;
			MessageId = messageId;
			Exception = exception;
		}

		public string Stage { get; set; }
		public string[] MessageTypes { get; set; }
		public Guid MessageId { get; set; }
		public Exception Exception { get; set; }
	}
}