namespace Letterbook.Core.Models;

/// <summary>
/// Useful in cases where we only need the Id, and otherwise don't care what an Object actually is
/// </summary>
public class ObjectRef : IObjectRef
{
    public Uri Id { get; set; }
    public string? LocalId { get; set; }
    public string Authority { get; set; }
}