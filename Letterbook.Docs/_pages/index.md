---
title: Letterbook Project
---

Letterbook is a federated microblogging service, implementing ActivityPub. The goal for the project is to make hosting a fediverse server a better and more sustainable experience. We also want to make it a safer and more inclusive space for the people who make the fediverse their social media home. Those are big aspirations, but we have some solid plans about how to start, and we would love more input about where to go from there.

## Features

Letterbook is in early development, so we don't have an exhaustive list of features. But plan to support many of the features that are already common in the fediverse. You'll be able to post, edit your posts, use hashtags and custom emojis, and migrate accounts, for example. Letterbook is essentially a microblogging platform with a focus on conversations between peers.

<details>
  <summary role="button">For Admins and Ops</summary>

::: Info
[Discussion issue for Admin features](https://github.com/Letterbook/Letterbook/issues/131)
:::

#### Easy setup for new instances
Letterbook initially deploys as a single executable with simple load-balanced scaling. Aside from ancillary services like object storage and email, 1 server and 1 database is all you need to get up and running. And if you do see huge scale in your future, you can still scale outward to distributed task workers and microservices.

#### Lower cost and complexity
Letterbook doesn't maintain any live state. Everything lives in the database, so there's no need to run a Redis cluster or similar to act as shared state storage. We also expect to have significantly lower compute demands, due to both the architectural choices to avoid expensive infrastructure and system sprawl, and the use of C#, a very high performance compiled language.

#### First class observability
Letterbook is thoroughly instrumented for both automatic and custom telemetry, including robust logging, metrics, and distributed tracing. We also provide out-of-the-box collection and dashboards for our telemetry. You can investigate errors, bugs, and performance issues the same way we the developers would.
</details>

<details>
  <summary role="button">For Moderators</summary>

::: Info
[Discussion issue for Moderator features](https://github.com/Letterbook/Letterbook/issues/132)
:::

#### Local-only posts
Avoid context collapse! You can talk to your users and they can talk to each other, without risking context collapse by exposure to the whole fediverse.

#### Automatically expiring actions
Moderator actions like blocking, muting, and limiting federation will all be able to automatically expire after a set time.

#### Fine grained federation controls
You'll be able to do things like prevent federated posts from appearing in promoted feeds, prevent them from appearing at all without an established follow relationship, hide posts behind a click-through and warning, and defederate without breaking your users follow relationships, in addition to the same basic options as other servers.

#### Other moderator tools
- Keep and share notes
- Audit logs
- Auditable privileged views of non-public posts
- Spam and quality filters
- And more
</details>

<details>
  <summary role="button">For Members</summary>

::: Info
[Discussion issue for general features](https://github.com/Letterbook/Letterbook/issues/133)
:::

#### Frequently requested features
- Quote replies
- Collapse notifications
- Block, limit, and remove replies to your posts
- Propose and accept edits to alt text and content warnings
- Compose multi-post threads
- Save drafts and scheduled posts
- Formatted posts
- Emoji reactions

#### Mastodon apps
We intend to implement the Mastodon API, which will provide support for many existing Mastodon apps. Over time, we expect our features will grow well beyond what Mastodon supports, of course. But until we do, or if those features don't interest you, your current favorite app will still be there for you.

#### Better discoverability
Letterbook will have features like topic detection and topic based feeds. Follow recommendations will also consider topics you express an interest in and friends-of-friends relationships.

#### More sophisticated authoring and following options
We hope to support long form, multi-page posts. We'll also be able to create multiple promoted feeds for your own posts, and have the ability to follow those feeds specifically. If for some strange reason people want to follow your analysis of CVEs and not your fursuit friday posts, that's a doable thing. And you'll eventually be able to co-author posts with other people.

#### Real DMs
We plan to provide a real direct message experience by implementing an XMPP server. If you previously used Jabber, then it's likely that your favorite chat client is ready and waiting for you, better than ever. And if you never stopped, then you probably know that better than we do, and hopefully this is good news for you. You will of course be able to send and receive posts with restricted visibility, just like you do now, so you won't lose access to Mastodon-style direct messages with your contacts.

</details>

## Background

All of this is just the beginning of what we have planned for Letterbook. We're very excited about the future, and we get to build it together. If you'd like to know  more about the thought process that went into starting the project, [you can read the blog post](https://jenniferplusplus.com/letterbook/).


## Contributing

::: Tip
To get started developing, see the Quckstart section in the [Contributing Guide](https://github.com/Letterbook/Letterbook/blob/main/CONTRIBUTING.md)
:::

Please reach out to let us know you're interested in the project! There are many ways to contribute, it's a lot more than just code. Experience in these areas would be particularly helpful, right now. And this list will only grow over time.

1. User experience research
2. Trust and safety advice
3. Visual design
4. Web UI code
5. Backend C# code
6. Project management
7. Technical communication
