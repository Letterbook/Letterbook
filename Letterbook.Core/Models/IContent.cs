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
	public void Sanitize(IEnumerable<IContentSanitizer> sanitizers);
}