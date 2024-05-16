using Medo;

namespace Letterbook.Workers.Contracts;

public record ActivityMessage : EventBase<string>
{
	public required Uuid7 OnBehalfOf { get; init; }
	public required Uri Inbox { get; init; }
}