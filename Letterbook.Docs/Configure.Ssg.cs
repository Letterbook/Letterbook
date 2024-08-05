using System.Reflection;
using Letterbook.Docs.Markdown;
using Markdig.Extensions.CustomContainers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceStack.IO;
using ServiceStack.Text;

[assembly: HostingStartup(typeof(Letterbook.Docs.ConfigureSsg))]

namespace Letterbook.Docs;

public class ConfigureSsg : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context,services) =>
        {
            context.Configuration.GetSection(nameof(AppConfig)).Bind(AppConfig.Instance);
            services.AddSingleton(AppConfig.Instance);
            services.AddSingleton<RazorPagesEngine>();
            services.AddSingleton<MarkdownIncludes>();
            services.AddSingleton<MarkdownPages>();
            services.AddSingleton<MarkdownVideos>();
            services.AddSingleton<MarkdownMeta>();

            services.AddMarkdown<MarkdownDate>()
	            .AddMarkdown<MarkdownCategories>()
				.AddProjectFiles();
            services.AddMarkdig()
	            .UseCustomContainers(extensions =>
	            {
		            extensions.AddBlockContainer("YouTube", new YouTubeContainer());
		            extensions.AddBlockContainer("Mastodon", new MastodonContainer());
		            extensions.AddInlineContainer("YouTube", new YouTubeInlineContainer());
		            extensions.AddInlineContainer("Mastodon", new MastodonInlineContainer());
	            });
        })
        .ConfigureAppHost(
            appHost => appHost.Plugins.Add(new CleanUrlsFeature()),
            afterPluginsLoaded: appHost =>
            {
                MarkdigConfig.Set(new MarkdigConfig
                {
                    ConfigurePipeline = pipeline =>
                    {
                        // Extend Markdig Pipeline
                    },
                    ConfigureContainers = config =>
                    {
                        config.AddBuiltInContainers();
                        // Add Custom Block or Inline containers
                        config.AddBlockContainer("YouTube", new YouTubeContainer());
                        config.AddInlineContainer("YouTube", new YouTubeInlineContainer());
                        config.AddBlockContainer("Mastodon", new MastodonContainer());
                        config.AddInlineContainer("Mastodon", new MastodonInlineContainer());
                    }
                });

                var meta = appHost.Resolve<MarkdownMeta>();
                var includes = appHost.Resolve<MarkdownIncludes>();
                var pages = appHost.Resolve<MarkdownPages>();
                var videos = appHost.Resolve<MarkdownVideos>();

                meta.Features = [pages, videos];

                includes.LoadFrom("_includes");
                pages.LoadFrom("_pages");
                videos.LoadFrom("_videos");
                AppConfig.Instance.GitPagesBaseUrl ??= ResolveGitBlobBaseUrl(appHost.ContentRootDirectory);
            },
            afterAppHostInit: appHost =>
            {
                // prerender with: `$ npm run prerender`
                AppTasks.Register("prerender", args =>
                {
	                // TODO: Broken meta
	                // As far as I can tell, what this does is build a collection of metadata from the site's markdown source
	                // That might be nice to have, if we can get it working
                    // appHost.Resolve<MarkdownMeta>().RenderToAsync(
                    //     metaDir: appHost.ContentRootDirectory.RealPath.CombineWith("wwwroot/meta"),
                    //     baseUrl: HtmlHelpers.ToAbsoluteContentUrl("")).GetAwaiter().GetResult();

                    var distDir = appHost.ContentRootDirectory.RealPath.CombineWith("dist");
                    if (Directory.Exists(distDir))
                        FileSystemVirtualFiles.DeleteDirectory(distDir);
                    FileSystemVirtualFiles.CopyAll(
                        new DirectoryInfo(appHost.ContentRootDirectory.RealPath.CombineWith("wwwroot")),
                        new DirectoryInfo(distDir));

                    // Render .html redirect files
                    RazorSsg.PrerenderRedirectsAsync(appHost.ContentRootDirectory.GetFile("redirects.json"), distDir)
                        .GetAwaiter().GetResult();

                    var razorFiles = appHost.VirtualFiles.GetAllMatchingFiles("*.cshtml");
                    RazorSsg.PrerenderAsync(appHost, razorFiles, distDir).GetAwaiter().GetResult();
                });
            });

    private string? ResolveGitBlobBaseUrl(IVirtualDirectory contentDir)
    {
        var srcDir = new DirectoryInfo(contentDir.RealPath);
        var gitConfig = new FileInfo(Path.Combine(srcDir.Parent!.FullName, ".git", "config"));
        if (gitConfig.Exists)
        {
            var txt = gitConfig.ReadAllText();
            var pos = txt.IndexOf("url = ", StringComparison.Ordinal);
            if (pos >= 0)
            {
                var url = txt[(pos + "url = ".Length)..].LeftPart(".git").LeftPart('\n').Trim();
                var gitBaseUrl = url.CombineWith($"blob/main/{srcDir.Name}");
                return gitBaseUrl;
            }
        }
        return null;
    }
}

public class AppConfig
{
    public static AppConfig Instance { get; } = new();
    public string LocalBaseUrl { get; set; }
    public string PublicBaseUrl { get; set; }
    public string? GitPagesBaseUrl { get; set; }
}

// Add additional frontmatter info to include
public class MarkdownFileInfo : MarkdownFileBase
{
}

public static class HtmlHelpers
{
    public static string ToAbsoluteContentUrl(string? relativePath) => HostContext.DebugMode
        ? AppConfig.Instance.LocalBaseUrl.CombineWith(relativePath)
        : AppConfig.Instance.PublicBaseUrl.CombineWith(relativePath);
    public static string ToAbsoluteApiUrl(string? relativePath) => HostContext.DebugMode
        ? AppConfig.Instance.LocalBaseUrl.CombineWith(relativePath)
        : AppConfig.Instance.PublicBaseUrl.CombineWith(relativePath);


    public static string ContentUrl(this IHtmlHelper html, string? relativePath) => ToAbsoluteContentUrl(relativePath);
    public static string ApiUrl(this IHtmlHelper html, string? relativePath) => ToAbsoluteApiUrl(relativePath);
}