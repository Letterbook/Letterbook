---
title: Fedi Moderation Tooling Research
authors:
  - mattly
---

# Intro
- 1❡: What We Learned

More than ever, it's imperative to [provide safe places](https://www.wrecka.ge/safer-places-now/), and good moderation tooling can better help right-minded people. Moderation tooling hasn't been a major priority for a lot of the server softwares common in the fediverse, and that's unfortunate, if understandable; oftentimes priorities and efforts for open source projects focus on technical aspects more than interface design.

For the last ten years or so, my work has focused on interface design for highly technical and often nuanced things, across a variety of domains. I've learned a lot of things through this work, but perhaps the most important is: when you are designing for practitioners, _you need to listen to the people doing the work_. So we talked to a small handful of people doing moderation work at a variety of scales and contexts: instances large & small, people moderating discord servers, people doing trust & safety work professionaly at scale.

This article is a summary of what we've learned through our interviews, and focuses on the areas where the tooling for people doing moderation work could be improved, and what obstacles are involved to someone seeking to improve that tooling. I have some basic recommendations which people developing server software could implment to help their moderation tooling, but I'm still a ways off from having cohesive designs. We feel these learnings are worth sharing with others doing similar work in the fediverse.

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

## De/Federating, Roles, & Mistake Proofing

Managing federation ties is another area where queries become tricky. I've heard many stories of instances defederating from each other over some spat between their admins, and their users get caught up in this when the relationships across the two servers get severed as part of the defederation. But even if someone _wanted_ to be more careful in managing their inter-server ties, it's difficult to find out how many relationships would be affected by defederating with another server or blocking a remote user. How many people on either server will be affected? Is it possible to inform them of those relationships being lost? Is it possible to restore them should the inter-server ties become restored?

Many server softwares don't distinguish between admin and moderation roles, but some that do make a mess of it by eschewing role-based controls for what could best be described as "capability templates" which provide a set of actions the users they are applied to can take. Where this falls apart is that if a template is updated, it doesn't update the user accounts to which the template had been applied. 

This may seem obvious to someone who thinks about it when I describe the data model this way, but the server software doesn't describe the data model this way and most people aren't going to think about it. This results in "ability drift" unless someone were to go through and update each users' permissions, which then requires keeping track of which person is supposed to have which permission set.

To my horror, an incident was described to me whereby the sole remaining admin for an instance had managed to remove their own admin bit. I place no blame on the person who did this, rather on the design of the software. Another problem from this approach to user capabilities was an inconsistent user interface, where the moderator would be presented with affordances (buttons, links, etc) for actions which they didn't have permission to perform.

A common theme in my work of this sort is [poka-yoke](https://en.wikipedia.org/wiki/Poka-yoke) or _mistake-proofing_, and there is certainly a lot of room for improvement in Fedi server software to better help admins avoid mistakes, particularly though clearly explaining what the result of an action will be. Just as with defederating, it may not be immediately obvious that "suspending" a user on a particular server software will in fact irrevocably delete that user's account and all of their content, but it's happened. 

There's a lot of places where simple safeguards (are you sure? type the profile name to suspend) would go a long way towards preventing mistakes, and others where notes about special cases (this user is a frequent target of harassment, be super careful) would go further.

## Operational Visibility

Adjacent but related to moderation, many we talked to complained about operational visibility, dashboards, and getting a sense of "what's happening in my instance?" People who've run public-facing products at scale know how important this is, but I got the overwhelming sense that fedi server software could use improvement in this realm as well. 

The main complaint is that surfaced metrics often aren't _actionable_; for example, showing a number or chart of "new account signups" might result in an "oh, neat" or "huh, ok" response, but there's no way to dig deeper into this data. Who are these new signups? Is there any trend in weird domain names or IP blocks that the administrator should know about? How many of these accounts fill out any parts of their profile, make posts, or follow patterns indicative of spam, abuse, sockpuppeting, or becoming a sleeper account for a future harassment campaign? 

There's often no way to know any of this without resorting to complex database queries, yet this information is vital to moderators who want to take a pro-active approach to keeping their instance safe.

# Coordination

Moderation is also about working with other people: your users, your co-moderators, other instance's moderators, themselves in the future, and automated tooling. There are a lot of further problems people face in this realm, some of which are very thorny and require people working together to define standards, and others which fedi server software can work to handle on its own.

## Within Your Instance -- People

To my surprise, a common frustration is the difficulty for admin staff to communicate privately with their own users. "Direct" / "Mentioned Only" messages only work so well, and email is a more reliable method of communication, but getting at a user's email address from the context it's needed is often many clicks & page loads away. That communication then lives outside the instance's records, and unless the moderation staff have a shared email inbox, is private between the individual moderator and the user.

Moderation teams often make good use of tools outside their server software to coordinate: Wikis, chat services (often in private Discord servers), web-based database query tooling, follow-the-sun scheduling tools, incident response management tooling. Many expressed a desire for some of this within their server software itself, though it seems teams are able to find external tooling that works well enough for them.

A common frustration and major use-case for private wikis is for coordinating across time with your past & future selves: Why were decisions made? Having searchable notes with labels and bidirectional page links goes a long way in making this easier. But while tooling that helps with this sort of thing exists in some server software, it's not pervasive enough and should be applied to more things such as instance block records (more on this below).

## Within Your Instance -- Automation

Also to my surprise, the people we spoke to were overwhelmingly in favor of using automated tooling to support a proactive approach to moderation, especially in how it can offset the human cost of performing this work.

People cited existing examples such as [Akkoma's Message Rewriting Facility](https://docs.akkoma.dev/stable/configuration/mrf/), which allows instance admins to run incoming messages through both arbitrary & predefined Elixir code, tools which automatically scan incoming images for CSAM, and a near-universal love for Discord Bots.

The biggest hesitation with introducing automated tooling is with providing guardrails against misunderstood behavior (mistake-proofing), and trustworthiness of third-party code. These concerns are also present when integrating community resrouces such as shared blocklists: Are these tools going to do what I think they're going to do? Do I want to allow them to do everything they say they're going to do?

Some moderators use automated reporting, especially for the sorts of things mentioned earlier about useful dashboard information, as another tool in their pro-active moderation toolbox, but this usually involves external tooling and complex database queries.

## Coordination Between Instances



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
## Product Values
## Protocol Issues
- many undefined behaviors, work needed to build common patterns
- AP data model is not coherent, and is centered around the *account*, which causes problems
- purist views by figureheads; specification lacking vs implementation details
## Data Retention
- a _very_ delicate & thorny issue with legal & social consequences
- GDPR is the elephant in the room, but a welcome one

# Recommendations
- polymorphic linkable notes, 
- "disposition labels" over hashtags
- community collaboration is very important
- moderation plugin ecosystem

# Conclusion
- where I'm going with this next
- some challenges involved