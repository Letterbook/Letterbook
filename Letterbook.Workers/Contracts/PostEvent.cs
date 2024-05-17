using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Workers.Contracts;

public record PostEvent : EventBase<Post>
{
	public Uuid7? Sender { get; init; }
}