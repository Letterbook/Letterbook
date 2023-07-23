namespace Letterbook.Core.Models;

public class ObjectCollection<T> : HashSet<T>, IObjectRef where T : IObjectRef
{
    public Uri Id { get; set; }
    public string? LocalId { get; set; }
    public string Authority { get; }
}