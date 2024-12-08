using Letterbook.Docs.Files;
using Markdig;
using Microsoft.Extensions.FileProviders;

namespace Letterbook.Docs.Markdown;

/// <summary>
/// Markdown loader for files organized into categories by subdirectory
/// <remarks>categories </remarks>
/// </summary>
public class LoadCategories(ILogger<LoadCategories> log, IWebHostEnvironment env, IProjectFiles fs, MarkdownPipeline pipeline) :
	LoaderBase<MarkdownCategory>(env, pipeline), IMarkdownFiles
{
	public List<MarkdownCategory> Files { get; set; } = new();

	public List<MarkdownCategory> GetAll() => Files.Where(IsVisible)
		.OrderBy(f => f.Date).ThenBy(f => f.Order).ThenBy(f => f.FileName).ToList();

	List<T> IMarkdownFiles.GetAll<T>() => GetAll().Cast<T>().ToList();

	public void LoadFrom(string path)
	{
		Files.Clear();
		var uncategorized = fs.GetFiles(path).ToList();
		log.LogInformation("Found {Count} uncategorized files", uncategorized.Count);
		foreach (var file in uncategorized)
		{
			if (Load(file, null) is { } doc)
				Files.Add(doc);
		}

		var categoryDirs = fs.GetSubdirectories(path);
		foreach (var categoryDir in categoryDirs)
		{
			var files = categoryDir.Then(fs.GetFiles).ToList();
			log.LogInformation("Found {Count} files in {Category}", files.Count, categoryDir.Name);
			foreach (var file in files)
			{
				if (Load(file, categoryDir.Name) is { } doc)
					Files.Add(doc);
			}
		}
	}

	private MarkdownCategory? Load(IFileInfo file, string? category)
	{
		var doc = Load(file);
		if (doc != null)
		{
			doc.Category = category;
		}

		return doc;
	}

	MarkdownDoc IMarkdownFiles.Reload(MarkdownDoc doc) => Reload(new MarkdownCategory(doc));

	public override MarkdownCategory Reload(MarkdownCategory doc) => Load(fs.GetMarkdownDoc(doc)) ?? doc;

	public MarkdownCategory? GetByCategory(string? category, string slug)
	{
		var doc = GetAll().Where(IsVisible).Where(x => x.Category == category).FirstOrDefault(x => x.Slug == slug.Trim('/'));
		return doc != null ? Reload(doc) : doc;
	}
}