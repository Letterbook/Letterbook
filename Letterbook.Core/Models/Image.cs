using System.Net.Mime;

namespace Letterbook.Core.Models;

public class Image : IExpiring, IContentRef
{
    private Image()
    {
        Id = default!;
        Creators = default!;
        Visibility = default!;
        CreatedDate = default!;
        MimeType = default!;
        FileLocation = default!;
        Expiration = DateTime.MaxValue;
    }

    public Image(Uri id)
    {
        Id = id;
        CreatedDate = default;
        MimeType = default!;
        FileLocation = default!;
        Expiration = DateTime.MaxValue;
    }
    
    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;
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