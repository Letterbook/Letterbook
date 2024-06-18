---
title: Deployments
---

## Enable GitHub Pages

The included [build.yml](https://github.com/NetCoreTemplates/razor-ssg/blob/main/.github/workflows/build.yml) GitHub Action
takes care of running the prerendered task and deploying it to your Repo's GitHub Pages where you'll need to enable 
GitHub Pages to be hosted from your **gh-pages** branch in your Repository settings:

![](/img/posts/razor-ssg/gh-pages.png)

## Register Custom Domains

You should then register a [Custom domain for GitHub Pages](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site/about-custom-domains-and-github-pages)
by registering a `CNAME` DNS entry for your preferred Custom Domain, e.g:

| Record | Type | Value | TTL|
| - | - | - | - |
| **mydomain.org** | CNAME | **org_name**.github.io | 3600 |

That you can either [configure in your Repo settings](https://docs.github.com/en/pages/configuring-a-custom-domain-for-your-github-pages-site/managing-a-custom-domain-for-your-github-pages-site#configuring-a-subdomain)
or if you prefer to maintain it with your code-base, save the domain name to `/wwwroot/CNAME`, e.g:

```
www.mydomain.org
```