using System.Diagnostics.CodeAnalysis;
using Markdig;

namespace Letterbook.Docs;

using System.Globalization;
using ServiceStack.IO;

/// <summary>
/// A MarkdownFiles loader for markdown that is organized in chronological subdirectories.
/// <remarks>The subdirectories should be named using the yyyy-MM-dd format</remarks>
/// </summary>
/// <param name="log"></param>
/// <param name="env"></param>
/// <param name="fs"></param>
public class MarkdownChrono(ILogger<MarkdownChrono> log, IWebHostEnvironment env, IVirtualFiles fs)
	: MarkdownPagesBase<MarkdownFileInfo>(log, env, fs)
{
	public override string Id => "blog";
	public List<MarkdownFileInfo> Posts { get; set; } = new();

	public override void LoadFrom(string fromDirectory)
	{
		Posts.Clear();
		var dirs = VirtualFiles.GetDirectory(fromDirectory).GetDirectories().ToList();
		log.LogInformation("Found {Count} directories in {FromDir}", dirs.Count, fromDirectory);

		var pipeline = CreatePipeline();

		foreach (var dir in dirs)
		{
			if (!DateTime.TryParseExact(dir.Name, "yyyy-MM-dd", CultureInfo.InvariantCulture,
				    DateTimeStyles.AdjustToUniversal, out var date))
			{
				log.LogWarning("Could not parse date '{DatePart}', ignoring...", dir.Name);
				continue;
			}

			foreach (var file in dir.GetFiles().OrderBy(x => x.Name))
			{
				try
				{
					var doc = Load(file.VirtualPath, pipeline);
					if (doc == null)
						continue;

					Posts.Add(doc);
				}
				catch (Exception e)
				{
					log.LogError(e, "Couldn't load {VirtualPath}: {Message}", file.VirtualPath, e.Message);
				}
			}
		}
	}

	public override MarkdownFileInfo? Load(string path, MarkdownPipeline? pipeline = null)
	{
		if (!TryGetDate(out var date))
			return default;

		var doc = base.Load(path, pipeline);
		if (doc == null) return doc;
		doc.Date = date;

		return doc;

		bool TryGetDate([NotNullWhen(true)] out DateTime? dt)
		{
			dt = null;
			var parts = path.Split('/');
			if (parts.Length != 3) return false;

			var found = DateTime.TryParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
				out var d);
			dt = d;
			return found;
		}
	}

	public override List<MarkdownFileBase> GetAll() => Posts.Where(IsVisible).Select(Doc).ToList();

	public MarkdownFileInfo? GetByDate(DateTime date, string slug)
	{
		return Fresh(Posts.Where(IsVisible).Where(x => x.Date == date).FirstOrDefault(x => x.Slug == slug.Trim('/')));
	}

	private MarkdownFileBase Doc(MarkdownFileInfo info) =>
		ToMetaDoc(info, x => x.Content = MarkdownExtensions.StripFrontmatter(info.Content));
}