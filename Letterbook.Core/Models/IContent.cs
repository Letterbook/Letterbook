using Medo;

namespace Letterbook.Core.Models;

public interface IContent
{
    Guid Id { get; set; }
    Uri FediId { get; set; }
    Post Post { get; set; }
    string? Summary { get; set; }
    string? Preview { get; set; }
    Uri? Source { get; set; }
    string Type { get; }

    public Uuid7 GetId();
    public string GetId25();
    public string? GeneratePreview();
    public void Sanitize();
}

public abstract class Content : IContent
{
    private Uuid7 _id;

    protected Content()
    {
        Id = Uuid7.NewUuid7();
        FediId = default!;
        Post = default!;
    }

    public Guid Id
    {
        get => _id;
        set => _id = value;
    }

    public required Uri FediId { get; set; }
    public required Post Post { get; set; }
    public string? Summary { get; set; }
    public string? Preview { get; set; }
    public Uri? Source { get; set; }
    public abstract string Type { get; }
    
    public Uuid7 GetId() => _id;
    public string GetId25() => _id.ToId25String();
    public abstract string? GeneratePreview();
    public abstract void Sanitize();
}