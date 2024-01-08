using Medo;

namespace Letterbook.Core.Models;

public interface IContent
{
    Uuid7 Id { get; set; }
    Uri FediId { get; set; }
    Post Post { get; set; }
    string? Summary { get; set; }
    string? Preview { get; set; }
    Uri? Source { get; set; }
    string Type { get; }

    public string? GeneratePreview();
    public void Sanitize();
}

public abstract class Content : IContent
{
    protected Content()
    {
        Id = Uuid7.NewUuid7();
        FediId = default!;
        Post = default!;
    }
    
    public Uuid7 Id { get; set; }
    public required Uri FediId { get; set; }
    public required Post Post { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public Uri? Source { get; set; }
    public abstract string Type { get; }
    
    public abstract string? GeneratePreview();

    public abstract void Sanitize();
}