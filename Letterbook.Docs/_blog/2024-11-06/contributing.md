---
title: Contributing to Letterbook
authors:
  - jenniferplusplus
---

We're coming up on the end of the year, and Letterbook has come a very long way. We have architecture and data models that will carry us for a long time into the future. We have a growing set of core backend functionality. We have a logo, colors, and seeds for a visual identity and language for Letterbook as a product. We've conducted moderator experience research, and we're actively designing and building moderation and safety features based on that research. I will always want this project to move farther and faster, but this has been a good year. So let's take stock of what we have and don't have, because I think that will illuminate our most pressing needs.

| Feature                    | Backend | API | Web  | Federated |
|----------------------------|---------|-----|------|-----------|
| Signup                     | ⚠️      | ⚠️   | ⚠️   | N/A       |
| Login                      | ⚠️      | ⚠️   | ⚠️   | N/A       |
| Manage your account        | ⚠️      | ⚠️   | ⚠️   | N/A       |
| View profiles              | ✅      | ✅   | ⚠️   | ⚠️        |
| Manage your profile        | ⚠️      | ✅   | ⚠️   | ✅        |
| Follow people              | ✅      | ✅   | ⚠️   | ✅        |
| Read posts                 | ✅      | ✅   | ❌   | ✅        |
| Post things                | ✅      | ✅   | ❌   | ✅        |
| Draft posts                | ✅      | ✅   | ❌   | N/A       |
| Be followed by people      | ✅      | ✅   | ⚠️   | ✅        |
| Reply to posts             | ✅      | ✅   | ❌   | ✅        |
| Like posts                 | ✅      | ✅   | ❌   | ✅        |
| Share posts                | ✅      | ✅   | ❌   | ✅        |
| Edit posts                 | ✅      | ✅   | ❌   | ✅        |
| Delete posts               | ✅      | ✅   | ❌   | ✅        |
| Mention people             | ✅      | ✅   | ❌   | ✅        |
| Limit visibility of posts  | ✅      | ✅   | ❌   | ✅        |
| Block people               | 🔜      | ❌   | ❌   | ❌        |
| Mute people                | 🔜      | ❌   | ❌   | ❌        |
| Report things              | 🔜      | ❌   | ❌   | ❌        |
| Get notifications          | ❌      | ❌   | ❌   | N/A       |

 ✅ : Done  
 ⚠️ : Partially done  
 ❌ : Not started  
 🔜 : In progress  

So, there's a chunk of account management that's sort of usable, but not actually good. And there's a chunk of moderation/trust/safety features that don't exist yet, but I'm actively working on those right now. And then there's the whole user interface, which barely exists. We could _really_ use help in that area.

# Join In

Clearly there's a lot to do. In fact, there will always be a lot to do. I don't imagine that Letterbook is the kind of software that would ever become "finished." If you want to help, or even just to ask questions, please do. I'm going to try holding some maintainer office hours, and see how that goes. These are open to anyone who's interested, you just have to join a video call (camera optional). The first one will be on **November 17, beginning at 10am US Central Time**. There's a [public calendar][office-hours-cal] that will include this and future office hours events.

Read on for my thoughts on what contributions might looks like. But, if you think you have a better idea, come talk to me. I'll happily take working and maintainable contributions! They don't have to look exactly like what I would have done.

## Plan for the Web UI

Letterbook is meant to be easy to operate. And, to the extent possible, inexpensive to operate. We also want it to be maximally accessible to people. And, finally, we want it to be easy to maintain. In terms of the web app, I think that suggests a certain path. Specifically, that path is using conventional server-side rendering to deliver html, css, and some javascript to the browser. That means intentionally not building a SPA. In my experience, SPAs tend to be a trap. They come at a steep up front, first-load performance impact. They also create a large maintenance burden, as everyone needs to learn both the particular JS framework underlying the app, and then the app itself. Essentially, most contributors need to learn a whole new language in order to contribute. And finally, SPAs tend to become a dumping ground for complexity. Any time something is hard, handling it gets deferred until it's a problem for the client, and then the client is forced to tackle it. That makes the already complex problem of building a client for humans to use even harder.

You might see this as a constraint, and that's fair. As a constraint, I think it's actually rather interesting. There's such a thing as [enabling constraints][enabling-constraints]. They form a kind of boundary to your problem domain. If you've decided you're not going to do a thing, then you don't need to accomodate that thing. You don't need to build or design for it. You don't need to account for its side effects. We're talking about just shipping html to browsers. And if we're just shipping html to browsers, then we can explore the idea of not using javascript at all. In practice, I expect nearly all clients would enable javascript. But what if it's not necessary? Html does the things we want to do, actually. We can use html forms, buttons, and links. And they just work. So what if we target just html? We can treat javascript as a progressive enhancement. Then we can use it to replace links with infinite scroll, for instance. Or intercept form events to load a partial page update instead of a full page reload. If this sounds like "Html As The Engine Of Application State" to you, then you're on the right track.

So, this is what I'm thinking:

- ASP Razor pages, for server-side rendering
- PicoCSS, for styling
- HTMX, for handling progressive enhancement
- Other javascript as necessary, for specific scenarios

With this stack, we don't need a javascript build, or even really a bundler. Most complexity lives in the server, where we can handle it with C#. We generally will not need to coordinate changes between server and client applications. Scaling stays relatively simple, because we don't need long lived persistent connections to a server. We're likely to add those later, to support notifications, but we can approach it as another kind of progressive enhancement. And the site should be usable on old, cheap android phones and terminal browsers. I think that last part is really valuable, given the state of the global climate. And the state of browsers, for that matter. Of course, this is still up for discussion. We have only a very small set of pages right now. And most of those were scaffolded from defaults provided by ASP.net. All of it needs to change, no matter what. If you're excited to work on this, that matters a lot. The best way to make it into what you want it to be is to build it. And the best time to do that is now.

## Web Design

So, that's the stack I'm imagining. But, that doesn't tell you much about how Letterbook would look and feel. This area is even more wide open. Letterbook will fall generally into the microblogging category. And there are common patterns in that space. We'll likely have a home feed of recent posts, and that will likely be the central feature of the site. But, we also want to surface active conversations between people you follow. We'll want lists, and topic feeds, and navigation between them. It's likely we'll want something like Sharkey's/Cohost's following feeds. We definitely need thread views. We'll have search, profile views, media galleries, and on and on. All of that needs web design.

Right now we have some fonts and some colors, and that's about it. We need layouts that work across the myriad devices people actually use. We need visuals, and guides, and interaction flows. This is a huge part of the project, and you could help.

## Docs

I assume that you're reading this on our docs site. So, as you can see, it exists. Beyond that? It's not great. This is a static site, generated from markdown sources, and powered by the same stack as as I proposed above. Or as close as I could get it. It's a little bit janky, and lacks a lot of polish. But, I thought it was valuable to keep the tech stack similar. That way, working on the docs should increase people's familiarity with the application, and vice versa.

It also lacks a lot of content. The day is coming when we'll need installation, configuration, and troubleshooting guides. I wish we had better developer docs. It would be nice to extract and publish our inline xml docs, and OpenAPI specs. They exist already, they just need to be integrated here. I'm sure our development and contributing guides could use some attention. These are all very high impact contributions, that mostly would not require much or any programming expertise.


[enabling-constraints]: https://jordankaye.dev/posts/enabling-constraints/
[office-hours-cal]: https://user.fm/calendar/v1-c31799a84a233330f05e39af1c184a71/Letterbook%20Office%20Hours.ics
