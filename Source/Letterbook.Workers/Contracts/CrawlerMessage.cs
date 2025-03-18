using Letterbook.Core.Models;

namespace Letterbook.Workers.Contracts;

public class CrawlerMessage
{
	public required ProfileId OnBehalfOf { get; init; }
	public ProfileId Profile { get; init; }
	public required Uri? Resource { get; init; }
	public required int DepthLimit { get; init; }
	public required ExpectedType Type { get; init; }

	public enum ExpectedType
	{
		Unknown,
		Collection,
		Post,
		Profile,
		Thread,
		ProfileOutbox,
		ProfileStream
	}
}