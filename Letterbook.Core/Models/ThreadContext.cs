using Medo;

namespace Letterbook.Core.Models;

public class ThreadContext
{
    public Uuid7 Id { get; set; } = Uuid7.NewUuid7();
    public required Uri IdUri { get; set; }
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    public required Post Root { get; set; }
}