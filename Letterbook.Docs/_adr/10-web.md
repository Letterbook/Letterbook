---
title: Razor Pages
code: adr-10
order: 10
discussionUrl: https://github.com/Letterbook/Letterbook/pull/213
statusHistory:
- status: decided
  date: 2024-05-05
---

# Razor Pages

Build our own UI, using Razor Pages, instead of depending primarily on the Mastodon API.

# Decision

As the project has developed, it's started to seem more important to have a first party UI. We will still implement the Mastodon API and attempt to support Mastodon clients as well as we can. But, depending on the features that are available in Mastodon clients would be limiting, especially for admin and moderator features.

## Impact

We selected Razor Pages as the basic framework.

Razor scaffolding includes JQuery, Bootstrap, and a few other front end utilities. We're not settled on these aspects, and we would certainly consider other alternatives.

## Context

This is to summarize and record a decision that's already made. Importantly, that also includes identifying parts of the implementation that are incidental and more open to change.

## Discussion

Before choosing Razor pages, we discussed using Blazor instead. Blazor requires active websocket sessions and continuous server-side evaluation. The concern is this makes Blazor more resource intensive, and simultaneously more complicated to run and scale. And Blazor WASM has a very large download size which can be a problem on mobile networks.

- Initial PR adding the web project [#204](https://github.com/Letterbook/Letterbook/pull/204)
- PR discussing this ADR [#213](https://github.com/Letterbook/Letterbook/pull/213)
