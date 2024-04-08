using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Api.Dto;

public class FullProfileDto
{
	public required Uuid7 Id { get; set; }
	public Uri? FediId { get; set; }
	public required string Handle { get; set; }
	public required string DisplayName { get; set; }
	public required string Description { get; set; }
	public CustomField[]? CustomFields { get; set; }
	public DateTimeOffset? Updated { get; set; }
	public DateTimeOffset? Created { get; set; }
	public ActivityActorType? Type { get; set; }
	public ICollection<AudienceDto>? Audiences { get; set; } = new HashSet<AudienceDto>();
	public IList<PublicKeyDto>? Keys { get; set; } = new List<PublicKeyDto>();
}