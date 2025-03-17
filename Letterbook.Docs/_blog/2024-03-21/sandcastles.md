---
title: Sandcastles
authors:
  - jenniferplusplus
---

# Development Update

I've been working recently to finish up the initial implementation of moderation features in Letterbook. It, unfortunately, taken longer than I wanted, and I haven't had much to show or talk about in terms of features, as a result. But, that's not because I'm not doing anything! And I don't want the things I'm doing to happen behind-the-curtain, so to speak. I don't know how many people are really watching the development of Letterbook. Probably very few. But there should be something here to watch, either way. So, consider this post a sort of developer blog.

First, let me share the work that has been done on moderation features. To begin with, we conducted a [series of user experience research interviews][research] with fediverse moderators. This helped us get a better understanding of how moderators work in practice. It also taught us a lot about the state of the tools that are available, the needs those tools fill, and the unmet needs that existing tools are not currently filling. I recommend anyone who's interested in this should read the analysis I linked above.

:::
Based on that research, I started building out the features. I'm pleased to say that starting with a clear understanding of the design goals and requirements made that a much smoother process than usual. It turns out design is good and useful! Who knew!? The initial work on back end moderation features is done, from an internal feature perspective. Or at least done enough to support what I think is the minimum viable moderation work flow. Members can report posts and/or profiles that don't align with the server's values and policies. Moderators can review those reports, keep notes, manage and close them when they're done.
:::

<aside>
Who knew? I did. I knew. Probably lots of other people knew, too. I wish the tech field hadn't gutted design as a practice, and replaced it with product management. But that's a post for another day, on a different blog.
</aside>

What's not done is federating these reports to other servers. Developing that capability is unfortunately a process of trial and error. That's just the nature of the ActivityPub standard. That process is easy enough between two instances of Letterbook itself. That's just routine unit and integration testing. It takes minutes to set up, and milliseconds to execute. But it's much harder to involve other backends. And that's where [Sandcastles][sandcastles] comes in.

# Sandcastles

Sandcastles is an attempt to build a federation sandbox where multiple heterogenous fedivserse backends can run and interact in a controlled environment with production-like configuration. As a practical matter, every fediverse backend I'm aware of requires remote objects to be homed at https URLs, or on otherwise secure transport schemes that provide tamper resistance and identity verification of the host. Now, all of them can relax the requirement by enabling some kind of test, or development, or debug configurations. But those configurations can have other side effects that make it hard to be confident the behavior you get in your test setup is the same as what you'll see in the wild. Sandcastles solves that problem.

It does this by creating a network namespace where it can provide DNS resolution for the various backends you want to run. Connections to those domain names are proxied through traefik, which automatically provisions TLS certificates for those hosts, and handles TLS termination. This is generally similar to how these servers are often configured in production systems, and it's a common pattern in IT operations. However, in production systems, those TLS certificates can be issued by certificate authorities that have trust anchored by a global root certificate authority. That doesn't work in this private network namespace, where none of the traffick should ever even reach the internet, and none of the DNS host names involved would be publically resolvable. So, Sandcastles also runs a private root CA, and configures the servers it manages to trust that CA. And with that, fediverse backend servers can be run in production configurations in a local test bed, without depending on any infrastructure from the public internet. Hooray!

In order to orchestrate this private network namespace, I use docker compose. I set this all up about a year ago, and it worked ok. Not great, but well enough for my needs at the time. But then I moved my personal computing to linux, from windows. And docker updates broke some of my host networking, so I moved my container dev workflow from docker to podman. Somewhere in there I also defaulted to running rootless containers. And, Sandcastles stopped working for me. Docker is fickle, so I initially thought I would replace the containers with VMs, and use vagrant to orchestrate them. I spent some time on that, and had to give it up because it was even harder to get working, and slower to iterate. As a development tool, I consider iteration speed to be a critical feature.

[research]: /blog/post/2024/11/18/moderation-tooling-research
[sandcastles]: https://github.com/Letterbook/Sandcastles
