using MassTransit;

namespace Letterbook.Workers.Tests;

public class FaultObserver : IReceiveObserver
{
	public List<FaultSummary> Faults = new();

	public async Task PreReceive(ReceiveContext context)
	{
		Assert.True(true);
	}

	public async Task PostReceive(ReceiveContext context)
	{
		Assert.True(true);
	}

	public async Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
	{
	}

	public async Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception)
		where T : class
	{
		Faults.Add(new FaultSummary("Consume", context.ReceiveContext.GetMessageTypes(),
			context.ReceiveContext.GetMessageId() ?? Guid.Empty, exception));
	}

	public async Task ReceiveFault(ReceiveContext context, Exception exception)
	{
		Faults.Add(new FaultSummary("Receive", context.GetMessageTypes(), context.GetMessageId() ?? Guid.Empty, exception));
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