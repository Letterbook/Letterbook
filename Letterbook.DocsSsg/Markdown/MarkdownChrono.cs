using System.Globalization;
using Letterbook.DocsSsg.Files;
using Markdig;

namespace Letterbook.DocsSsg.Markdown;

/// <summary>
/// Markdown loader for files organized into chronological subdirectories
/// <remarks>subdirectories must be named in the yyyy-MM-dd format</remarks>
/// </summary>
public class MarkdownChrono(ILogger<MarkdownChrono> log, IWebHostEnvironment env, IProjectFiles fs, MarkdownPipeline pipeline) :
	MarkdownBase<MarkdownDoc>(env, pipeline), IMarkdownFiles
{
	public List<MarkdownDoc> Files { get; set; } = new();

	public List<MarkdownDoc> GetAll() => Files.Where(IsVisible).ToList();

	public void LoadFrom(string path)
	{
		Files.Clear();
		var files = fs.GetSubdirectories(path).GetPaths().ToList();
		log.LogInformation("Found {Count} files", files.Count);
		foreach (var file in files)
		{
			if (Load(file) is { } doc)
			{
				Files.Add(doc);
				log.LogInformation("Loaded {Path}", file);
			}
			else
			{
				log.LogWarning("Couldn't load {Path}", file);
			}
		}
	}

	public override MarkdownDoc? Load(string path)
	{
		if (!TryGetDate(out var date))
			return default;

		var doc = base.Load(path);
		if (doc == null) return doc;
		doc.Date = date;

		return doc;

		bool TryGetDate(out DateTime dt)
		{
			dt = new DateTime();
			var parts = path.Split('/');
			if (parts.Length != 3) return false;

			var found = DateTime.TryParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
				out var d);
			dt = d;
			return found;
		}
	}

}