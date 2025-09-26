using System.Collections.Immutable;
using System.Security.Claims;
using Letterbook.Core;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Workers.Contracts;
using MassTransit;
using Microsoft.Extensions.Options;
using Claim = System.Security.Claims.Claim;

namespace Letterbook.Workers.Publishers;

public class AccountEventPublisher : IAccountEventPublisher
{
	private readonly ILogger<AccountEventPublisher> _logger;
	private readonly IBus _bus;
	private readonly CoreOptions _options;

	public AccountEventPublisher(IOptions<CoreOptions> options, ILogger<AccountEventPublisher> logger, IBus bus)
	{
		_logger = logger;
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
		var message = FormatMessage(updated, original, nameof(Updated), claims, null);
		await _bus.Publish(message);
	}

	public async Task Verified(Account account, IEnumerable<Claim> claims)
	{
		var message = FormatMessage(account, nameof(Verified), claims);
		await _bus.Publish(message);
	}

	public async Task PasswordResetRequested(Account account, string resetLink)
	{
		// TODO(email): remove this and send email instead
		_logger.LogWarning("User requested password reset for {Email}. Send this {Link}", account.Email, resetLink);
		var message = FormatMessage(account, nameof(PasswordResetRequested), resetLink);
		await _bus.Publish(message);
	}

	private AccountEvent FormatMessage(Account nextValue, string action, string other)
		=> FormatMessage(nextValue, null, action, [], other);
	private AccountEvent FormatMessage(Account nextValue, string action, IEnumerable<Claim> claims)
		=> FormatMessage(nextValue, null, action, claims, null);
	private AccountEvent FormatMessage(Account nextValue, Account? prevValue, string action, IEnumerable<Claim> claims, string? otherData)
	{
		return new AccountEvent
		{
			Subject = nextValue.Id.ToString(),
			Claims = claims.Select(c => (Contracts.Claim)c).ToArray(),
			Type = action,
			NextData = nextValue,
			PrevData = prevValue,
			OtherData = otherData
		};
	}
}