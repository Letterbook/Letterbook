# Building Timelines

This document attempts to design and describe, at a high level, the data models and schemas involved in sharing posts and using them to build timelines. This is (I hope obviously) a work in progress.

```mermaid
erDiagram
    profile {

    }
    actor {

    }
    audience {
        uri id pk "ex https://letterbook.example/actor/@janedoe/followers"
        uri host "easily distinguish local and remote authority"
    }

    audience_member {
        int actor_id pk
        uri audience_id pk
    }

    share {
        uri audience_id pk,fk
        datetime date_shared pk "timeseries?"
        uri object_uri pk,fk
        uri actor_id fk
        string object_type
    }

    object {
        int id pk
        uri object_uri
    }

    audience 1--0+ audience_member : has
    actor 1--0+ audience_member : is
    profile 1--1+ actor : controls
    share 1--1+ audience : with
    actor 1--0+ share : initiates
    share 1+--1 object : refers_to
```