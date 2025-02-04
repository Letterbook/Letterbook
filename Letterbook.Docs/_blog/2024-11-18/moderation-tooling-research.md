---
title: 'Research: Moderator Tools User Experience'
authors:
  - mattly
---

# Moderator Tools User Experience
More than ever, it's imperative to [provide safe places](https://www.wrecka.ge/safer-places-now/). Moderation tooling hasn't been a major priority for a lot of the server softwares common in the fediverse, and that's unfortunate, if understandable -- oftentimes priorities and efforts for open source projects focus on technical aspects more than interface design.

For the last ten years or so, my work has focused on interface design for highly technical and often nuanced things, across a variety of domains (and I often have to remind people what the "I" is in API). I've learned a lot of things through this work, but perhaps the most important is: when you are designing for practitioners, _you need to listen to the people doing the work_. We talked to a small handful of people doing moderation work at a variety of scales and contexts: instances large & small, people moderating discord servers, people doing trust & safety work professionaly at scale. We talked about trust & safety as it relates to administering and moderating servers; while there is much worthwhile work which could empower individual users in these regards, our conversations focused on the admin/moderation roles.

This article is a summary of what we've learned through our interviews: the problems faced by people doing moderation work, and the obstacles to solving those problems. I also have some recommendations for technically-minded people who want to work to improve trust & safety tooling in the fediverse.

# Problems: Keeping Context

Moderation is partly a game of context-keeping, and context is primarily social: who are the people involved in a situation? What are their histories, how are they trusted, what are the social norms & rules in which a dispute are occuring? This is often easier in smaller communities, but federation brings its problems, as does an increase in instance size; keeping track of these things becomes a problem.

Context is also, often, information. _Who are the people involved?_ can become _What do we know about the people involved?_ Providing access to this kind of information is a common frustration point, and it's where I see the biggest opportunities for better-designed tooling to make access to relevant information eaiser, or even possible.  One instance admin described giving their mod team read-only access to the database along with a library of queries to get at information their server software doesn't make available. There's lots of room for improvement here!

## Flag Activities & Reports

The main case for a moderator gathering information is in dealing with flag activities, also known as reports. [Hachyderm did a great write-up on how these work](https://community.hachyderm.io/blog/2024/09/01/hachyderms-introduction-to-mastodon-moderation-the-report-feature-and-moderator-actions/). Challenges in evaluating a report can stem from the server software's design, its data architecture decisions, the design of ActivityPub, differences between fedi sofrwares, and/or the distributed nature of federation. Social information plays an important role, too.

From a design & data standpoint, many questions arise that are difficult to answer: What is the history of a user who is the subject of a report -- what reports have been filed against them previously? What is the history of the user filing the report? Is this user receiving a lot of other reports at the same time? Who are the accounts doing the reporting? 

If the report creator is remote, what information can you get about them? What is the recent or overall history of report exchanges between your instance and the remote instance? It can be hard to distinguish situations where someone is a bad actor compared to someone who is under attack by sockpuppet accounts, especially as there is much less contextual information about users, such as the domains their email addresses are from, their IP ranges, etc.

Sometimes a post that is part of a report is a response to a post that's been deleted, and since the server software deletes those posts immediately, the full thread is gone. It's completely possible for a report to become "un-solveable" by any reasonable standard, but the model around ActivityPub flags don't allow for this.

And sometimes it's a permissions issue, where posts not visible to the public (either "mentioned people only" or "followers only") are either the subject of a report or part of the evaluation context for one. However, some server software applies the standard visibility rules to admins, so the moderator can't see these non-public posts that are part of the larger evaluation context.

From a protocol standpoint, the ActivityPub `Flag` activity used for reporting has loose semantic definitions for its fields, and there is little guidance on what fields are supposed to indicate. This leaves implementors to define that, which causes problems from an interoperability standpoint. The [ActivityPub Trust and Safety group](https://github.com/swicg/activitypub-trust-and-safety/) is working to define this better.

Server software treats reporting between instances differently as well, some software sends inter-instance reports straight to the remote admins, some require the local admins to forward it, and some don't let the local admins forward.

Socially, a common concern & problem is understanding the context in which a report is taking place. Language shifts between cultures, marginalized groups reclaim slurs, and it can be hard to keep track of that. What resources exist to help a moderator evaluate these sorts of things for a community they're not a part of?

## De/Federating, Roles, & Mistake Proofing

Managing federation ties is another area where queries become tricky. I've heard many stories of instances defederating from each other over some spat between their admins, and their users get caught up in this when the relationships across the two servers get severed as part of the defederation. But even if someone _wanted_ to be more careful in managing their inter-server ties, it's difficult to find out how many relationships would be affected by defederating with another server or blocking a remote user. How many people on either server will be affected? Is it possible to inform them of those relationships being lost? Is it possible to restore them should the inter-server ties become restored?

Many server softwares don't distinguish between admin and moderation roles, but some that do make a mess of it by eschewing role-based controls for what I will describe as "capability templates" which provide a set of actions the users they are applied to can take. Where this falls apart is that if a template is updated, it doesn't update the user accounts to which the template had been applied, so their moderator team has different permissions than what the admin thinks they have.

This approach causes problems: One admin described an incident where the sole remaining admin for an instance had managed to remove their own admin bit. I place no blame on the person who did this, rather on the design of the software. It also resulted in an inconsistent user interface, where the moderator would be presented with affordances (buttons, links, etc) for actions which they didn't have permission to perform.

A common theme in my work of this sort is [poka-yoke](https://en.wikipedia.org/wiki/Poka-yoke) or _mistake-proofing_, and there is certainly a lot of room for improvement in Fedi server software to better help admins avoid mistakes, particularly though clearly explaining what the result of an action will be. Just as with defederating, it may not be immediately obvious that "suspending" a user on a particular server software will actually due to how that server software behaves, irrevocably delete that user's account and all of their content.

There's a lot of places where simple safeguards (are you sure? type the profile name to suspend) would go a long way towards preventing mistakes, and others where notes about special cases (this user is a frequent target of harassment, be super careful) would go further.

## Community Awareness

Adjacent but related to moderation, many we talked to wanted to get a better sense of "what's happening with the accounts in my instance?" People who've run public-facing products at scale know how important this is, but I got the overwhelming sense that fedi server software could use improvement in this realm as well. 

The main complaint is that surfaced metrics often aren't _actionable_; for example, showing a number or chart of "new account signups" might result in an "oh, neat" or "huh, ok" response, but there's no way to dig deeper into this data. Who are these new signups? Is there any trend in weird domain names or IP blocks that the administrator should know about? How many of these accounts fill out any parts of their profile, make posts, or follow patterns indicative of spam, abuse, sockpuppeting, or becoming a sleeper account for a future harassment campaign? 

There's often no way to know any of this without resorting to complex database queries, yet this sort of information is vital to moderators who want to take a pro-active approach to keeping their instance safe.

# Problems: Coordination

Moderation is also about working with other people: your users, your co-moderators, other instance's moderators, themselves in the future, and automated tooling. There are a lot of further problems people face in this realm, some of which are very thorny and require people working together to define standards, and others which fedi server software can work to handle on its own.

## Within Your Instance -- People

To my surprise, a common frustration is the difficulty for admin staff to communicate privately with their own users. "Direct" / "Mentioned Only" messages only work so well, and email is a more reliable method of communication, but getting at a user's email address from the context it's needed is often many clicks & page loads away. That communication then lives outside the instance's records, and unless the moderation staff have a shared email inbox, is private between the individual moderator and the user.

Moderation teams often make good use of tools outside their server software to coordinate: Wikis, chat services (often private Discord servers), spreadsheets, web-based database query tooling, follow-the-sun scheduling tools, and incident response management tooling. Many expressed a desire for some of this within their server software itself, though it seems teams are able to find external tooling that works well enough for them.

A common frustration and major use-case for private wikis is for coordinating across time with your past & future selves: Why were decisions made? Having searchable notes with labels and bidirectional page links goes a long way in making this easier. But while tooling that helps with this sort of thing exists in some server software, it's not pervasive enough and should be applied to more things such as instance block records (more on this below).

## Within Your Instance -- Automation & Shared Resources

Also to my surprise, the people we spoke to were enthusiastic about using automated tooling to support a proactive approach to moderation, especially in how it can offset the human cost of performing this work, citing  examples such as [Akkoma's Message Rewriting Facility](https://docs.akkoma.dev/stable/configuration/mrf/), which allows instance admins to run incoming messages through both arbitrary & predefined Elixir code, tools which [automatically classify incoming content](https://about.iftas.org/activities/moderation-as-a-service/content-classification-service/), and a love for Discord's Bots.

The biggest hesitation with introducing automated tooling is developing trust with third party code and integrating community resources like blocklists, both from a "is this code trustworthy?" standpoint as well as "will this do what I think it's going to do?"

Some moderators use external tooling for automated reporting of the sorts I mentioned previously about community awareness, but this typically requires great technical knowledge to setup the services and develop the right queries.

## Coordination Between Instances

Not surprisingly, coordination between instances yields some of the biggest and thorniest problems with moderation and admin work in the fediverse. Some of these are due to the nature of federation: lack of visible info for remote accounts, the broad set of social contexts and norms that exist, the variety of server software in use, and the clash of egos, budgets, and governance philosophies that co-exist. 

These problems aren't intractable -- many require collaboration towards standards, but not without careful tiptoeing around potential legal issues. The short of these problems is that distributed trust is a hard problem.

For reporting, a common complaint is about the technical difficulty in sharing data relevant to a report between instances, even when all parties are willing, which also gets back to the poorly-defined data structure that makes for a report to begin with. Data-sharing sometimes involves legal issues as well.

Different server instances also have different rules, but these are expressed as freeform text, as opposed to any kind of semantic labels. In addition to making it more difficult for someone evaluating "which server should I join?" because they have to read through all the various texts, it makes it more difficult and laborious for a moderator evaluating a report or determining if they should defederate to understand the alignment of rules between servers. Machine-parseable community standard labels for rules such as "Don't be an asshole", "Don't deadname people" or the like would go a very long way in reducing the human cost of moderation, but these sorts of things require social and technical consensus.

Legal issues can extend into published resources such as blocklists; one of the reasons why it would be useful to have labels & notes on blocklist entries even though you might have public-facing information in #fediblock or elsewhere is it gets into libel & liability. If you're blocking a server for rampant illegal content such as CSAM, saying so publicly exposes you to legal action. While you might be able to prove this in court, doing so is a drain on time, money, and emotions, so it's easier to keep that note private.

Finally, most moderators are volunteers. They tend to get little or no training, and have limited time available. They can't be expected to become experts in their software. Getting assistance from peers is complicated by differences in how server software's interfaces work. If you're trying to help another moderator out, you can't assume they're using the same software as you, or even the same *version* of software as you. Their interface might be different, and this poses a communication challenge.

# Obstacles

There are a handful of obstacles to improving moderation tooling, but the biggest one is getting people with influence to prioritize moderation tools. Large technology projects require collaboration and vision. In private companies, this typically rests in the hands of someone with a "Product" title, who acts as the arbiter of values that go into making the software what it is. Open source projects tend to instead have core maintiners who spearheaded a project's technical foundation and steer the project's direction and values as it gains in popularity.

In talking with people doing moderation work, I got an overwhelming sense of frustration that people with influence over the software and protocols haven't prioritized this work. If we are to provide a safe federated space equivalent to popular commercial platforms, we need the people involved in both the protocol and server software to pay attention.

Improving the protocol is a pressing challenge -- the fediverse is not *just* Mastodon, and ultimately any initiative to improve trust and safety between server instances will require improvements and clarifications to the protocol so the plethora of server softwares people use can continue to interoperate. This sort of collaboration and consensus-building takes time and a variety of perspectives.

I was also surprised to find that data retention policy poses a set of obstacles to moderation work in the fediverse. Federating posts over ActivityPub is, to some degree, a data processing arrangement, but without any kind of formal agreements in place. Retention is a very delicate and thorny issue with a lot of legal, social, and ethical consequences; one that many volunteer or amateur administrators are not ready to deal with. The GDPR is the elephant in the room, but it can also provide some guidance on good defaults for server software to set for its retention policies.

# Recommendations

Based on these conversations, I can make a handful of recommendations to developers who want to help improve trust & safety in the fediverse:

1. Get involved in improving the protocol. I get it -- consensus-building is hard work, and talking with people isn't as fun as writing code. It is important work, it is important to push back against people with influence who don't value this work, and I believe it will yield the best long-term results. 

2. Get involved in defining qualitative community labeling efforts. Commonly understood qualitative, semantic codes for specific rules, offenses, governance methods, and other things can go a long way in reducing the human cost of understanding how servers align in their philosophies and why a post might have been flagged.

3. It would be useful to have a library of reference samples for the kinds of messages that software will send and expect to receive. This would help give implementors examples to work against, and further conversation about protocol issues by providing concrete examples to talk about.

4. Having the ability to add notes and labels for *everything* (even things you don't think need them) and the ability to query those, would go a long way in helping both keep context across time into the future and provide ways to understand trends for what's happening now.

  Notes can be aggregatable text fields similar to comments, about some ... thing, such as a post, a flag activity, a user account, a block action, a blocklist entry, etc. It should be easy to get notes for things related to a thing you're looking at.

  Labels are something more than hashtags: Where a hashtag might be `#status/investigating`, a label could be `status: investigating`, which then allows one to more easily enumerate (and limit) the `status` types, and find things by those. By far and away this is the most common thing in my notes from these conversations, as I think it could be used for a wide variety of cases in helping to keep context both across time and across the aggregate of what's happening now. 

5. Building Query/Reporting tools into server software can go a long way towards helping proactive moderation; but also building out resources to share and translate these queries across server software. These queries should help with spotting patterns: What are the common email domains that signed up recently? What instances are submitting the most reports? What _types_ of reports are coming in?

6. Think more about surrounding context in your admin & moderation interfaces. This builds on Notes/Labels and Query/Reporting tools previously, but also requires getting out of the "this screen shows a database entry" frame-of-mind that often pervades webapp development. Consider the questions someone might ask, and make them easier to answer, with fewer steps to answer them. Let's say you're looking at a report of flagged content. What sorts of reports come in against this account? What sorts of reports come in from the submitting user, or remote server? Gathering all this information into one place, and sharing it for discussion, is a frustrating and difficult aspect of coordinating with other moderators.

7. Think about what shareable automation tooling would look like. There are very few places where "working code wins arguments" should apply, and this is very much NOT one of them. There are a *lot* of considerations, but I think the most important ones are:
  - How can I know that this plugin isn't going to do anything nefarious? Maybe a plugin needs network access to check an image fingerprint against known illegal content list. Maybe a plugin needs to read everything by a specific user. I think a capability-based permission system makes sense for this sort of thing.
  - How can I know that this plugin is going to do what I think it's going to do? Look at interfaces for creating email fitlers, across more clients than just the one you regularly use -- After setting some sorts of rules for selecting messages to act on, you're typically offered a preview of messages you currently have which would be selected by these rules. This helps you know that your selection logic is correct. A plugin meant to help reject incoming spam messages from other instances should offer a regression preview to illustrate how it would have handled previous messages, to provide confidence that it will behave the way the admin expects it to behave.

8. Work to develop suggestions for data retention policies that support both trust & safety needs as well as legal needs, and work to help people understand what good defaults server software should adopt. This is outside the expertize of almost everyone working in moderation.

9. In a broader sense, I think it would be useful to have a commonly understood set of things moderators need to do during the course of their work, a sort of shared training guide, and HOWTO-style guides for accomplishing those for different server softwares. 

# Conclusion

One of our biggest impressions from these interviews is that the people doing this work really care about their communities. The work of trust and safety is never-ending, and for people doing it on a volunteer basis it seems to be a labor of love. 

From here, I'm moving into exploring ideas for interfaces for managing federation and dealing with reports. I don't work with static mockup tools such as Figma -- for this type of work I prefer to create working prototype web pages, often locally-runnable using fixture data generated to match a variety of scenarios that came about during the initial research discussions. This approach tends to involve more work but I've seen it repeatedly short-circuit assumptions and miscommunications people have when looking at static images. I have a few "challenging moderation scenarios" in mind to replicate, but I'm still looking for a few more. If you've done moderation work and think I've missed something, please don't hesitate to [reach out](https://hachyderm.io/@mattly)!
