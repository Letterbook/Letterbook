---
title: Static Site Generation (SSG)
---

All features up till now describes how this template implements a Markdown powered Razor Pages .NET application, where this template
differs in its published output, where instead of a .NET App deployed to a VM or App server it generates static `*.html` files that's
bundled together with `/wwwroot` static assets in the `/dist` folder with:

:::sh
npm run prerender
:::

That can then be previewed by launching a HTTP Server from the `/dist` folder with the built-in npm script:

:::sh
npm run serve
:::

That runs **npx http-server** on `http://localhost:8080` that you can open in a browser to preview the published version of your
site as it would be when hosted on a CDN.

### Static Razor Pages

The static generation functionality works by scanning all your Razor Pages and prerendering the pages with prerendering instructions.

### Pages with Static Routes

Pages with static routes can be marked to be prerendered by annotating it with the `[RenderStatic]` attribute as done in
[About.cshtml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Pages/About.cshtml):

```csharp
@page "/about"
@attribute [RenderStatic]
```

Which saves the pre-rendered page using the pages route with a **.html** suffix, e.g: `/{@page route}.html` whilst pages with static
routes with a trailing `/` are saved to `/{@page route}/index.html`:

```csharp
@page "/vue/"
@attribute [RenderStatic]
```

#### Explicit generated paths

To keep the generated pages in-sync with using the same routes as your Razor Pages in development it's recommended to use the implied
rendered paths, but if preferred you can specify which path the page should render to instead with:

```csharp
@page "/vue/"
@attribute [RenderStatic("/vue/index.html")]
```

### Pages with Dynamic Routes

Prerendering dynamic pages follows [Next.js getStaticProps](https://nextjs.org/docs/basic-features/data-fetching/get-static-props)
convention which you can implement using `IRenderStatic<PageModel>` by returning a Page Model for each page that should be generated
as done in [Vue/Page.cshtml](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Pages/Vue/Page.cshtml) and
[Page.cshtml](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Pages/Page.cshtml):

```csharp
@page "/{slug}"
@model Letterbook.Docs.Page

@implements IRenderStatic<Letterbook.Docs.Page>
@functions {
    public List<Page> GetStaticProps(RenderContext ctx)
    {
        var markdown = ctx.Resolve<MarkdownPages>();
        return markdown.GetVisiblePages().Map(page => new Page { Slug = page.Slug! });
    }
}
...
```

In this case it returns a Page Model for every **Visible** markdown page in
[/_pages](https://github.com/NetCoreTemplates/razor-ssg/tree/main/Letterbook.Docs/_pages) that ends up rendering the following pages in `/dist`:

- `/what-is-razor-press.html`
- `/structure.html`
- `/privacy.html`

### Limitations

The primary limitations for developing statically generated Apps is that a **snapshot** of entire App is generated at deployment,
which prohibits being able to render different content **per request**, e.g. for Authenticated users which would instead require 
executing custom JavaScript after the page loads to dynamically alter the page's initial content.

Otherwise in practice you'll be able develop your Razor Pages utilizing Razor's full feature-set, the primary concessions stem
from Pages being executed in a static context which prohibits pages from returning dynamic content per request, instead any
**"different views"** should be maintained in separate pages.

#### No QueryString Params

As the generated pages should adopt the same routes as your Razor Pages you'll need to avoid relying on **?QueryString** params
and instead capture all required parameters for a page in its **@page route** as done for:

[Posts/Author.cshtml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Pages/Posts/Author.cshtml)

```csharp
@page "/posts/author/{slug}"
@model AuthorModel
@inject MarkdownBlog Blog

@implements IRenderStatic<AuthorModel>
@functions {
    public List<AuthorModel> GetStaticProps(RenderContext ctx) => ctx.Resolve<MarkdownBlog>()
        .AuthorSlugMap.Keys.Map(x => new AuthorModel { Slug = x });
}
...
```

Which lists all posts by an Author, e.g: [/posts/author/lucy-bates](https://razor-ssg.web-templates.io/posts/author/lucy-bates), 
likewise required for:

[Posts/Tagged.cshtml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Pages/Posts/Tagged.cshtml)

```csharp
@page "/posts/tagged/{slug}"
@model TaggedModel
@inject MarkdownBlog Blog

@implements IRenderStatic<TaggedModel>
@functions {
    public List<TaggedModel> GetStaticProps(RenderContext ctx) => ctx.Resolve<MarkdownBlog>()
        .TagSlugMap.Keys.Map(x => new TaggedModel { Slug = x });
}
...
```

Which lists all related posts with a specific tag, e.g: 
[/posts/tagged/markdown](https://razor-ssg.web-templates.io/posts/tagged/markdown), and for:

[Posts/Year.cshtml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/Letterbook.Docs/Pages/Posts/Year.cshtml)

```csharp
@page "/posts/year/{year}"
@model YearModel
@inject MarkdownBlog Blog

@implements IRenderStatic<YearModel>
@functions {
    public List<YearModel> GetStaticProps(RenderContext ctx) => ctx.Resolve<MarkdownBlog>()
        .VisiblePosts.Select(x => x.Date.GetValueOrDefault().Year)
            .Distinct().Map(x => new YearModel { Year = x });
}

...
```

Which lists all posts published in a specific year, e.g: 
[/posts/year/2023](https://razor-ssg.web-templates.io/posts/year/2023).

Conceivably these "different views" could've been implemented by the same page with different `?author`, `?tag` and `?year`
QueryString params, but need to instead be extracted into different pages to support its statically generated `*.html` outputs.

## Prerendering Task

The **prerender** [AppTask](https://docs.servicestack.net/app-tasks) that pre-renders the entire website is also registered in
[Configure.Ssg.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Configure.Ssg.cs):

```csharp
.ConfigureAppHost(afterAppHostInit: appHost =>
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
//...
```

Which we can see:
1. Deletes `/dist` folder
2. Copies `/wwwroot` contents into `/dist`
3. [Generates redirect](/redirects) `.html` files for all paths in `redirects.json`
4. Passes all App's Razor `*.cshtml` files to `RazorSsg` to do the pre-rendering

Where it processes all pages with `[RenderStatic]` and `IRenderStatic<PageModel>` prerendering instructions to the
specified `/dist` folder.

### Previewing prerendered site

To preview your SSG website, run the prerendered task with:

:::sh
npm run prerender
:::

Which renders your site to `/dist` which you can run a HTTP Server from with:

:::sh
npm run serve
:::

That you can preview with your browser at `http://localhost:8080`.