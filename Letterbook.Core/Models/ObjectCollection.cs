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

    public static ObjectCollection<T> Creators(Uri id)
    {
        var b = new UriBuilder(id);
        b.Path += "/creators";
        return new ObjectCollection<T>(b.Uri);
    }

    public static ObjectCollection<T> Audience(Uri id)
    {
        var b = new UriBuilder(id);
        b.Path += "/audience";
        return new ObjectCollection<T>(b.Uri);
    }
    public static ObjectCollection<T> Followers(Uri id)
    {
        var b = new UriBuilder(id);
        b.Path += "/followers";
        return new ObjectCollection<T>(b.Uri);
    }
}