namespace Letterbook.Core.Models;

public record UpdateResponse<T> where T : class
{
    public required T Original { get; set; }
    public T? Updated { get; set; }
}