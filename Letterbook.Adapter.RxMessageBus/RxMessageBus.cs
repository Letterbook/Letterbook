using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.RxMessageBus;

public class RxMessageBus : IMessageBusAdapter, IMessageBusClient
{
    private readonly IRxMessageChannels _channels;
    private readonly ILogger<RxMessageBus> _logger;

    public RxMessageBus(ILogger<RxMessageBus> logger, IRxMessageChannels channels)
    {
        _logger = logger;
        _channels = channels;
    }

    public IObserver<CloudEvent> OpenChannel<T>(string? source)
    {
        var channel = _channels.GetSubject(typeof(T).ToString());
        _logger.LogInformation("{Source} ready to send on {Channel}", source, typeof(T));
        return channel.AsObserver();
    }

    public IObservable<CloudEvent> ListenChannel<T>(TimeSpan delay, [CallerMemberName] string? name = "")
    {
        var channel = _channels.GetSubject(typeof(T).ToString());
        _logger.LogInformation("{Name} listening on {Channel}", name, typeof(T));
        return channel
            .SubscribeOn(TaskPoolScheduler.Default)
            .Delay(delay)
            .Do(ce =>
            {
                _logger.LogInformation(
                    "Start handling {Type} message on channel {Channel} in {Name}",
                    ce.Type, typeof(T), name);
                _logger.LogDebug("Start handling {Type} on ({Thread})", ce.Type, Environment.CurrentManagedThreadId);
            })
            .AsObservable();
    }
}