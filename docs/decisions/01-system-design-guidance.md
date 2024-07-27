# System Design Guidance

Priorities for the project, and some guidance on how to balance those priorities when making high level architectural decisions.

## Status

- [x] Decided (2023-06-20)
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

Updated 2024-07-26

# Decision

This is about guidance, so here's the guidance.

1. Prefer multiple providers. There will be system components that can be rented from managed service providers (you know, clouds). It's better to choose technologies that are available in multiple cloud providers. In the future, it would be even better to support multiple services in a category (MySQL AND PostreSQL, for example). That's not feasible now, but try not to design out the option of doing it later. 
2. Be self-hostable. Don't choose services that are only available in the cloud (like snowflake or dynamodb), or that are difficult to administer (like kafka). This goes double if it's only available from a single provider.
3. Plan for scale-up. Networks tend to grow over time. It should be a reasonable amount of work for a volunteer admin to add capacity as the need arises.


## Impact

> [!NOTE]
> Updated 2024-07-26 to reflect actual state of the project

Now
* DB - Postgres
* Timeseries DB - Timescale
* Scheduling - MassTransit (in-memory)
* UI Framework - Server-side rendering + HTMX

Future
* Object Storage - Any S3 compatible
* Message Queue - MassTransit (RabbitMQ)
* Streaming/Websockets - TBD, likely SignalR
* Javascript Framework - TBD, likely VueJS if needed
* XMPP - TBD
* ~~Orchestration - Something internal (threads, processes), then maybe Nomad?~~ (We're unlikely to build out generalized orchestration solutions)

Optional/Alternatives
* In-process DB - TBD (useful for development, strongly discouraged in production)
* File storage - Local filesystem (useful for development and small deployments, strongly discouraged in production)

## Context

Basically, this came up when I was considering which database engine should serve as the core database for the project. There are a number of goals for Letterbook, and I want to write them down, for the future.

### Optimize for the operators

There is some threshold of features that would make Letterbook viable for users. Beyond that, we should favor the needs of instance administrators, operations teams, and moderators. That group is likely to overwhelmingly be hobbyists and volunteers. They will often have shoestring budgets. They will probably live all over the world, speak numerous languages, and work in various time zones. In particular, I'm thinking of the experience that the https://hachyderm.io team had in setting up and scaling up their mastodon instance. They had a team of frankly world-class SREs, and they still struggled with it continuously for weeks. Many instances had to close registrations during the twitter migration because they couldn't operationally support the added demand. You shouldn't have to have a team of senior engineers to run a community chat service.

### Minimize cost

Letterbook should be maximally affordable, because people use these services for community projects, and fund them on donations. Businesses can just throw RAM or IOPS at problems until they go away, but that's not likely to be an option here. So system components need to actually be suited to the task and scale.


## Discussion

So, I think those technology choices are pretty reasonable. Except maybe Postgres. I don't *love* using Postgres for this. I considered a document db. I think the ones that could be viable are Mongo or CouchDB. The problem with mongo is that I can't find a managed provider that I think has a reasonable price point. And the problem with CouchDB is that the only managed provider I can find is IBM. I also considered a graph database. It's a social network, after all. The network graph is the key thing here. The problem again is that the only reasonable option is Neo4j, and it's super expensive to get as a managed service.

So, that leaves good old SQL databases. And of the common options, postgres is the one I'm most familiar with. It also has the Apache AGE extension, which probably solves the basic cases that would call for a graph database.

[See the original PR for some additional discussion about telemetry, feature flags, and a plug-in system.](https://github.com/Letterbook/Letterbook/pull/1)
