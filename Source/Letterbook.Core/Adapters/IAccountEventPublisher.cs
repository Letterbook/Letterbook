using System.Security.Claims;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Events and a corresponding channel related to <see cref="Account">Accounts</see>
/// </summary>
public interface IAccountEventPublisher
{
	public Task Created(Account account);
	public Task Deleted(Account account, IEnumerable<Claim> claims);
	public Task Suspended(Account account, IEnumerable<Claim> claims);
	public Task Updated(Account original, Account updated, IEnumerable<Claim> claims);
	public Task Verified(Account account, IEnumerable<Claim> claims);
	public Task PasswordResetRequested(Account account, string resetLink);
}