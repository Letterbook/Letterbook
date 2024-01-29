using Medo;

namespace Letterbook.Core.Models;

/// <summary>
/// Useful in cases where we only need the Id, and otherwise don't care what an Object actually is
/// </summary>
public abstract class ObjectRef : IFederated
{
    public Uri FediId { get; set; }
    public Uuid7 Id { get; set; }
    public string Authority
    {
        get => FediId.Authority;
    }

    protected ObjectRef(Uri id)
    {
        FediId = id;
    }

    protected ObjectRef(Uri id, Guid localId) : this(id)
    {
        Id = localId;
    }
}