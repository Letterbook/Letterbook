---
title: Structure
---

### Markdown Feature Structure

All markdown features are effectively implemented in the same way, starting with a **_folder** for maintaining its static markdown
content, a **.cs** class to load the markdown and a **.cshtml** Razor Page to render it:

| Location                                                                                             | Description                                                           |
|------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------|
| `/_{Feature}`                                                                                        | Maintains the static markdown for the feature                         |
| `Markdown.{Feature}.cs`                                                                              | Functionality to read the feature's markdown into logical collections |
| `{Feature}.cshtml`                                                                                   | Functionality to Render the feature                                   |
| [Configure.Ssg.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Configure.Ssg.cs) | Initializes and registers the feature with ASP .NET's IOC             |

Lets see what this looks like in practice by walking through the "Pages" feature:

## Pages Feature

The pages feature simply makes all pages in the **_pages** folder, available from `/{filename}`.

Where the included pages:

```files
/_pages
  what-is-razor-press.md
  structure.md
  privacy.md    
```

Are made available from:

- [/what-is-razor-press](/what-is-razor-press)
- [/structure](/structure)
- [/privacy](/privacy)

This is primarily where most Markdown documentation will be maintained. 

### Document Collections

Folders can be used to maintain different document collections as seen in [/vue/](/vue/) and [/creatorkit/](/creatorkit/) folders: 

```files
/_pages
  /creatorkit
    about.md
    components.md
    customize.md
  /vue
    alerts.md
    autocomplete.md
    autoform.md
```

Each documentation collection needs a Razor Page to render each page in that collection, which can be configured independently
and include additional features when needed, examples of this include:

- [/Vue/Page.cshtml](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs/Pages/Vue/Page.cshtml)
- [/CreatorKit/Page.cshtml](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs/Pages/CreatorKit/Page.cshtml)

They can contain custom Razor Pages as needed, e.g. both [/vue/](/and/) and [/creatorkit/](/creatorkit/) have custom index pages:

- [/Vue/Index.cshtml](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs/Pages/Vue/Index.cshtml)
- [/CreatorKit/Index.cshtml](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs/Pages/CreatorKit/Index.cshtml)

If no custom home page is needed, a `/{slug?}` or `/{**slug}` wildcard route can be used to handle a collection's 
index and content pages, e.g:

- [/AutoQuery.cshtml](https://github.com/ServiceStack/docs.servicestack.net/blob/main/Letterbook.Docs/Pages/AutoQuery.cshtml)
- [/OrmLite.cshtml](https://github.com/ServiceStack/docs.servicestack.net/blob/main/Letterbook.Docs/Pages/OrmLite.cshtml)
- [/Redis.cshtml](https://github.com/ServiceStack/docs.servicestack.net/blob/main/Letterbook.Docs/Pages/Redis.cshtml)

Which are used to render all pages in each documentation collection:

- [docs.servicestack.net/autoquery/](https://docs.servicestack.net/autoquery/)
- [docs.servicestack.net/ormlite/](https://docs.servicestack.net/ormlite/)
- [docs.servicestack.net/redis/](https://docs.servicestack.net/redis/)

::: tip
See [Sidebars](/sidebars) for how to configure different Sidebar menus for each collection
:::

### Loading Markdown Pages

The code that loads the Pages feature markdown content is in [Markdown.Pages.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Markdown.Pages.cs),
which ultimately just loads Markdown files using the configured [Markdig](https://github.com/xoofx/markdig) pipeline that 
is made available via its `VisiblePages` property which returns all documents **during development** but hides any
**Draft** or content published at a **Future Date** from **production builds**.

## What's New Feature

The [/whatsnew](/whatsnew) page is an example of creating a custom Markdown feature to implement a portfolio or a product releases page
where a new folder is created per release, containing both release date and release or project name, with all features in that release
maintained markdown content sorted in alphabetical order:

```files
/_whatsnew
  /2023-03-08_Animaginary
    feature1.md
  /2023-03-18_OpenShuttle
    feature1.md
  /2023-03-28_Planetaria
    feature1.md    
```

What's New follows the same structure as Pages feature which is loaded in:

- [Markdown.WhatsNew.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Markdown.WhatsNew.cs)

and rendered in:
- [WhatsNew.cshtml](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Pages/WhatsNew.cshtml)

## Markdown Videos Feature

Videos is another Markdown powered feature for display collections of YouTube videos populated from a Directory of Markdown Video
pages in [/_videos](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs/_videos):

```files
/_videos
  /projects
    video1.md
    video2.md
  /vue
    video1.md
    video2.md
```

Loaded with:
 
- [Markdown.Videos.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Markdown.Videos.cs)

and Rendered with Razor Pages:

- [Shared/VideoGroup.cshtml](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Pages/Shared/VideoGroup.cshtml) - Razor Partial for displaying a Video Collection
- [Videos.cshtml](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Pages/Videos.cshtml) - Razor Page displaying multiple Video Collections

## Metadata APIs Feature

Typically a disadvantage of statically generated websites is the lack of having APIs we can call to query website data 
in a easily readable data format like JSON. However we can also easily support this by also pre-rendering static `*.json` 
data structures along with the pre-rendered website at deployment.

This capability is provided by the new [Markdown.Meta.cs](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Markdown.Meta.cs) 
feature which generates multiple projections of the Markdown metadata for each type of content added in every year, e.g:

```files
/meta
  /2021
    all.json
    posts.json
    videos.json
  /2022
    all.json
    posts.json
  /2023
    all.json
    pages.json
    posts.json
    videos.json
    whatsnew.json
  all.json
  index.json    
```

With this you can fetch the metadata of all the new **Blog Posts** added in **2023** from:

[/2023/posts.json](https://razor-ssg.web-templates.io/meta/2023/posts.json)

Or all the website content added in **2023** from:

[/2023/all.json](https://razor-ssg.web-templates.io/meta/2023/all.json)

Or **ALL** the website metadata content from:

[/all.json](https://razor-ssg.web-templates.io/meta/all.json)

This feature makes it possible to support use-cases like CreatorKit's
[Generating Newsletters](https://servicestack.net/creatorkit/portal-mailruns#generating-newsletters) feature which generates 
a Monthly Newsletter Email with all new content added within a specified period.

## General Features

Most unique markdown features are captured in their Markdown's frontmatter metadata, but in general these features
are broadly available for all features:

- **Live Reload** - Latest Markdown content is displayed during **Development**
- **Custom Layouts** - Render post in custom Razor Layout with `layout: _LayoutAlt`
- **Drafts** - Prevent posts being worked on from being published with `draft: true`
- **Future Dates** - Posts with a future date wont be published until that date
- **Order** - Specify custom ordering for a collection pages

### Initializing and Loading Markdown Features

All markdown features are initialized in the same way in [Configure.Ssg.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Configure.Ssg.cs)
where they're registered in ASP.NET Core's IOC and initialized after the App's plugins are loaded
by injecting with the App's [Virtual Files provider](https://docs.servicestack.net/virtual-file-system)
before using it to read from the directory where the markdown content for each feature is maintained:

```csharp
public class ConfigureSsg : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {
            context.Configuration.GetSection(nameof(AppConfig)).Bind(AppConfig.Instance);
            services.AddSingleton(AppConfig.Instance);
            services.AddSingleton<RazorPagesEngine>();
            services.AddSingleton<MarkdownPages>();
            services.AddSingleton<MarkdownWhatsNew>();
            services.AddSingleton<MarkdownVideos>();
            services.AddSingleton<MarkdownMeta>();
        })
        .ConfigureAppHost(afterPluginsLoaded: appHost => {
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
                }
            });

            var pages = appHost.Resolve<MarkdownPages>();
            var whatsNew = appHost.Resolve<MarkdownWhatsNew>();
            var videos = appHost.Resolve<MarkdownVideos>();
            var meta = appHost.Resolve<MarkdownMeta>();

            meta.Features = new() { pages, whatsNew, videos };
            meta.Features.ForEach(x => x.VirtualFiles = appHost.VirtualFiles);
            
            pages.LoadFrom("_pages");
            whatsNew.LoadFrom("_whatsnew");
            videos.LoadFrom("_videos");
        });
    });
    //...
}
```

These dependencies are then injected in the feature's Razor Pages to query and render the loaded markdown content.

## Custom Frontmatter

You can extend the `MarkdownFileInfo` type used to maintain the markdown content and metadata of each loaded Markdown file
by adding any additional metadata you want included as C# properties on:

```csharp
// Add additional frontmatter info to include
public class MarkdownFileInfo : MarkdownFileBase
{
}
```

Any additional properties are automatically populated using ServiceStack's
[built-in Automapping](https://docs.servicestack.net/auto-mapping) which includes rich support for converting string frontmatter
values into native .NET types.

## Updating to latest version

You can easily update all the JavaScript dependencies used in
[postinstall.js](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/postinstall.js) by running:

:::sh
npm install
:::

This will also update the Markdown features `*.cs` implementations which is delivered as source files instead of an external
NuGet package to enable full customization, easier debugging whilst supporting easy upgrades.

If you do customize any `Markdown*.cs` files, you'll want to exclude them from being updated by removing them from:

```js
const hostFiles = [
  'Markdown.Meta.cs',
  'Markdown.Pages.cs',
  'Markdown.WhatsNew.cs',
  'Markdown.Videos.cs',
  'MarkdownPagesBase.cs',
  'MarkdownTagHelper.cs',
]
```

## Markdown Tag Helper

The included [MarkdownTagHelper.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/MarkdownTagHelper.cs) can be used
in hybrid Razor Pages like [About.cshtml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Pages/About.cshtml)
to render the [/about](/about) page which uses the flexibility of Razor Pages and static content component maintained with inline Markdown.

The `<markdown />` tag helper renders plain HTML, which you can apply [Tailwind's @typography](https://tailwindcss.com/docs/typography-plugin)
styles by including **typography.css** and annotating it with your preferred `prose` variant, e.g:

```html
<link rel="stylesheet" href="/css/typography.css">
<markdown class="prose">
  Markdown content...
</markdown>
```