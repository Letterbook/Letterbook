using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

/// <summary>
/// ActivityEventService provides a loosely coupled publish/subscribe interface between other core services. Typically,
/// core services will be narrowly scoped, and will publish a message through the event service. Other services may
/// consume that message and perform their own processing in response. For instance, to send notifications.
/// </summary>
public class ActivityEventService : IActivityEventService
{
    private readonly CoreOptions _options;
    private readonly IMessageBusAdapter _messageBusAdapter;
    private readonly IObserver<CloudEvent> _notesChannel;
    private readonly IObserver<CloudEvent> _imagesChannel;
    private readonly IObserver<CloudEvent> _profileChannel;

    public ActivityEventService(IOptions<CoreOptions> options, IMessageBusAdapter messageBusAdapter)
    {
        _options = options.Value;
        _messageBusAdapter = messageBusAdapter;
        _notesChannel = _messageBusAdapter.OpenChannel<Note>();
        _imagesChannel = _messageBusAdapter.OpenChannel<Image>();
        _profileChannel = _messageBusAdapter.OpenChannel<Profile>();
    }

    public void Created<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Created));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Updated<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Updated));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Deleted<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Deleted));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Flagged<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Flagged));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Liked<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Liked));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Boosted<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Boosted));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Approved<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Approved));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Rejected<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Rejected));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Requested<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Requested));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }
    
    public void Offered<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Offered));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    public void Mentioned<T>(T value) where T : class, IObjectRef
    {
        var message = FormatMessage(value, nameof(Mentioned));
        var channel = GetChannel(value);
        channel.OnNext(message);
    }

    private IObserver<CloudEvent> GetChannel(IObjectRef value)
    {
        return value switch
        {
            Note => _notesChannel,
            Image => _imagesChannel,
            Profile => _profileChannel,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value.GetType(), $"Unsupported type {value.GetType()}")
        };
    }

    private CloudEvent FormatMessage(IObjectRef value, string action)
    {
        return new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Source = CoreOptions.BaseUri(_options),
            Data = value,
            Type = $"{nameof(ActivityEventService)}.{value.GetType()}.{action}",
            Subject = value.Id.ToString(),
            Time = DateTimeOffset.UtcNow
        };
    }
}
