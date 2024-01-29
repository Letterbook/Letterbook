using System.Diagnostics.CodeAnalysis;
using Letterbook.Core.Extensions;
using Medo;

namespace Letterbook.Core.Models;

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
    
    public static Uri LocalId(IContent content, CoreOptions opts) =>
        new(opts.BaseUri(), $"{content.Type}/{content.Id.ToId25String()}");
    
    public abstract string? GeneratePreview();

    public abstract void Sanitize();

    public void SetLocalFediId(CoreOptions opts)
    {
        FediId = LocalId(this, opts);
    }
}