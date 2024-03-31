using Medo;

namespace Letterbook.Api.Dto;

public class PostDto
{
	public Uuid7? Id { get; set; } = Uuid7.NewUuid7();
	public Uri? FediId { get; set; }
	public ThreadDto? Thread { get; set; }
	public string? Summary { get; set; }
	public string? Preview { get; set; }
	public string? Source { get; set; }
	public ICollection<MiniProfileDto>? Creators { get; set; } = new HashSet<MiniProfileDto>();
	public DateTimeOffset? CreatedDate { get; set; } = DateTimeOffset.UtcNow;
	public DateTimeOffset? PublishedDate { get; set; }
	public DateTimeOffset? UpdatedDate { get; set; }
	public ICollection<ContentDto> Contents { get; set; } = new HashSet<ContentDto>();
	public ICollection<AudienceDto> Audience { get; set; } = new HashSet<AudienceDto>();
	public ICollection<MentionDto> AddressedTo { get; set; } = new HashSet<MentionDto>();
	public PostDto? InReplyTo { get; set; }
	public IList<PostDto>? RepliesCollection { get; set; } = new List<PostDto>();
	public IList<MiniProfileDto>? LikesCollection { get; set; } = new List<MiniProfileDto>();
	public IList<MiniProfileDto>? SharesCollection { get; set; } = new List<MiniProfileDto>();
}