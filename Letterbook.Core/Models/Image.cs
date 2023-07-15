using System.Net.Mime;

namespace Letterbook.Core.Models;

public class Image : IExpiring, IObjectRef
{
    public Uri Id { get; set; }
    public string? LocalId { get; set; }
    public Uri Authority { get; set; }
    public Actor Creator { get; set; }
    public ContentType MimeType { get; set; }
    public Uri Location { get; set; } // good enough?
    public DateTime Expiration { get; set; }
    public string? Description { get; set; }
}