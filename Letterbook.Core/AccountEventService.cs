using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class AccountEventService : IAccountEventService
{
    private readonly IMessageBusAdapter _messageBusAdapter;
    private readonly IObserver<CloudEvent> _channel;
    private readonly CoreOptions _options;

    public AccountEventService(IOptions<CoreOptions> options, IMessageBusAdapter messageBusAdapter)
    {
        _options = options.Value;
        _messageBusAdapter = messageBusAdapter;
        _channel = _messageBusAdapter.OpenChannel<Account>(nameof(ActivityEventService));
    }

    public void Created(Account account)
    {
        var message = FormatMessage(account, nameof(Created));
        _channel.OnNext(message);
    }

    public void Deleted(Account account)
    {
        var message = FormatMessage(account, nameof(Deleted));
        _channel.OnNext(message);
    }

    public void Suspended(Account account)
    {
        var message = FormatMessage(account, nameof(Suspended));
        _channel.OnNext(message);
    }

    public void Updated(Account original, Account updated)
    {
        // TODO: warn on equality
        // if (ReferenceEquals(original, updated)) _logger.LogWarning("");
        var message = FormatMessage((original, updated), nameof(Updated));
        _channel.OnNext(message);
    }

    public void Verified(Account account)
    {
        var message = FormatMessage(account, nameof(Verified));
        _channel.OnNext(message);
    }
    
    private CloudEvent FormatMessage(Account value, string action)
    {
        return new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Source = _options.BaseUri(),
            Data = value,
            Type = $"{nameof(ActivityEventService)}.{value.GetType()}.{action}",
            Subject = value.Id.ToString(),
            Time = DateTimeOffset.UtcNow,
            ["ltrauth"] = "" // I'd really like events to carry authentication info
            // But then either core services will require auth info as tramp data
            // Or controllers have to send events, rather than core services
        };
    }
    
    private CloudEvent FormatMessage((Account original, Account updated) values, string action)
    {
        return new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Source = _options.BaseUri(),
            Data = values,
            Type = $"{nameof(ActivityEventService)}.{values.updated.GetType()}.{action}",
            Subject = values.updated.Id.ToString(),
            Time = DateTimeOffset.UtcNow,
            ["ltrauth"] = ""
        };
    }
    
    

}