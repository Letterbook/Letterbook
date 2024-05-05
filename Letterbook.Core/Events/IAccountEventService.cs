using Letterbook.Core.Models;

namespace Letterbook.Core.Events;

/// <summary>
/// Events and a corresponding channel related to <see cref="Account">Accounts</see>
/// </summary>
public interface IAccountEvents
{
	public void Created(Account account);
	public void Deleted(Account account);
	public void Suspended(Account account);
	public void Updated(Account original, Account updated);
	public void Verified(Account account);
}