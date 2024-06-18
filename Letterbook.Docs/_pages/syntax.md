---
title: Markdown Syntax
---

## Configuring Markdig

Razor Press uses the high-quality [Markdig](https://github.com/xoofx/markdig) CommonMark compliant implementation 
for its Markdown parsing in .NET.

Each [Markdown*.cs](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs) feature is able to customize
which [Markdig features](https://github.com/xoofx/markdig#features) it wants to use by providing a custom
`CreatePipeline()` implementation with all the Markdig extensions it needs.

Alternatively the Markdig pipeline can be globally extended for all Markdown features by adding it to `MarkdigConfig` 
pipeline in [Configure.Ssg.cs](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Configure.Ssg.cs):

```csharp
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
```

## Header Anchors

Headers, like above, automatically get anchor links applied with an **id** that's automatically generated from the 
Header text.

### Custom Anchors {#my-anchor}

To specify a custom anchor tag for a heading instead of using the auto-generated one, add a suffix to the heading:

```markdown
### Custom Anchors {#my-anchor}
```

This allows you to link to the heading as `#my-anchor` instead of the default `#custom-anchors`.

### Custom Classes 

Custom Classes can be added to headings with the suffix:

```markdown
### Custom Classes {.my-class}
### Custom Classes {#my-anchor .my-class .my-class2}
```

However to override the default [@tailwindcss/typography](https://tailwindcss.com/docs/typography-plugin) styles applied
to headings they'll need to included within a `not-prose` class which can be done with:

```markdown
:::{.not-prose}
## Custom Class {.text-5xl .font-extrabold .tracking-tight .text-indigo-600}
:::
```

Which generates the HTML:

```html
<div class="not-prose">
    <h2 id="custom-class" class="text-5xl font-extrabold tracking-tight text-indigo-600">
        Custom Class
    </h2>
</div>
```

To render it with the custom tailwind styles we want:

:::{.not-prose}
## Custom Class {.text-5xl .font-extrabold .tracking-tight .text-indigo-600}
:::

## Document Map

A Document Map is created for each Markdown document from its **Heading 2** and **Heading 3** headings, e.g: 

```markdown
## Heading 2
### Heading 3
### Heading 3a

## Heading 2a
```

Which populates the `MarkdownFileInfo.DocumentMap` collection that renders the Document Map on the right column of
each document, that's displayed in devices with larger resolutions that can fit them.

The document map also makes use of the Auto heading anchors for its navigation, that's kept updated as you scroll.

## GitHub-Style Tables

Many [GitHub Flavored Markdown](https://github.github.com/gfm/) syntax is also supported in Markdig like their ASCII
[Tables](https://github.github.com/gfm/#tables-extension-), e.g:

#### Input

```markdown
| Tables        |      Are      |  Cool |
| ------------- | :-----------: | ----: |
| col 3 is      | right-aligned | $1600 |
| col 2 is      |   centered    |   $12 |
| zebra stripes |   are neat    |    $1 |
```

#### Output

| Tables        |      Are      |  Cool |
| ------------- | :-----------: | ----: |
| col 3 is      | right-aligned | $1600 |
| col 2 is      |   centered    |   $12 |
| zebra stripes |   are neat    |    $1 |

Which can be further styled with custom classes:

#### Input

```markdown
:::{.not-prose .table .table-striped}
| Tables        |      Are      |  Cool |
| ------------- | :-----------: | ----: |
| col 3 is      | right-aligned | $1600 |
| col 2 is      |   centered    |   $12 |
| zebra stripes |   are neat    |    $1 |
:::
```

#### Output

:::{.not-prose .table .table-striped}
| Tables        |      Are      |  Cool |
| ------------- | :-----------: | ----: |
| col 3 is      | right-aligned | $1600 |
| col 2 is      |   centered    |   $12 |
| zebra stripes |   are neat    |    $1 |
:::

## Syntax Highlighting in Code Blocks

Razor Press uses [highlight.js](https://highlightjs.org) to highlight code blocks allowing you to add syntax highlighting 
using the same syntax as 
[GitHub Code Blocks](https://docs.github.com/en/get-started/writing-on-github/working-with-advanced-formatting/creating-and-highlighting-code-blocks), e.g:

#### Input

:::pre
```csharp
class A<T>
{
    public string? B { get; set; }
}
```
:::

#### Output

```csharp
class A<T>
{
    public string? B { get; set; }
}
```

#### Input

:::pre
```json
{ "A": 1, "B": true }
```
:::

#### Output

```json
{ "A": 1, "B": true }
```

## Markdown Fragments

Markdown fragments should be maintained in `_pages/_include` - a special folder rendered with
[Pages/Includes.cshtml](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/Pages/Includes.cshtml) using
an Empty Layout which can be included in other Markdown and Razor Pages or fetched on demand with Ajax
from [/includes/vue/formatters](/includes/vue/formatters).

## Includes

Markdown Fragments can be included inside other markdown documents with the `::include` inline container, e.g:

:::pre
::include vue/formatters.md::
:::

Where it will be replaced with the HTML rendered markdown contents of markdown fragments maintained in `_pages/_include`, 
which in this case embeds the rendered contents of [_include/vue/formatters.md](https://github.com/NetCoreTemplates/razor-press/blob/main/Letterbook.Docs/_include/vue/formatters.md).

### Include Markdown in Razor Pages

Markdown Fragments can be included in Razor Pages using the custom `MarkdownTagHelper.cs` `<markdown/>` tag: 

```html
<markdown include="vue/formatters.md"></markdown>
```

### Inline Markdown in Razor Pages

Alternatively markdown can be rendered inline with:

```html
<markdown>
## Using Formatters

Your App and custom templates can also utilize @servicestack/vue's
[built-in formatting functions](href="/vue/use-formatters).
</markdown>
```
