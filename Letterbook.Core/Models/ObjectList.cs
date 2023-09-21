namespace Letterbook.Core.Models;

public class ObjectList<T> : List<T>, IObjectRef where T : IObjectRef
{
    public Uri Id { get; set; }
    public Guid? LocalId { get; set; }
    public string Authority => Id.Authority;

    private ObjectList()
    {
        Id = default!;
    }

    public ObjectList(Uri id)
    {
        Id = id;
    }
    
    public static ObjectList<T> Replies(Uri id)
    {
        var b = new UriBuilder(id);
        b.Path += "/replies";
        return new ObjectList<T>(b.Uri);
    }
    public static ObjectList<T> Likes(Uri id)
    {
        var b = new UriBuilder(id);
        b.Path += "/likes";
        return new ObjectList<T>(b.Uri);
    }
    public static ObjectList<T> Boosts(Uri id)
    {
        var b = new UriBuilder(id);
        b.Path += "/boosts";
        return new ObjectList<T>(b.Uri);
    }

}