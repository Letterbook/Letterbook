---
title: Sidebars
---

## Main Sidebar

The sidebar defines the main navigation for your documentation, you can configure the sidebar menu in `_pages/sidebar.json`
which adopts the same structure as [VitePress Sidebars](https://vitepress.dev/reference/default-theme-sidebar#sidebar), e.g:

```json
[
  {
    "text": "Introduction",
    "link": "/",
    "children": [
      {
        "text": "What is Razor Press?",
        "link": "/what-is-razor-press"
      },
      {
        "text": "Structure",
        "link": "/structure"
      }
    ]
  },
  {
    "text": "Markdown",
    "children": [
      {
        "text": "Sidebars",
        "link": "/sidebars"
      }
    ]
  }
]
```

## Navigation Headings

Primary navigation headings can optionally have `"links"` to make them linkable and `"icon"` to render them
with a custom icon, e.g:

```json
{
    "icon": "<svg xmlns='http://www.w3.org/2000/svg'>....</svg>", 
    "text": "Markdown",
    "link": "/markdown/",
    "children": [
    ]
}
```

## Documentation Group Sidebars

If your happy to use the same document page title for its menu item label, you can use an implicitly generated Sidebar 
navigation like [/creatorkit/](/creatorkit/about) uses for its Sidebar navigation which can be ordered with the **order**
index defined in its frontmatter, e.g:

```yaml
title: About
order: 1
```

Which can also be grouped into different navigation sections using the **group** frontmatter, e.g:

```yaml
title: Overview
order: 6
group: Portal
```

### Custom Sidebars

For more flexibility a custom sidebar can be defined for each group by defining a `sidebar.json` in its folder 
`_pages/<group>/sidebar.json` which [/vue/](/vue/install) uses for its explicit Sidebar Navigation, e.g:

```json
[
  {
    "text": "Vue",
    "link": "/vue",
    "children": [
      {
        "text": "Install",
        "link": "/vue/install"
      }
    ]
  },
  {
    "text": "Component Gallery",
    "children": [
      {
        "text": "AutoQueryGrid",
        "link": "/vue/autoquerygrid"
      }
    ]
  }
]
```
