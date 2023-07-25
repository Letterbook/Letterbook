using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.Logging;

namespace Letterbook.Adapter.RxMessageBus;

[AutoAdapter(typeof(IMessageBusAdapter), InjectableScope.Singleton)]
[AutoAdapter(typeof(IMessageBusClient), InjectableScope.Singleton)]
public class RxMessageBus : IMessageBusAdapter, IMessageBusClient
{
    private static readonly Lazy<Dictionary<Type, Subject<object>>> LazyChannels = new();
    private static Dictionary<Type, Subject<object>> Channels => LazyChannels.Value;

    private readonly ILogger<RxMessageBus> _logger;

    public RxMessageBus(ILogger<RxMessageBus> logger)
    {
        _logger = logger;
    }

    public IObserver<T> OpenChannel<T>()
    {
        var channel = GetSubject<T>();
        return channel.AsObserver();
    }

    public IObservable<T> ListenChannel<T>()
    {
        var channel = GetSubject<T>();
        return channel.AsObservable()
            .SubscribeOn(TaskPoolScheduler.Default);
    }

    private Subject<T> GetSubject<T>()
    {
        Subject<T> subject;

        if (!Channels.ContainsKey(typeof(T)))
        {
            subject = new Subject<T>();
            Channels.Add(typeof(T), (subject as Subject<object>)!);
            _logger.LogInformation("Created Channel for {Type}", typeof(T));
        }
        else
        {
            subject = (Channels[typeof(T)] as Subject<T>)!;
        }

        return subject;
    }
}