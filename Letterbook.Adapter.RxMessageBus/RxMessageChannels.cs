using System.Reactive.Subjects;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.RxMessageBus;

public class RxMessageChannels : IRxMessageChannels
{
	private readonly ILogger<RxMessageChannels> _logger;
	private Dictionary<string, Subject<CloudEvent>> _channels;

	public RxMessageChannels(ILogger<RxMessageChannels> logger)
	{
		_logger = logger;
		_channels = new Dictionary<string, Subject<CloudEvent>>();
	}

	public Subject<CloudEvent> GetSubject(string type)
	{
		if (_channels.TryGetValue(type, out var subject))
		{
			_logger.LogDebug("Loaded existing channel for {Type}", type);
			return subject;
		}

		subject = new Subject<CloudEvent>();
		_channels.Add(type, subject);
		_logger.LogInformation("Created channel for {Type}", type);
		return subject;
	}
}