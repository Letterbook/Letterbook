# Architecture Update

A summary of the current architecture of the application

## Status

- [ ] Decided
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

## Hexagonal Architecture

The general philosophy of the project is still to follow a hexagonal architecture pattern. See [ADR-02][adr-02] for a more thorough discussion of what that means.

To quickly summarize: hexagonal architecture is a layered pattern. The core layer encapsulates all of the distinctive logic of the application. The outer layers can be thought of as either drivers or adapters. The core layer defines a number of adapter interfaces, and core services depend on those interfaces. Adapters implement those interfaces, to provide managed interaction with outside services, such as the database, message queues, clients, and so on. Some of these adapters will respond to external input and generally trigger behavior in the rest of the application. It's helpful to think of these as drivers. The `Letterbook.Api` project is the primary example of a driver at this stage.

All of this relies on dependency injection to function. Some host process is responsible for managing the application lifecycle, and configuring the dependency injection provider. At this stage, that is also Aspnet Core, as configured in `Letterbook.Api`. In the future, there may be additional configurations with multiple specialized hosts. In order to support easy health checks, those hosts will likely also be aspnet core applications.

## Current State
<!--
```mermaid
%% the svg below is rendered from this mermaid definition
%% the block type is really handy for architecture diagrams, but also really new and not well supported yet
block-beta
    columns 4
    block:Drivers
        columns 1
        Api["Letterbook.Api"]
        space:2
    end
    block:Core:2
        columns 2
        block
            columns 1
            block:Services
                columns 1
                a["AccountService"]
                b["PostService"]
                c["ProfileService"]
                d["TimelineService"]
                f["AuthorizationService"]
            end
            %% down1<["depends on"]>(down)
            space:1
            block:Events
                columns 1
                h["ActivityMessageService"]
                g["AccountEventService"]
                i["ProfileEventService"]
                j["PostEventService*"]
            end
        end
        block:Interfaces
            columns 1
            iapa{{"IAccountProfileAdapter"}}
            iapc{{"IActivityPubClient"}} 
            ifa{{"IFeedsAdapter"}}
            imba{{"IMessageBusAdapter"}}
            imbc{{"IMessageBusClient"}}
            ipa{{"IPostAdapter"}}
        end
    end
    block:Adapters:1
        columns 1
        db["Letterbook.Adapter.Db"]
        ap["Letterbook.Adapter.ActivityPub"]
        rx["Letterbook.Adapter.RxMessageBus"]
        ts["Letterbook.Adapter.TimescaleFeeds"]
    end

    db -- > ipa
    db -- > iapa
    ap -- > iapc
    rx -- > imba
    rx -- > imbc
    ts -- > ifa
    
    a -- > iapa
    b -- > ipa
    c -- > iapa
    c -- > iapc
    d -- > ifa
    d -- > ipa
    Services -- > Events
    Events -- > imba
    Events -- > imbc
    h -- > iapc

    Api -- > a
    Api -- > b
    Api -- > c
    Api -- > d
    Api -- > f
```
-->
![architecture-update-01.svg](assets%2Farchitecture-update-01.svg)
*not implemented yet

So, generally, core logic services sit in the center of the architecture, and everything else depends on them, or implements an interface that core services depend on. This isolates the core logic from any concerns related to interacting with outside systems or infrastructure. And the adapters are agnostic to the specifics of the core logic. They will generally handle core data models, but mostly to serialize them. Just like with other dependencies, core services should only handle core data models. They should generally not handle serialization or DTOs.

## Core Services
* `AccountService` manages actions related to accounts and identity. Signup, login, logout, password management, and contact methods would all be managed through the Account service.
* `ProfileService` manages actions related to external social profiles. Follows, followers, blocks, and profile data like display names and profile pictures are handled through the Profile service.
* `PostService` manages actions related to posting. Creating, sharing, and viewing posts and threads will generally rely on the Post service.
* `TimelineService` builds feeds of recent posts. Viewing the home feed or a list would be handled by the Timeline service.
* `AuthorizationService` encapsulates authorization logic for the rest of the application.

In addition, there are a variety of event and message services. These are generally pretty thin, and just exist to consistently schedule messages to an adapter.

## Infrastructure

* Postgresql is the core database for Letterbook. This is the system of record, and it stores all of Letterbook's application data.
* Timescale is a specialty timeseries database. Letterbook uses it to provide a materialized view of post feeds (and soon, notifications). Timescale is actually a plugin for Postgres, so in many cases it's possible to run with only the one database server.

These are not yet implemented, but Letterbook will also depend on an S3-compatible object store, and an email service. Farther down the line, it's also likely we'll optionally incorporate a text search database like ElasticSearch.

### Message Queues
In practice, ActivityPub requires some asynchronous message exchanges, often in large volumes. Like most other fediverse servers, Letterbook facilitates this by using message queues. At present, these are all in-memory Observables, provided by Rx.net. In future, this will optionally be provided by a dedicated external message queue, likely RabbitMQ or another AMQP service.

## Future Services

There are some obvious categorical feature gaps that will require new services to cover. We have no handling for any kind of multimedia, or any kind of files whatsoever. We'll need file and media services; services to handle notifications; moderation services; and web and streaming. There's likely other things I'm forgetting at the moment. Which is to say, the software architecture is going to grow a lot over time. Hopefully a short time, at that.

## Discussion


[adr-02]: ./02-architecture-design-patterns.md