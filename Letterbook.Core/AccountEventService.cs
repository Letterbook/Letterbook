using CloudNative.CloudEvents;
using Letterbook.Core.Adapters;
using Letterbook.Core.Events;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Letterbook.Core;

public class AccountEventService : IAccountEvents
{
	private readonly IBus _bus;
	private readonly CoreOptions _options;

	public AccountEventService(IOptions<CoreOptions> options, IBus bus)
	{
		_bus = bus;
		_options = options.Value;
	}

	public void Created(Account account)
	{
		var message = FormatMessage(account, nameof(Created));
		_bus.Publish(message);
	}

	public void Deleted(Account account)
	{
		var message = FormatMessage(account, nameof(Deleted));
		_bus.Publish(message);
	}

	public void Suspended(Account account)
	{
		var message = FormatMessage(account, nameof(Suspended));
		_bus.Publish(message);
	}

	public void Updated(Account original, Account updated)
	{
		// TODO: warn on equality
		// if (ReferenceEquals(original, updated)) _logger.LogWarning("");
		var message = FormatMessage((original, updated), nameof(Updated));
		_bus.Publish(message);
	}

	public void Verified(Account account)
	{
		var message = FormatMessage(account, nameof(Verified));
		_bus.Publish(message);
	}

	private CloudEvent FormatMessage(Account value, string action)
	{
		return new CloudEvent
		{
			Id = Guid.NewGuid().ToString(),
			Source = _options.BaseUri(),
			Data = value,
			Type = $"{nameof(AccountEventService)}.{value.GetType()}.{action}",
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
			Type = $"{nameof(AccountEventService)}.{values.updated.GetType()}.{action}",
			Subject = values.updated.Id.ToString(),
			Time = DateTimeOffset.UtcNow,
			["ltrauth"] = ""
		};
	}



}