using Letterbook.Docs.Files;
using Markdig;

namespace Letterbook.Docs.Markdown;

/// <summary>
/// Markdown loader for files in a directory
/// </summary>
public class LoadDirectory(ILogger<LoadDirectory> log, IWebHostEnvironment env, IProjectFiles fs, MarkdownPipeline pipeline) :
	LoaderBase(env, pipeline), IMarkdownFiles
{
	public List<MarkdownDoc> Files { get; set; } = [];

	public List<T> GetAll<T>() where T : MarkdownDoc => Files.Where(IsVisible)
		.OrderBy(f => f.Order)
		.ThenBy(f => f.Date)
		.ThenBy(f => f.FileName)
		.Cast<T>()
		.ToList();

	public void LoadFrom<T>(string dir) where T : MarkdownDoc
	{
		Files.Clear();
		var files = fs.GetFiles(dir).ToList();
		log.LogInformation("Found {Count} files in directory {Dir}", files.Count, dir);
		foreach (var file in files)
		{
			if (Load<T>(file) is { } doc)
				Files.Add(doc);
		}
	}

	public T Reload<T>(T doc) where T : MarkdownDoc => Load<T>(fs.GetMarkdownDoc(doc)) ?? doc;
}