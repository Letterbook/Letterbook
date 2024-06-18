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
            services.AddSingleton<MarkdownWhatsNew>();
            services.AddSingleton<MarkdownVideos>();
            services.AddSingleton<MarkdownMeta>();
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
                    }
                });

                var includes = appHost.Resolve<MarkdownIncludes>();
                var pages = appHost.Resolve<MarkdownPages>();
                var whatsNew = appHost.Resolve<MarkdownWhatsNew>();
                var videos = appHost.Resolve<MarkdownVideos>();
                var meta = appHost.Resolve<MarkdownMeta>();

                meta.Features = [pages, whatsNew, videos];
                
                includes.LoadFrom("_includes");
                pages.LoadFrom("_pages");
                whatsNew.LoadFrom("_whatsnew");
                videos.LoadFrom("_videos");
                AppConfig.Instance.GitPagesBaseUrl ??= ResolveGitBlobBaseUrl(appHost.ContentRootDirectory);
            },
            afterAppHostInit: appHost =>
            {
                // prerender with: `$ npm run prerender` 
                AppTasks.Register("prerender", args =>
                {
                    appHost.Resolve<MarkdownMeta>().RenderToAsync(
                        metaDir: appHost.ContentRootDirectory.RealPath.CombineWith("wwwroot/meta"),
                        baseUrl: HtmlHelpers.ToAbsoluteContentUrl("")).GetAwaiter().GetResult();

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

// Example of implementing a custom Block Container
public class YouTubeContainer : HtmlObjectRenderer<CustomContainer>
{
    protected override void Write(HtmlRenderer renderer, CustomContainer obj)
    {
        if (obj.Arguments == null)
        {
            renderer.WriteLine($"Missing YouTube Id, Usage :::{obj.Info} <id>");
            return;
        }
        
        renderer.EnsureLine();

        var youtubeId = obj.Arguments!;
        var attrs = obj.TryGetAttributes()!;
        attrs.Classes ??= new();
        attrs.Classes.Add("not-prose text-center");
        
        renderer.Write("<div").WriteAttributes(obj).Write('>');
        renderer.WriteLine("<div class=\"text-3xl font-extrabold tracking-tight\">");
        renderer.WriteChildren(obj);
        renderer.WriteLine("</div>");
        renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <lite-youtube class=""w-full mx-4 my-4"" width=""560"" height=""315"" videoid=""{youtubeId}"" 
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')""></lite-youtube>
            </div>
        </div>");
    }
}

public class YouTubeInlineContainer : HtmlObjectRenderer<CustomContainerInline>
{
    protected override void Write(HtmlRenderer renderer, CustomContainerInline obj)
    {
        var youtubeId = obj.FirstChild is Markdig.Syntax.Inlines.LiteralInline literalInline
            ? literalInline.Content.AsSpan().RightPart(' ').ToString()
            : null;
        if (string.IsNullOrEmpty(youtubeId))
        {
            renderer.WriteLine($"Missing YouTube Id, Usage ::YouTube <id>::");
            return;
        }
        renderer.WriteLine(@$"<div class=""mt-3 flex justify-center"">
            <lite-youtube class=""w-full mx-4 my-4"" width=""560"" height=""315"" videoid=""{youtubeId}"" 
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')""></lite-youtube>
        </div>");
    }
}
