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

Context is also, often, information. _Who are the people involved?_ can become _What do we know about the people involved?_ Providing access to this kind of information is a common problem point, and one that tooling could often do a much better job of helping with: One instance admin described giving their mod team read-only access to the database along with a library of queries to get at information their instance software doesn't make available. There's lots of room for improvement here!

## Reports
The main case for a moderator gathering information is in dealing with reports. Challenges in evaluating a report can stem from the instance software's design, its data architecture decisions, the design of ActivityPub, differences between fedi sofrwares, and/or the distributed nature of federation. Social information plays an important role, too.

From a design & data standpoint, many questions arise that are difficult to answer: What is the history of a user who is the subject of a report -- what reports have been filed against them previously? What is the history of the user filing the report? Is this user receiving a lot of other reports at the same time? Who are the accounts doing the reporting? If the report creator is remote, what information can you get about them? What is the recent or overall history of report exchanges between your instance and the remote instance? It can be hard to distinguish situations where someone is a bad actor compared to someone who is under attack by sockpuppet accounts.

Sometimes a post that is part of a report is a response to a post that's been deleted, and since the instance software deletes those posts immediately, the full thread is gone. It's completely possible for a report to become "un-solveable" by a reasonable standard, but the model around ActivityPub flags don't allow for this.

From a protocol standpoint, the ActivityPub `Flag` activity used for reporting has loose semantic definitions for its fields, and there is little guidance on what fields are supposed to indicate. This leaves implementors to define that, which causes problems from an interoperability standpoint. The ActivityPub Trust and Safety group is working to define this better.

Instance software treats reporting between instances differently as well, some software sends inter-instance reports straight to the remote admins, some require the local admins to forward it, and some don't let the local admins forward.

Socially, a common concern & problem is understanding the context in which a report is taking place. Language shifts between cultures, marginalized groups reclaim slurs, and it can be hard to keep track of that.  

# De/Federating & Blocking

# Mistake-Proofing, Roles

# Operational Visibility

## Context is Social
- Who are people involved? What are their norms? What are their rules? 
- easier at small scale, harder at large scale
## Context is Data
- who said what, when? What related information is needed? What patterns can't I see?
- data visibility; RBAC, deletions, not stored
- what are the consequences of an action I'm going to take?
  - accidental deletions from suspending users
  - releationships severed due to defederation
- protocol makes coherent data model a problem
- operational visibility: what's happening that needs attention? actionable data vs "oh, neat?"
- trust profiles for an account (history, posts, email/ips/links)

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