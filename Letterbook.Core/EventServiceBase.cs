using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Events;

namespace Letterbook.Core;

/// <summary>
/// Base type for event emitter services, which handles opening the channel on the appropriate type
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EventServiceBase<T> where T : IEventChannel
{
	protected IMessageBusAdapter _messageBusAdapter;
	protected IObserver<CloudEvent> _channel;

	protected EventServiceBase(IMessageBusAdapter messageBusAdapter)
	{
		_messageBusAdapter = messageBusAdapter;
		_channel = messageBusAdapter.OpenChannel<T>(GetType().Name);
	}
}