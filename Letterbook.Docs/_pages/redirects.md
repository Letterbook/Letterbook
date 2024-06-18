---
title: Redirects
---

## Static HTML Page Redirects

The [redirects.json](https://github.com/NetCoreTemplates/razor-press/tree/main/Letterbook.Docs/redirects.json) file allows you to
define a map of routes with what routes they should redirect to, e.g:

```json
{
    "/creatorkit": "/creatorkit/",
    "/vue": "/vue/"
}
```

When prerendering this will generate a `*.html` page for each mapping containing a 
[meta refresh](https://www.w3.org/TR/WCAG20-TECHS/H76.html) to perform a client-side redirect to the new route which will
work in all static file hosts and CDNs like GitHub Pages CDN.

Where it will redirect invalid routes like [/vue](https://razor-press.web-templates.io/vue) and
[/creatorkit](https://razor-press.web-templates.io/creatorkit) to their appropriate `/vue/` and `/creatorkit/` paths.

## Redirects in AWS S3

Alternatively if you deploy your static site to a smarter static file host like an AWS S3 bucket you can perform these
redirects on the server by defining them in 
[Custom Redirection Rules](https://docs.aws.amazon.com/AmazonS3/latest/userguide/how-to-page-redirect.html).
