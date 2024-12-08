using Letterbook.Docs.Files;
using Letterbook.Docs.Markdown;
using Markdig;
using ContainerExtensions = Letterbook.Docs.Markdown.ContainerExtensions;

namespace Letterbook.Docs;

public static class DependencyInjection
{
	public static IServiceCollection AddMarkdown<T>(this IServiceCollection services) where T : class, IMarkdownFiles
	{
		return services.AddSingleton<LoaderFactory<T>>()
			.AddSingleton<IMarkdownMeta, LoaderFactory<T>>()
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

	public static MarkdownPipelineBuilder UseCustomContainers(this MarkdownPipelineBuilder pipeline,
		Action<ContainerExtensions>? configure = null)
	{
		var ext = new ContainerExtensions();
		configure?.Invoke(ext);
		pipeline.Extensions.AddIfNotAlready(ext);
		return pipeline;
	}
}