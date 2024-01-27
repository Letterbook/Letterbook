using System.Net.Mime;
using Medo;

namespace Letterbook.Core.Models;

public class Image : IExpiring, IContentRef
{
    private Image()
    {
        FediId = default!;
        Creators = default!;
        Visibility = default!;
        CreatedDate = default!;
        MimeType = default!;
        FileLocation = default!;
        Expiration = DateTime.MaxValue;
    }

    public Image(Uri id)
    {
        FediId = id;
        CreatedDate = default;
        MimeType = default!;
        FileLocation = default!;
        Expiration = DateTime.MaxValue;
    }
    
    public Uri FediId { get; set; }
    public Uuid7 Id { get; set; }
    public string Authority => FediId.Authority;
    public ICollection<Profile> Creators { get; set; } = new HashSet<Profile>();
    public DateTime CreatedDate { get; set; }
    public ActivityObjectType Type => ActivityObjectType.Image;
    public ContentType MimeType { get; set; }
    public Uri FileLocation { get; set; } // good enough?
    public DateTime Expiration { get; set; }
    public string? Description { get; set; }
    public ICollection<Audience> Visibility { get; set; } = new HashSet<Audience>();
    public ICollection<Mention> Mentions { get; set; } = new HashSet<Mention>();
}