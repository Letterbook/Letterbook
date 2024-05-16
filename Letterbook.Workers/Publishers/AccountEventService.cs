using CloudNative.CloudEvents;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Events;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Letterbook.Workers.Publishers;

public class AccountEventPublisher : IAccountEventPublisher
{
	private readonly IBus _bus;
	private readonly IMessageBusAdapter _messageBusAdapter;
	private readonly IObserver<CloudEvent> _channel;
	private readonly CoreOptions _options;

	public AccountEventPublisher(IOptions<CoreOptions> options, IBus bus)
	{
		_bus = bus;
		_options = options.Value;
	}

	public async Task Created(Account account)
	{
		var message = FormatMessage(account, nameof(Created));
		await _bus.Publish(message);
	}

	public async Task Deleted(Account account)
	{
		var message = FormatMessage(account, nameof(Deleted));
		await _bus.Publish(message);
	}

	public async Task Suspended(Account account)
	{
		var message = FormatMessage(account, nameof(Task));
		await _bus.Publish(message);
	}

	public async Task Updated(Account original, Account updated)
	{
		// TODO: warn on equality
		// if (ReferenceEquals(original, updated)) _logger.LogWarning("");
		var message = FormatMessage((original, updated), nameof(Updated));
		await _bus.Publish(message);
	}

	public async Task Verified(Account account)
	{
		var message = FormatMessage(account, nameof(Verified));
		await _bus.Publish(message);
	}

	private CloudEvent FormatMessage(Account value, string action)
	{
		return new CloudEvent
		{
			Id = Guid.NewGuid().ToString(),
			Source = _options.BaseUri(),
			Data = value,
			Type = $"{nameof(AccountEventPublisher)}.{value.GetType()}.{action}",
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
			Type = $"{nameof(AccountEventPublisher)}.{values.updated.GetType()}.{action}",
			Subject = values.updated.Id.ToString(),
			Time = DateTimeOffset.UtcNow,
			["ltrauth"] = ""
		};
	}



}