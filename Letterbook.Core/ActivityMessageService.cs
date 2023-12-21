using ActivityPub.Types.AS;
using ActivityPub.Types.Conversion;
using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class ActivityMessageService : IActivityMessageService
{
    private readonly IMessageBusAdapter _messageBusAdapter;
    private readonly IJsonLdSerializer _serializer;
    private readonly IObserver<CloudEvent> _channel;
    private readonly CoreOptions _options;
    private readonly ILogger<ActivityMessageService> _logger;

    public ActivityMessageService(ILogger<ActivityMessageService> logger, IOptions<CoreOptions> options,
        IJsonLdSerializer serializer, IMessageBusAdapter messageBusAdapter)
    {
        _messageBusAdapter = messageBusAdapter;
        _options = options.Value;
        _serializer = serializer;
        _logger = logger;
        _channel = _messageBusAdapter.OpenChannel<ASType>();
    }

    public void Deliver(Uri inbox, ASType activity, Profile? onBehalfOf)
    {
        _channel.OnNext(FormatMessage(inbox, activity, onBehalfOf));
    }

    private CloudEvent FormatMessage(Uri inbox, ASType activity, Profile? onBehalfOf)
    {
        var subject = activity.Is<ASObject>(out var o)
            ? o.Id 
            : activity.Is<ASLink>(out var l) 
                ? l.HRef.ToString() 
                : string.Join(',', activity.TypeMap.ASTypes);
        return new CloudEvent
        {
            Id = Guid.NewGuid().ToString(),
            Source = _options.BaseUri(),
            Data = _serializer.Serialize(activity),
            Type = $"{nameof(IActivityMessageService)}",
            Subject = subject,
            Time = DateTimeOffset.UtcNow,
            [IActivityMessageService.DestinationKey] = inbox.ToString(),
            [IActivityMessageService.ProfileKey] = onBehalfOf?.LocalId?.ToShortId() ?? "",
            ["ltrauth"] = "",
        };
    }
}