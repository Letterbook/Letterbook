---
title: 'Development Update: Sandcastles edition'
authors:
  - jenniferplusplus
---

# Development Update

I've been working recently to finish up the initial implementation of moderation features in Letterbook. This has honestly taken much longer than I wanted. I haven't had much to show or talk about in terms of features, as a result. But, that's not because I'm not doing anything! And I don't want the things I'm doing to happen behind-the-curtain, so to speak. I don't know how many people are really watching the development of Letterbook. Probably very few. But there should be something here to watch, either way. So, consider this post a sort of developer blog.

First, let me share the work that has been done on moderation features. To begin with, we conducted a [series of user experience research interviews][research] with fediverse moderators. This helped us get a better understanding of how moderators work in practice. It also taught us a lot about the state of the tools that are available, the needs those tools fill, and the unmet needs that existing tools are not currently filling. I recommend anyone who's interested in this should read that analysis.

Based on that research, I started building out the features. I'm pleased to say that starting with a clear understanding of the design goals and requirements made that a much smoother process than usual. It turns out design is good and useful! Who knew!? The initial work on back end moderation features is done, from an internal feature perspective. Or at least done enough to support what I think is the minimum viable moderation work flow. Members can report posts and/or profiles that don't align with the server's values and policies. Moderators can review those reports, keep notes, manage and close them when they're done.

<aside>
<details>
<summary>Who knew?</summary>
Who knew? I did. I knew. And so did most people who've done or benefited from exploratory, generative design work. I wish the tech field hadn't gutted that design practice, and replaced it with product management. But that's a post for another day, on another blog.
</details>
</aside>

What's not done is federating these reports to other servers. Developing that capability is unfortunately a process of trial and error. That's just the nature of the ActivityPub standard. That process is easy enough between two instances of Letterbook itself. In that case, it's just routine unit and integration testing. It takes minutes to set up, and milliseconds to execute. But it's much harder to involve other backends. And that's where [Sandcastles][sandcastles] comes in.

# Sandcastles

Sandcastles is an attempt to build a federation sandbox where multiple heterogenous fedivserse backends can run and interact in a controlled environment with production-like configuration. As a practical matter, every fediverse backend I'm aware of requires remote objects to be homed at https URLs, or on otherwise secure transport schemes that provide tamper resistance and identity verification of the host. Now, all of them can relax the requirement by enabling some kind of test, or development, or debug configurations. But those configurations can have other side effects that make it hard to be confident the behavior you get in your test setup is the same as what you'll see in the wild. Sandcastles solves that problem.

It does this by creating a network namespace where it can provide DNS resolution for the various backends you want to run. Connections to those domain names are proxied through traefik, which automatically provisions TLS certificates for those hosts, and handles TLS termination. This is generally similar to how these servers are often configured in production systems, and it's a common pattern in IT operations. However, in production systems, those TLS certificates can be issued by certificate authorities that have trust anchored by a global root certificate authority. That doesn't work in this private network namespace, where none of the traffick should ever even reach the internet, and none of the DNS host names involved would be publically resolvable. So, Sandcastles also runs a private root CA, and configures the servers it manages to trust that CA. And with that, fediverse backend servers can be run in production configurations in a local test bed, without depending on any infrastructure from the public internet. Hooray!

## Version 1

In order to orchestrate this private network namespace, I use docker compose. I set this all up about a year ago, and it worked ok. Not great, but well enough for my needs at the time. But then I moved my personal computing to linux, from windows. And docker updates broke some of my host networking, so I moved my container dev workflow from docker to podman. Somewhere in there I also defaulted to running rootless containers. And, Sandcastles stopped working for me. Docker is fickle, so I initially thought I would replace the containers with VMs, and use vagrant to orchestrate them. I spent some time on that, and had to give it up because it was even harder to get working, and slower to iterate. As a development tool, I consider iteration speed to be a critical feature. VMs made it even harder to orchestrate and even slower to iterate. So, I had to switch back to my original strategy of using containers.

## Second Verse, Same as the First

In every iteration of Sandcastles I've tried up to this point, the same thing has become the biggest stumbling block: bridging to a service that's running directly on the host. I want that because it supports my development workflow, and this is a development tool I'm building for myself. The problem is that it's really hard to do. All of these things are, at their core, process isolation mechanisms. That's what enables all the virtual network shenanigans that make this thing function at all. But that means breaking isolation is actually really hard. So, I decided to give up on that one feature.

This means that Sandcastles no longer attempts to support connecting to a service that's running on the host, outside the docker network namespace. Performing interoperability tests with an app under development requires containerizing the app, and running it in the docker composition along with everything else. Giving up on that requirement means it's slower to iterate on Letterbook. The trade off is that there are way fewer moving parts, and the whole set up is more reliable. I started with an iteration cycle that involves rebuilding the letterbook image whenever I want to test a change. That takes about 1.5 minutes on my laptop, which is subjectively quite a long time. I could tolerate that, if I had to, but I don't think I have to. That time is mostly taken by rebuilding the docker image all the time, which I had to do because dotnet intermingles build artifacts all over the project source workspace. I can work around that.

The first step is to collect source code into a `Source/` directory. This is kind of tedious, but doable. Both the projects and solutions all reference other projects via relative file paths. So, moving things around means hunting down and changing all the references to it in other `csproj` files. As long as I was at it, I also split out test code into a `Tests/` directory. This is even more tedious, because test code needs to reference application code that's now in the `Source/` dir. But, it makes it much easier to exclude that code from future builds. So, I say it's worth it.

The second step is to add a `Directory.Build.props` config file for the project. This provides uniform build config across the whole project, in a similar manner as `Directory.Packages.props` does for package management. The important and useful part is that it lets us set a single build output directory, and place it anywhere we like. The `dotnet run` command respects this config, which is what we need.

The final step is to set up the sandcastle component. All of those refactors make it straightforward to bind mount the project source and configs as volumes, and simply execute `dotnet run` in the container, just like you would do locally. These are the relevant bits:

```yml
services:
  letterbook:
#    <snip/>
    command: dotnet run --project Source/Letterbook/Letterbook.csproj -c Debug --launch-profile sandcastle
    volumes:
      - 'letterbook_nuget_cache:/root/.nuget/packages/'
      - 'letterbook_build_cache:/app/artifacts/'
      - '${LETTERBOOK_REPO}Source:/app/Source:z'
      - '${LETTERBOOK_REPO}Letterbook.sln:/app/Letterbook.sln:z'
      - '${LETTERBOOK_REPO}Directory.Build.props:/app/Directory.Build.props:z'
      - '${LETTERBOOK_REPO}Directory.Packages.props:/app/Directory.Packages.props:z'
```

The `letterbook_nuget_cache` and `letterbook_build_cache` volumes are additional speed enhancements, to let us preserve nuget packages and build artifacts across different container lifetimes, to speed up builds 2+. Again, just like working locally. That's important to me. I want a familiar, easy, _rapid_ development loop. I want to attach a debugger and set break points. And I want to use all my usual tools to do that. This way I can. The end result is that the cycle time between making a change and being able to exercise it the sandcastle is around 15 seconds, at least on my laptop. And that is not bad. Definitely good enough for now.

If you want to replicate this in your own Sandcastle environment, it should be straight forward. Just make sure you have a local clone of the Letterbook source, and set the `LETTERBOOK_REPO` env var to that location on your machine.

# Takeaway

So, I spent about 2 weeks working on that, which has been deeply frustrating. I wanted to make actual progress, and instead I had to go back to square one with some essential fediverse development tooling. A side goal of this whole project is to improve that situation. So, in that spirit, I've packaged up as much of the day 1 setup and day 2 operation of Sandcastles as I could into a [castle management CLI][sandcastles]. I attempted to handle all the various docker vs podman quirks that I've so far encountered, and hopefully made the process much easier and more portable to other people's machines. I hope it's useful to you.

[research]: /blog/post/2024/11/18/moderation-tooling-research
[sandcastles]: https://github.com/Letterbook/Sandcastles
