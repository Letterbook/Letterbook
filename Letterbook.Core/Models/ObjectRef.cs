namespace Letterbook.Core.Models;

/// <summary>
/// Useful in cases where we only need the Id, and otherwise don't care what an Object actually is
/// </summary>
public abstract class ObjectRef : IObjectRef
{
    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority
    {
        get => Id.Authority;
    }

    protected ObjectRef(Uri id)
    {
        Id = id;
    }

    protected ObjectRef(Uri id, Guid localId) : this(id)
    {
        LocalId = localId;
    }
}