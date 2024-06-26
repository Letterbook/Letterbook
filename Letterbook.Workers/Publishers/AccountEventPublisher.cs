using System.Collections.Immutable;
using System.Security.Claims;
using CloudNative.CloudEvents;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Letterbook.Workers.Contracts;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Letterbook.Workers.Publishers;

public class AccountEventPublisher : IAccountEventPublisher
{
	private readonly IBus _bus;
	private readonly CoreOptions _options;

	public AccountEventPublisher(IOptions<CoreOptions> options, IBus bus)
	{
		_bus = bus;
		_options = options.Value;
	}

	public async Task Created(Account account)
	{
		var message = FormatMessage(account, nameof(Created), []);
		await _bus.Publish(message);
	}

	public async Task Deleted(Account account, IEnumerable<Claim> claims)
	{
		var message = FormatMessage(account, nameof(Deleted), claims);
		await _bus.Publish(message);
	}

	public async Task Suspended(Account account, IEnumerable<Claim> claims)
	{
		var message = FormatMessage(account, nameof(Suspended), claims);
		await _bus.Publish(message);
	}

	public async Task Updated(Account original, Account updated, IEnumerable<Claim> claims)
	{
		var message = FormatMessage(updated, original, nameof(Updated), claims);
		await _bus.Publish(message);
	}

	public async Task Verified(Account account, IEnumerable<Claim> claims)
	{
		var message = FormatMessage(account, nameof(Verified), claims);
		await _bus.Publish(message);
	}

	private AccountEvent FormatMessage(Account nextValue, string action, IEnumerable<Claim> claims)
		=> FormatMessage(nextValue, null, action, claims);
	private AccountEvent FormatMessage(Account nextValue, Account? prevValue, string action, IEnumerable<Claim> claims)
	{
		return new AccountEvent
		{
			Subject = nextValue.Id.ToString(),
			Claims = claims.ToImmutableHashSet(),
			Type = action,
			NextData = nextValue,
			PrevData = prevValue
		};
	}
}