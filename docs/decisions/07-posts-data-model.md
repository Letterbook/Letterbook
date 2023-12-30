# Data Model for Generic Posts

We can expect almost any of the ActivityStreams Object types to federate in a way that resembles a post. And we will want to do the same eventually; we don't need to limit ourselves to just Notes. This offers a way to represent posts in a timeline, thread, etc as a generic container for their content.

## Status

- [ ] Decided 
- [ ] Decided Against 
- [ ] Deferred 
- [ ] Superseded 

# Decision

A summary of what was proposed and decided

```mermaid
erDiagram
    Note 1 -- zero or one Post : posted
    Image 1+ -- 0+ Post : posted
    Etc 1+ -- 0+ Post : posted
    Post 0+ -- 1+ Profile : created-by
    Post 0+ -- 0+ Profile : liked-by
    Post 0+ -- 0+ Profile : shared-by
    Post 0+ -- 0+ Profile: addressed-to
    Post 0+ -- zero or one Post : in-reply-to
    
    Post {
        uri id pk
        uuid localId
        date created
        date updated
        string client
    }
    
    Note {
        uuid id pk
        uri uri
        string summary
        string preview
        string source
    }
    
    Image {
        uuid id pk
        uri uri
        string summary
        string preview
        string source
    }
    
```

## Impact

Details about what changes because of the decision

## Context

Information about the situation that prompted the proposal/decision

## Discussion

A summary of the comments/issues/concerns that led up to the decision