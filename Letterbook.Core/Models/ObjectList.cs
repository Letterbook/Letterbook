namespace Letterbook.Core.Models;

public class ObjectList<T> : List<T>, IObjectRef where T : IObjectRef
{
    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority { get; }
}