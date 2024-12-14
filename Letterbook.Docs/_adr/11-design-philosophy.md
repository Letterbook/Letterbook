---
title: Design Philosophy
code: adr-11
order: 11
discussionUrl: https://github.com/Letterbook/Letterbook/
statusHistory:
  - status: proposed
    date: 2024-12-13
---

# Design Philosophy

One of the goals with Letterbook is to make the fediverse a safer and more inclusive space. That goal is served both by the features we build, and also the way those features are designed. People respond to the affordances of their software. It makes some things easier or harder, and more or less visible. That's a product of the design of those features, and thoughtful design can encourage more desirable behavior.  

# Decision

As a general rule, the user interface should focus on meeting one user intention at a time. Some examples of intentions might be to see what's new, to follow an ongoing conversation, or to get better acquainted with someone else. It should be easy to shift to new intentions as they arise. But, the interface should be focused on just the one, and should not prompt or invite actions from beyond that scope. This is easier to discuss by example.

### Example: Checking the feed

Typically, people will log into apps like Letterbook to see what's new from the people they're following. In practice, this would consist of opening the default feed and scrolling through it. They'll stop to read posts that look interesting. They may like or share some of those posts. And then they'll likely scroll on. That's all normal, expected behavior, and it would look the way many other apps look: as a scrollable list of posts. This is pursuing an intention like seeing what's new. To facilitate that, we want to include context for those posts, without being distracting. And perhaps more importantly, we don't want to include affordances that invite careless action beyond that intention. So we likely wouldn't make it easy to reply from the feed, which should belong to a more conversational intention. We likely wouldn't invite people to follow or unfollow from the feed, either. Or at least we would do so with care. Managing relationships should also belong to its own intention.

Instead, we would make it easy to move to new views that are centered around those intentions. For instance, to reply to a post, you would likely open the thread and reply from there. This would give you the benefit of context for the larger conversation. To follow someone, you would open their profile page, which gives you more context in the form of their own self-description and recent posts. 

## Impact

When building or updating user interfaces, be mindful of the focus and intent that view encapsulates, and prefer not to expand or distract from that focus if possible. We should be explicit about what that focus is, in code, documentation, or both. Linking to a page where a user can take action is almost always a good idea. Including affordances to take that action in a new place calls for more consideration. We're building out some initial UI views in the near future. The next sections offer some specific guidance for those views.

### Profile View

The profile view is meant to serve the intention of getting to know someone. Obviously, it's impossible to do that in a deep way simply from reading a social media profile. So this is more about the initial, surface level familiarity you would get from simply being in the same social space. It gives the profile's owner an opportunity to intentionally craft their presentation. And it gives everyone else a chance to make decisions and act on that. The kind of decisions you might make include: choosing to follow and get to know more about them, choosing to let them follow you and potentially share some of your space, or choosing to exclude them from your space, among others.

### Feed View

It's possible we'll have multiple feeds in the future, but for now we just have the one default or home feed. So this advice applies mainly to that one feed, although it may generalize. The main feature of a feed is a scrollable list of posts, more-or-less[^1] in reverse chronological order. After you've started to follow some people, the feeds are there to serve the intention of catching up with what they're talking about. This is a kind of browsing activity. It's not unlike skimming through headlines in a newspaper. And like a newspaper, we should provide additional context for the posts in that feed. That certainly includes some biographical info about the post's author. It might also include the same info about other profiles mentioned in the post. Or highlight posts that mention the user. Posts from the same thread should be grouped together, and we should consider pulling in other posts from the thread as well.

Threads and conversations are essential units of communication on Letterbook. It would be unusual that we want to consider a single post in isolation. So, the default feed should also surface ongoing conversations that may be of interest to you. These are threads which include multiple posts from people you follow[^2]. We can explore mixing these threads into the list of posts vs creating a dedicated space in the UI. Either way, the context for conversations should include the topic of conversation (likely the OP, to begin with), and the relevant participants. So, if a conversation between Picard, Janeway, and Sisko was brought to the attention of someone who follows only Janeway and Sisko, those two are the relevant participants.

Note that the catching-up, or browsing, intention served by the feed is not the same as posting or replying. The choices you would make from a feed are whether to read more of a thread, continue browsing, or do something else. It should be easy to navigate to a view for posts or replies, but likely shouldn't be done from the feed itself.

### Thread View

Threads of posts are the basic unit of communication in Letterbook. They encapsulate all the context we can provide on a conversation. And they serve the intents of reading and conversing with each other. That context, at minimum, includes all the replies[^3] we know about (notwithstanding some safety controls, like blocks). We should also highlight the OP, and your relationship with the OP, as well as biographical information about other participants in the conversation. The structure of the conversation is also important information. We can and should explore ways to present it, but a thread is likely to actually be a tree of replies. It's important that it be clear what posts are replies, and which other posts they are in reply to. Likewise, if you decide to reply, it should be clear which post you're replying to, and you should be able to see that (whole) post, as well as the rest of the posts in the thread.

## Context

This is about designing affordances to encourage desirable behavior and discourage undesirable behavior. At some point we may need to define those two categories a little more clearly. Until then, consider [Erin Kissane's discussion on the role of affordances][affordance-loop] in the social web (and Mastodon in particular) to be essential reading. It's a good idea to also (re)read [Jenniferplusplus's original project announcement][starting-letterbook]; particularly the conclusion.

## Discussion

[^1]: We have no specific plans to have an "algorithm", in the colloquial sense of a weighted and possibly uniquely tailored sorting and recommendation system. The default feed will be reverse chronological, as far as any individual API call is concerned. But, clients can be a little more sophisticated than that. So, it's easy to imagine an infinite scrolling UI, which would periodically request more recent posts in what is otherwise a scroll backwards through history.
[^2]: This does actually constitute something more like an algorithm, although it's still very light weight and temporally situated.
[^3]: It's possible that a thread could have so many replies it's not practical to load them all in a single batch. That's fine, we can have paged replies. But, that there are more replies to be loaded is still important context.

[affordance-loop]: https://erinkissane.com/the-affordance-loop 
[starting-letterbook]: https://letterbook.com/blog/post/2023/8/16/starting-Letterbook.html