using System.Text.Json.Serialization;
using Medo;

namespace Letterbook.Api.Dto;

public class PostDto
{
    public Id25? Id25 { get; set; }
    public Uuid7? Id { get; set; }
    public Uri? FediId { get; set; }
    public Uuid7? Thread { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public string? Source { get; set; }
    public ICollection<Uuid7>? Creators { get; set; } = new HashSet<Uuid7>();
    public DateTimeOffset? CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTimeOffset? PublishedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
    public ICollection<ContentDto> Contents { get; set; } = new HashSet<ContentDto>();
    public ICollection<AudienceDto> Audience { get; set; } = new HashSet<AudienceDto>();
    public ICollection<MentionDto> AddressedTo { get; set; } = new HashSet<MentionDto>();
    public Guid? InReplyTo { get; set; }
    public IList<Uuid7>? RepliesCollection { get; set; } = new List<Uuid7>();
    public IList<Uuid7>? LikesCollection { get; set; } = new List<Uuid7>();
    public IList<Uuid7>? SharesCollection { get; set; } = new List<Uuid7>();
}