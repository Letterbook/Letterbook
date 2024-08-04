using System.Globalization;
using Letterbook.Docs.Files;
using Markdig;
using Microsoft.Extensions.FileProviders;

namespace Letterbook.Docs.Markdown;

/// <summary>
/// Markdown loader for files organized into chronological subdirectories
/// <remarks>subdirectories must be named in the yyyy-MM-dd format</remarks>
/// </summary>
public class MarkdownDate(ILogger<MarkdownDate> log, IWebHostEnvironment env, IProjectFiles fs, MarkdownPipeline pipeline) :
	MarkdownBase<MarkdownDoc>(env, pipeline), IMarkdownFiles
{
	public List<MarkdownDoc> Files { get; set; } = new();

	List<T> IMarkdownFiles.GetAll<T>() => GetAll().Cast<T>().ToList();
	public List<MarkdownDoc> GetAll() => Files.Where(IsVisible)
		.OrderBy(f => f.Date).ThenBy(f => f.Order).ThenBy(f => f.FileName).ToList();

	public void LoadFrom(string path)
	{
		Files.Clear();
		var files = fs.GetSubdirectories(path).Then(fs.GetFiles).Where(f => f.PhysicalPath != null).ToList();
		log.LogInformation("Found {Count} files", files.Count);
		foreach (var file in files)
		{
			if (Load(file) is { } doc)
			{
				Files.Add(doc);
				log.LogInformation("Loaded {Path}", file.PhysicalPath);
			}
			else
			{
				log.LogWarning("Couldn't load {Path}", file.PhysicalPath);
			}
		}
	}

	public override MarkdownDoc? Load(IFileInfo file)
	{
		if (!TryGetDate(out var date))
			return default;

		var doc = base.Load(file);
		if (doc == null) return doc;
		doc.Date = date;

		return doc;

		bool TryGetDate(out DateTime dt)
		{
			var p = Path.GetFileName(Path.GetDirectoryName(file.PhysicalPath!));
			dt = new DateTime();
			// var parts = file.Split('/');
			// if (parts.Length != 3) return false;

			var found = DateTime.TryParseExact(p, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
				out var d);
			dt = d;
			return found;
		}
	}

	public override MarkdownDoc Reload(MarkdownDoc doc) => Load(fs.GetMarkdownDoc(doc)) ?? doc;

	public MarkdownDoc? GetByDate(DateTime date, string slug)
	{
		var doc = GetAll().Where(IsVisible).Where(x => x.Date == date).FirstOrDefault(x => x.Slug == slug.Trim('/'));
		return doc != null ? Reload(doc) : doc;
	}
}