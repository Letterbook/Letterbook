using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Workers.Contracts;

public record PostEvent : EventBase<Post>
{
	public required PostEventType EventType { get; init; }
	public Uuid7? Sender { get; init; }

	public enum PostEventType
	{
		Created,
		Deleted,
		Updated,
		Published,
		Liked,
		Shared
	}
}