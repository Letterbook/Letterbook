namespace Letterbook.Core.Models;

// It will make sense to be able to have an expiration date on a *lot* of things. Cached remote media and moderation
// action seem like obvious examples
public interface IExpiring
{
	public DateTime Expiration { get; set; }
}