using Medo;

namespace Letterbook.Core.Models.Dto;

public class PostRequestDto

{
	public Uuid7? Id { get; set; } = Uuid7.NewUuid7();
	public string? Summary { get; set; }
	public string? Source { get; set; }
	public ICollection<MiniProfileDto>? Creators { get; set; } = new HashSet<MiniProfileDto>();
	public DateTimeOffset? CreatedDate { get; set; } = DateTimeOffset.UtcNow;
	public ICollection<ContentDto> Contents { get; set; } = new HashSet<ContentDto>();
	public ICollection<AudienceDto> Audience { get; set; } = new HashSet<AudienceDto>();
	public ICollection<MentionDto> AddressedTo { get; set; } = new HashSet<MentionDto>();
	public PostDto? InReplyTo { get; set; }
}

public class PostDto : PostRequestDto
{
	public Uri? FediId { get; set; }
	public ThreadDto? Thread { get; set; }
	public string? Preview { get; set; }
	public DateTimeOffset? PublishedDate { get; set; }
	public DateTimeOffset? UpdatedDate { get; set; }
	public Uri? Replies { get; set; }
	public IList<PostDto>? RepliesCollection { get; set; } = new List<PostDto>();
	public Uri? Likes { get; set; }
	public IList<MiniProfileDto>? LikesCollection { get; set; } = new List<MiniProfileDto>();
	public Uri? Shares { get; set; }
	public IList<MiniProfileDto>? SharesCollection { get; set; } = new List<MiniProfileDto>();
}