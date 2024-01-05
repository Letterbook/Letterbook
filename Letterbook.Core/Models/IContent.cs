using Medo;

namespace Letterbook.Core.Models;

public interface IContent
{
    Uuid7 Id { get; set; }
    Uri IdUri { get; set; }
    Post Post { get; set; }
    string? Summary { get; set; }
    string? Preview { get; set; }
    Uri? Source { get; set; }
    string Type { get; }
}

public abstract class Content : IContent
{
    protected Content()
    {
        Id = Uuid7.NewGuid();
        IdUri = default!;
        Post = default!;
    }
    
    public required Uuid7 Id { get; set; }
    public required Uri IdUri { get; set; }
    public required Post Post { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public Uri? Source { get; set; }
    public abstract string Type { get; }
}