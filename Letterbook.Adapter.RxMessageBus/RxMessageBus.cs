using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.RxMessageBus;

[AutoAdapter(typeof(IMessageBusAdapter), InjectableScope.Singleton)]
[AutoAdapter(typeof(IMessageBusClient), InjectableScope.Singleton)]
public class RxMessageBus : IMessageBusAdapter, IMessageBusClient
{
    private static readonly Lazy<Dictionary<string, Subject<CloudEvent>>> LazyChannels = new();
    private static Dictionary<string, Subject<CloudEvent>> Channels => LazyChannels.Value;

    private readonly ILogger<RxMessageBus> _logger;

    public RxMessageBus(ILogger<RxMessageBus> logger)
    {
        _logger = logger;
    }

    public IObserver<CloudEvent> OpenChannel(string type)
    {
        var channel = GetSubject(type);
        return channel.AsObserver();
    }

    public IObservable<CloudEvent> ListenChannel(string type)
    {
        var channel = GetSubject(type);
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