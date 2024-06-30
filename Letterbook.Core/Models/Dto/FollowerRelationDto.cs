using Letterbook.Core.Values;
using Medo;

namespace Letterbook.Core.Models.Dto;

public class FollowerRelationDto
{
	public Uuid7? Follower { get; set; }
	public Uuid7? Follows { get; set; }
	public FollowState State { get; set; }
	public DateTimeOffset Date { get; set; }
}