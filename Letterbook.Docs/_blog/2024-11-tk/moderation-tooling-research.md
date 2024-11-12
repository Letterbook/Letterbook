---
title: Fedi Moderation Tooling Research
authors:
  - mattly
---

# Intro
- 1❡: What We Learned
- What we want to accomplish
- Moderator tooling is generally not a priority
- General overview of who we talked to
- What I do, How I do it
- What this report is & isn't
- Glossary

# Context is Social, Context is Information
Moderation is partly a game of context-keeping, and context is primarily social: who are the people involved in a situation? What are their histories, how are they trusted, what are the social norms & rules in which a dispute are occuring? This is often easier in smaller communities, but federation brings its problems, as does an increase in instance size; keeping track of these things becomes a problem.

Context is also, often, information. _Who are the people involved?_ can become _What do we know about the people involved?_ Providing access to this kind of information is a common problem point, and one that tooling could often do a much better job of helping with: One instance admin described giving their mod team read-only access to the database along with a library of queries to get at information their server software doesn't make available. There's lots of room for improvement here!

## Reports
The main case for a moderator gathering information is in dealing with reports. Challenges in evaluating a report can stem from the server software's design, its data architecture decisions, the design of ActivityPub, differences between fedi sofrwares, and/or the distributed nature of federation. Social information plays an important role, too.

From a design & data standpoint, many questions arise that are difficult to answer: What is the history of a user who is the subject of a report -- what reports have been filed against them previously? What is the history of the user filing the report? Is this user receiving a lot of other reports at the same time? Who are the accounts doing the reporting? 

If the report creator is remote, what information can you get about them? What is the recent or overall history of report exchanges between your instance and the remote instance? It can be hard to distinguish situations where someone is a bad actor compared to someone who is under attack by sockpuppet accounts, especially as there is much less contextual information about users, such as the domains their email addresses are from, their IP ranges, etc.

Sometimes a post that is part of a report is a response to a post that's been deleted, and since the server software deletes those posts immediately, the full thread is gone. It's completely possible for a report to become "un-solveable" by any reasonable standard, but the model around ActivityPub flags don't allow for this.

Sometimes it's a permissions issue, where posts not visible to the public (either "metioned people only" or "followers only") are either the subject of a report or part of the evaluation context for one. However, some server software applies the standard visibility rules to admins, so the moderator can't see these non-public posts that are part of the larger evaluation context.

And sometimes it's the little things. For example, Mastodon doesn't make it easy to communicate directly with a user under review; an admin who wants to email a user has to click through to their profile page to get their email address.

From a protocol standpoint, the ActivityPub `Flag` activity used for reporting has loose semantic definitions for its fields, and there is little guidance on what fields are supposed to indicate. This leaves implementors to define that, which causes problems from an interoperability standpoint. The [ActivityPub Trust and Safety group](https://github.com/swicg/activitypub-trust-and-safety/) is working to define this better.

Server software treats reporting between instances differently as well, some software sends inter-instance reports straight to the remote admins, some require the local admins to forward it, and some don't let the local admins forward.

Socially, a common concern & problem is understanding the context in which a report is taking place. Language shifts between cultures, marginalized groups reclaim slurs, and it can be hard to keep track of that. What resources exist to help a moderator evaluate these sorts of things for a community they're not a part of?

# De/Federating & Mistake Proofing

Managing federation ties is another area where queries become tricky. I've heard many stories of instances defederating from each other over some spat between their admins, and their users get caught up in this when the relationships across the two servers get severed as part of the defederation. But even if someone _wanted_ to be more careful in managing their inter-server ties, it's difficult to find out how many relationships would be affected by defederating with another server or blocking a remote user. How many people on either server will be affected? Is it possible to inform them of those relationships being lost? Is it possible to restore them should the inter-server ties become restored?

A common theme in my work of this sort is [poka-yoke](https://en.wikipedia.org/wiki/Poka-yoke) or _mistake-proofing_, and there is certainly a lot of room for improvement in Fedi server software to better help admins avoid mistakes, particularly though clearly explaining what the result of an action will be. Just as with defederating, it may not be immediately obvious that "suspending" a user on a particular server software will in fact irrevocably delete that user's account and all of their content, but it's happened.

# Roles

# Operational Visibility
- operational visibility: what's happening that needs attention? actionable data vs "oh, neat?"

# Coordination
## Coordination on their own instance
### With Users
- no easy way to communicate with users directly; email, lack of up-front info
### Between Moderators
- usage in discord, wikis
- coordination across time: how can we help our future selves remember? remembering the _why_
- roles / permissions / capabilities
## Coordination between admins/mods on other instances
- reports
- fediblock; lacking the _why_
- inconsistencies between fedi software interfaces can cause problems
- server's rules are text, not data
- difficult to coordinate *data* between instances
- distributing trust is _hard_
## Coordination between an admin and automated tooling
- stuff in use: auto-image-classification tooling; discord bots
- problems/challenges; pulling from fedi-block; incremental capabilities
- examples of benefits; offsetting human costs

# Obstacles
## Protocol Issues
- many undefined behaviors, work needed to build common patterns
- AP data model is not coherent, and is centered around the *account*, which causes problems
- purist views by figureheads; specification lacking vs implementation details
## Data Retention
- a _very_ delicate & thorny issue with legal & social consequences
- GDPR is the elephant in the room, but a welcome one

# Conclusion
- where I'm going with this next
- some challenges involved