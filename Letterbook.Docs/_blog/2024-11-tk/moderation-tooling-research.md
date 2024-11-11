---
title: Moderation Tooling Research
authors:
  - mattly
---

# Intro
- 1❡: What We Learned
- What we want to accomplish
- Moderator tooling is generally not a priority
- What I do, How I do it
- General overview of who we talked to
- What this report is & isn't

# Context
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