namespace Letterbook.Core.Models;

public class NoneObject : IObjectRef
{
    public Uri Id { get; set; }
    public Guid? LocalId { get; set; } = null;
    public string Authority => "None";
}