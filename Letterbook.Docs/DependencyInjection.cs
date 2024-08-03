using Letterbook.Docs.Files;
using Letterbook.Docs.Markdown;
using Markdig;

namespace Letterbook.Docs;

public static class DependencyInjection
{
	public static IServiceCollection AddMarkdown<T>(this IServiceCollection services) where T : class, IMarkdownFiles
	{
		return services.AddSingleton<MarkdownFilesFactory<T>>()
			.AddSingleton<IMarkdownMeta, MarkdownFilesFactory<T>>()
			.AddTransient<T>();
	}

	public static IServiceCollection AddProjectFiles(this IServiceCollection services)
	{
		return services.AddSingleton<IProjectFiles, ProjectFiles>();
	}

	public static MarkdownPipelineBuilder AddMarkdig(this IServiceCollection services)
	{
		var builder = new MarkdownPipelineBuilder()
			.UseYamlFrontMatter()
			.UseAdvancedExtensions();
		services.AddSingleton<MarkdownPipeline>(_ => builder.Build());

		return builder;
	}
}