using Letterbook.Docs.Containers;
using Letterbook.Docs.Files;
using Letterbook.Docs.Markdown;
using Markdig;
using ContainerExtensions = Letterbook.Docs.Markdown.ContainerExtensions;

namespace Letterbook.Docs;

public static class DependencyInjection
{
	public static IServiceCollection AddMarkdown(this IServiceCollection services)
	{
		return services.AddSingleton<LoaderFactory>()
			.AddSingleton<IMarkdownMeta, LoaderFactory>();
	}

	public static IServiceCollection AddMarkdownLoader<T>(this IServiceCollection services) where T : class, IMarkdownFiles
	{
		return services.AddTransient<T>();
	}

	public static IServiceCollection AddProjectFiles(this IServiceCollection services)
	{
		return services.AddSingleton<IProjectFiles, ProjectFiles>();
	}

	public static MarkdownPipelineBuilder AddMarkdig(this IServiceCollection services)
	{
		var builder = new MarkdownPipelineBuilder()
			.UseAutoIdentifiers()
			.UseYamlFrontMatter()
			.UseAdvancedExtensions()
			.Use(ContainerExtensions.Instance);
		services
			.AddSingleton<MarkdownPipeline>(_ => builder.Build());

		return builder;
	}

	public static MarkdownPipelineBuilder UseCustomContainers(this MarkdownPipelineBuilder pipeline,
		Action<ContainerExtensions>? configure = null)
	{
		var ext = ContainerExtensions.Instance;
		configure?.Invoke(ext);
		pipeline.Extensions.AddIfNotAlready(ext);
		return pipeline;
	}

	public static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
	{
		context.Configuration.GetSection(nameof(AppConfig)).Bind(AppConfig.Instance);
		services.AddSingleton(AppConfig.Instance);
		services.AddSingleton<RazorPagesEngine>();

		services.AddMarkdown()
			.AddMarkdownLoader<LoadDate>()
			.AddMarkdownLoader<LoadDirectory>()
			.AddMarkdownLoader<LoadCategories>()
			.AddProjectFiles();
		services.AddMarkdig()
			.UseCustomContainers(extensions =>
			{
				// extensions.AddBlockContainer("YouTube", new YouTubeContainer());
				extensions.AddBlockContainer("Mastodon", new MastodonContainer());
				extensions.AddBlockContainer("Tip", new CustomInfoRenderer());
				extensions.AddBlockContainer("Info", new CustomInfoRenderer() { Class = "info", Title = "INFO", });
				extensions.AddBlockContainer("Warning", new CustomInfoRenderer() { Class = "warning", Title = "WARNING", });
				extensions.AddBlockContainer("Danger", new CustomInfoRenderer() { Class = "danger", Title = "DANGER", });
			});
	}
}