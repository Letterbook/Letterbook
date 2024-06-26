namespace Letterbook.Workers.Tests.Fakes;

/// <summary>
/// A minimal message contract, used by <see cref="FakeConsumer"/>
/// </summary>
public record FakeEvent
{
	public string Data { get; init; }
}