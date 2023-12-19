using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.RxMessageBus;

public class RxMessageBus : IMessageBusAdapter, IMessageBusClient
{
    private readonly Dictionary<string, Subject<CloudEvent>> _channels;
    private readonly ILogger<RxMessageBus> _logger;

    public RxMessageBus(ILogger<RxMessageBus> logger)
    {
        _logger = logger;
        _channels = new Dictionary<string, Subject<CloudEvent>>();
    }

    public IObserver<CloudEvent> OpenChannel<T>()
    {
        _logger.LogInformation("Opened channel {Channel}", typeof(T));
        var channel = GetSubject(typeof(T).ToString());
        return channel.AsObserver();
    }

    public IObservable<CloudEvent> ListenChannel<T>(TimeSpan delay, [CallerMemberName]string? name = "")
    {
        _logger.LogInformation("{Name} listening on {Channel}", name, typeof(T));
        var channel = GetSubject(typeof(T).ToString());
        return channel.AsObservable()
            .Delay(delay)
            .Do(ce => _logger.LogInformation(
                "{Name} handling message on channel {Channel} - {Id} type {Type} subject {Subject}", 
                name, typeof(T), ce.Id, ce.Type, ce.Subject))
            .SubscribeOn(TaskPoolScheduler.Default);
    }

    private Subject<CloudEvent> GetSubject(string type)
    {
        Subject<CloudEvent> subject;

        if (!_channels.ContainsKey(type))
        {
            subject = new Subject<CloudEvent>();
            _channels.Add(type, subject);
            _logger.LogInformation("Created channel for {Type}", type);
        }
        else
        {
            subject = _channels[type];
        }

        return subject;
    }
}