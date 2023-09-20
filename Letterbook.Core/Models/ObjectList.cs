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
    
    public static ObjectList<T> Replies(Uri id) => new(new Uri(id, "/replies"));
    public static ObjectList<T> Likes(Uri id) => new(new Uri(id, "/likes"));
    public static ObjectList<T> Boosts(Uri id) => new(new Uri(id, "/boosts"));

}