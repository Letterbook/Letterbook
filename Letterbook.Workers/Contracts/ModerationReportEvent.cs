using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;

namespace Letterbook.Workers.Contracts;

public record ModerationReportEvent
{
	public ProfileId Author { get; init; }
	public Guid Moderator { get; init; }
	public required FullModerationReportDto NextData { get; init; }
	public required Claim[] Claims { get; init; }
	public required string Type { get; init; }
};