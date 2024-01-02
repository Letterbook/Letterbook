namespace Letterbook.Core.Models;

public interface IContent
{
    Guid Id { get; set; }
    Uri Uri { get; set; }
    string? Summary { get; set; }
    string? Preview { get; set; }
    Uri? Source { get; set; }

    string Type { get; }
}