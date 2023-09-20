namespace Letterbook.Core.Models;

public class ObjectCollection<T> : HashSet<T>, IObjectRef where T : IObjectRef
{
    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;

    private ObjectCollection()
    {
        Id = default!;
    }

    public ObjectCollection(Uri id)
    {
        Id = id;
    }

    public static ObjectCollection<T> Creators(Uri id) => new(new Uri(id, "/creators"));
    public static ObjectCollection<T> Audience(Uri id) => new(new Uri(id, "/audience"));
    public static ObjectCollection<T> Followers(Uri id) => new(new Uri(id, "/followers"));
}