namespace Letterbook.Core.Models;

/// <summary>
/// IContentRef extends the minimal set of identifiers to also include authorship information, which is common across
/// multiple core models (Note, Image, others in the future)
/// </summary>
public interface IContentRef : IObjectRef
{
    public ObjectCollection<Profile> Creators { get; set; }
    public DateTime CreatedDate { get; set; }
    public ActivityObjectType Type { get; }
}