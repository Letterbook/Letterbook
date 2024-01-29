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