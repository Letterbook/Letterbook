---
title: Contributing to Letterbook
authors:
  - jenniferplusplus
---

We're coming up on the end of the year, and Letterbook has come a very long way. We have architecture and data models that will carry us for a long time into the future. We have a growing set of core backend functionality. We have a logo, colors, and seeds for a visual identity and language for Letterbook as a product. We've conducted moderator experience research, and we're actively designing and building moderation and safety features based on that research. I will always want this project to move farther and faster, but this has been a good year. So let's take stock of what we have and don't have, because I think that will illuminate our most pressing needs.

| Feature                    | Backend | API | Web  | Federated |
|----------------            |---------|-----|------|-----------|
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

# Plan for the Web UI

Letterbook is meant to be easy to operate. And, to the extent possible, inexpensive to operate. We also want it to be maximally accessible to people. And, finally, we want it to be easy to maintain. In terms of the web app, I think that suggests a certain path. Specifically, that path is using conventional server-side rendering to deliver html, css, and some javascript to the browser. That means intentionally not building a SPA. In my experience, SPAs tend to be a trap. They come at a steep up front, first-load performance impact. They also create a large maintenance burden, as everyone needs to learn both the particular JS framework underlying the app, and then the app itself. Essentially, most contributors need to learn a whole new language in order to contribute. And finally, SPAs tend to become a dumping ground for complexity. Any time something is hard, handling it gets deferred until it's a problem for the client, and then the client is forced to tackle it.

You might see this as a constraint, and that's fair. As a constraint, I think it's actually rather interesting. There's such a thing as enabling constraints. They form a kind of boundary to your problem domain. If you've decided you're not going to do a thing, then you don't need to accomodate that thing. You don't need to build or design for it. You don't need to account for its side effects. We're talking about just shipping html to browsers. And if we're just shipping html to browsers, then we can explore the idea of not using javascript at all. In practice, I expect nearly all clients would enable javascript. But what if it's not necessary? Html does the things we want to do, actually. We can use html forms, buttons, and links. And they just work. So what if we target just html? We can treat javascript as a progressive enhancement. Then we can use it to replace links with infinite scroll, for instance. Or intercept form events to load a partial page update instead of a full page reload. If this sounds like "Html As The Engine Of Application State" to you, then you're on the right track.

So, this is what I'm thinking:

- ASP Razor pages, for server-side rendering
- PicoCSS, for styling
- HTMX, for handling progressive enhancement
- Other javascript as necessary, for specific scenarios

With this stack, we don't need a javascript build, or even really a bundler. Most complexity lives in the server, where we can handle it with C#. Scaling stays relatively simple, because we don't need long lived persistent connections to a server. And the site should be usable on old, cheap android phones and terminal browsers. I think that last part is really valuable, given the state of the global climate. And the state of browsers, for that matter. Of course, this is still up for discussion. We have only a very small set of pages right now. And most of those were scaffolded from defaults provided by ASP.net. All of it needs to change, no matter what. If you're excited to work on this, that matters a lot. The best way to make it into what you want it to be is to build it. And the best time to do that is now.

# Web Design

# Docs


