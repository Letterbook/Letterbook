using Letterbook.Docs.Markdown;
using ServiceStack.IO;

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

            services.AddMarkdown()
	            .AddMarkdownLoader<LoadDate>()
	            .AddMarkdownLoader<LoadDirectory>()
	            .AddMarkdownLoader<LoadCategories>()
				.AddProjectFiles();
            services.AddMarkdig()
	            .UseCustomContainers(extensions =>
	            {
		            extensions.AddBlockContainer("YouTube", new YouTubeContainer());
		            extensions.AddBlockContainer("Mastodon", new MastodonContainer());
		            extensions.AddInlineContainer("YouTube", new YouTubeInlineContainer());
		            extensions.AddInlineContainer("Mastodon", new MastodonInlineContainer());
		            extensions.AddBlockContainer("Tip", new CustomInfoRenderer());
		            extensions.AddBlockContainer("Info", new CustomInfoRenderer(){
			            Class = "info",
			            Title = "INFO",
		            });
		            extensions.AddBlockContainer("Warning", new CustomInfoRenderer(){
			            Class = "warning",
			            Title = "WARNING",
		            });
		            extensions.AddBlockContainer("Danger", new CustomInfoRenderer()
		            {
			            Class = "danger",
			            Title = "DANGER",
		            });
	            });
        })
        .ConfigureAppHost(
            appHost => appHost.Plugins.Add(new CleanUrlsFeature()),
            afterPluginsLoaded: appHost =>
            {
	            AppConfig.Instance.GitPagesBaseUrl = "https://github.com/Letterbook/Letterbook";
            },
            afterAppHostInit: appHost =>
            {
                // prerender with: `$ npm run prerender`
                AppTasks.Register("prerender", args =>
                {
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
