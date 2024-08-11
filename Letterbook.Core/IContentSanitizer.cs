using System.Net.Mime;

namespace Letterbook.Core;

public interface IContentSanitizer
{
	public string Sanitize(string content, string baseUrl = "");
	public ContentType ContentType { get; }
}