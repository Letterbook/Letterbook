using Letterbook.ActivityPub;

namespace Letterbook.Core.Models;

/// <summary>
/// Useful in cases where we only need the Id, and otherwise don't care what an Object actually is
/// </summary>
public class ObjectRef : DTO.Link, IObjectRef
{
    public new Uri Id
    {
        get => base.Id!;
        set => base.Id = value.ToString();
    }
    public string? LocalId { get; set; }

    public string Authority
    {
        get => Id.Authority;
    }

    public ObjectRef(string href) : base(href)
    {
    }

    public ObjectRef(CompactIri href) : base(href)
    {
    }
}