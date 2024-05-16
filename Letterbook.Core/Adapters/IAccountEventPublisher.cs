using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Events and a corresponding channel related to <see cref="Account">Accounts</see>
/// </summary>
public interface IAccountEventPublisher
{
	public Task Created(Account account);
	public Task Deleted(Account account);
	public Task Suspended(Account account);
	public Task Updated(Account original, Account updated);
	public Task Verified(Account account);
}