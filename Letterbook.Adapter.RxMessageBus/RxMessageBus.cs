using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.RxMessageBus;

public class RxMessageBus : IMessageBusAdapter, IMessageBusClient
{
    private readonly Dictionary<string, Subject<CloudEvent>> Channels;// => new();
    private readonly ILogger<RxMessageBus> _logger;

    public RxMessageBus(ILogger<RxMessageBus> logger)
    {
        _logger = logger;
        Channels = new Dictionary<string, Subject<CloudEvent>>();
    }

    public IObserver<CloudEvent> OpenChannel<T>()
    {
        var channel = GetSubject(typeof(T).ToString());
        return channel.AsObserver();
    }

    public IObservable<CloudEvent> ListenChannel<T>()
    {
        // TODO: Figure out how to define a new DI scope on subscribe
        var channel = GetSubject(typeof(T).ToString());
        return channel.AsObservable()
            .SubscribeOn(TaskPoolScheduler.Default);
    }

    private Subject<CloudEvent> GetSubject(string type)
    {
        Subject<CloudEvent> subject;

        if (!Channels.ContainsKey(type))
        {
            subject = new Subject<CloudEvent>();
            Channels.Add(type, subject);
            _logger.LogInformation("Created Channel for {Type}", type);
        }
        else
        {
            subject = Channels[type];
        }

        return subject;
    }
}