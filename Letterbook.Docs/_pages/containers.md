---
title: Custom Markdown Containers
---

[Custom Containers](https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/CustomContainerSpecs.md) are a 
popular method for implementing Markdown Extensions for enabling rich, wrist-friendly consistent content in your Markdown documents. 

## Built-in Containers

Most of [VitePress Containers](https://vitepress.dev/guide/markdown#custom-containers) are also implemented in Razor Press, e.g:

#### Input

```markdown
::: info
This is an info box.
:::

::: tip
This is a tip.
:::

::: warning
This is a warning.
:::

::: danger
This is a dangerous warning.
:::
```

#### Output

::: info
This is an info box.
:::

::: tip
This is a tip.
:::

::: warning
This is a warning.
:::

::: danger
This is a dangerous warning.
:::

### Custom Title

You can specify a custom title by appending the text right after the container type:

#### Input

```markdown
::: danger STOP
Danger zone, do not proceed
:::
```

#### Output

::: danger STOP
Danger zone, do not proceed
:::

### Pre

The **pre** container can be used to capture its content in a `<pre>` element instead of it's default markdown rendering:

```markdown
:::pre
...
:::
```

### copy

The **copy** container is ideal for displaying text snippets in a component that allows for easy copying:

#### Input

```markdown
:::copy
Copy Me!
:::
```

#### Output

:::copy
Copy Me!
:::

HTML or XML fragments can also be copied by escaping them first:

#### Input

```markdown
:::copy
`<PackageReference Include="ServiceStack" Version="6.*" />`
:::
```

#### Output

:::copy
`<PackageReference Include="ServiceStack" Version="6.*" />`
:::

### sh

Similarly the **sh** container is ideal for displaying and copying shell commands:

#### Input

```markdown
:::sh
npm run prerender
:::
```

#### Output

:::sh
npm run prerender
:::

## Implementing Block Containers

[Markdig Containers](https://github.com/xoofx/markdig/blob/master/src/Markdig.Tests/Specs/CustomContainerSpecs.md) are a
great way to create rich widgets that can be used directly in Markdown. 

They're useful for ensuring similar content is displayed consistently across all your documentation. A good use-case for
this could be to implement a YouTube component for standardizing how YouTube videos are displayed.

For this example we want to display a YouTube video using just its YouTube **id** and a **title** for the video which we can
capture in the Custom Container: 

```markdown
:::YouTube MRQMBrXi5Sc
Using Razor SSG to Create Websites in GitHub Codespaces
:::
```

Which we can implement with a normal Markdig `HtmlObjectRenderer<CustomContainer>`:

```csharp
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
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')"">
            </lite-youtube>
            </div>
        </div>");
    }
}
```

That should be registered in `Configure.Ssg.cs` with the name we want to use for the container:

```csharp
MarkdigConfig.Set(new MarkdigConfig
{
    ConfigureContainers = config =>
    {
        // Add Custom Block or Inline containers
        config.AddBlockContainer("YouTube", new YouTubeContainer());
    }
});
```

After which it can be used in your Markdown documentation:

#### Input

```markdown
:::YouTube MRQMBrXi5Sc
Using Razor SSG to Create Websites in GitHub Codespaces
:::
```

#### Output

:::YouTube MRQMBrXi5Sc
Using Razor SSG to Create Websites in GitHub Codespaces
:::

### Custom Attributes

Since we use `WriteAttributes(obj)` to emit any attributes we're also able to customize the widget to use a custom **id**
and classes, e.g:

#### Input

```markdown
:::YouTube MRQMBrXi5Sc {.text-indigo-600}
Using Razor SSG to Create Websites in GitHub Codespaces
:::
```

#### Output

:::YouTube MRQMBrXi5Sc {.text-indigo-600}
Using Razor SSG to Create Websites in GitHub Codespaces
:::

## Implementing Inline Containers

Custom Inline Containers are useful when you don't need a to capture a block of content, like if we just want to display
a video without a title, e.g:

```markdown
::YouTube MRQMBrXi5Sc::
```

Inline Containers can be implemented with a Markdig `HtmlObjectRenderer<CustomContainerInline>`, e.g:

```csharp
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
                style=""background-image:url('https://img.youtube.com/vi/{youtubeId}/maxresdefault.jpg')"">
            </lite-youtube>
        </div>");
    }
}
```

That can be registered in `Configure.Ssg.cs` with:

```csharp
MarkdigConfig.Set(new MarkdigConfig
{
    ConfigureContainers = config =>
    {
        // Add Custom Block or Inline containers
        config.AddInlineContainer("YouTube", new YouTubeInlineContainer());
    }
});
```

Where it can then be used in your Markdown documentation:

#### Input

```markdown
::YouTube MRQMBrXi5Sc::
```

#### Output

::YouTube MRQMBrXi5Sc::
