# Authorization Claims

Letterbook makes use of a attribute based authorization scheme. Attributes are primarily (but likely not exclusively) expressed as claims. This describes that scheme, and an initial set of claims to work from.

## Status

- [ ] Decided
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

A summary of what was proposed and decided

## Claims

This set of claims captures the foreseeable actions that clients would attempt to perform for regular members. It ignores a lot of moderator and system administrator needs, for now. We should revisit that after we've done some admin and moderator focussed UX research. It's worth noting that many of these claims might never be delegated. Part of the value in enumerating them is to make it unambiguous that they are withheld when some other claims _are_ delegated. 

Profile
- `Owner` (holder is the owner, useful to help limit the size of tokens)
- `Guest` (holder has (or may have) some delegated claims from another party)
- `Rename` (actions related to the display name and handle)
- `Edit` (actions related to other fields like description and pfp)
- `Follow` (follow other profiles)
- `Unfollow` (stop following other profiles)
- `ApproveFollower` (approve, reject, or remove followers)
- `Block` (actions related to moderation tools like block, mute, etc)
- `ReadNotif` (view the notifications sent to the profile)
- `ClearNotif` (mark notifications as read)
- `Export` (actions related to exporting bulk data about the profile)
- `Sign` (signing client-provided data with a server-managed key)
- `ManageKeys` (add or revoke signing keys)
- `Grant` (grant claims to other agents)
- `Migrate` (move to another profile ID, typically on another server)
- `Post:` (claims related to posting)
  - `Draft`
  - `Publish:`
    - `Any`
    - `Public`
    - `Private`
  - `Delete`
  - `Read:`
    - `OwnPublic`
    - `OwnPrivate`
    - `AnyPublic`
    - `AnyPrivate`
  - `ReadDraft`
  - `React`
  - `Share`
  - `SortReply`
  - `ApproveReply`
  - `Collect`
  - `InviteAuthor` (publishing posts with a coauthor should always require active approval from all authors[^1])
- `Message:` (claims related to direct messages)
  - `Read`
  - `Send`
  - `SeeContacts`
  - `SeePresence`

Account
  - `ViewInfo`
  - `ReadNotif`
  - `ClearNotif`
  - `ManageInfo`
  - `ManageCredentials`
  - `Grant`
  - `Guest` 
  - `Owner`

## Structure

Not every claim that we grant is appropriate to encode into authorization tokens. For many of these claims, revocation would be a time-sensitive concern. Making authorization tokens revocable is potentially quite expensive, in terms of compute resources and latency. However, we also want to authorize the most common requests efficiently. We can expect the most common scenario, by a wide margin, to be a single account acting on one or more profiles that it has full ownership of. So it's likely adequate to encode simply whether an identity's claims are their own, or delegated. We would then only need to confirm delegated claims. And since we would likely need to confirm _every_ delegated claim, there's limited value in embedding them in great detail.

To use an example JWT, consider this claim structure for a simple case, where the account has claims only on their own profile:

```json
{
  "kid": "blah",
  "alg": "blah"
}.{
  "iss": "https://letterbook.example",
  "exp": 1704088800,
  "sub": "katherine_janeway",
  "profile:coffee_nebula": ["Owner"]
}.signature
```

As a more complex example consider the same `katherine_janeway` as the owner of multiple profiles, and holding some delegated claims on someone else's profile:

```json
{
  "kid": "blah",
  "alg": "blah"
}.{
  "iss": "https://letterbook.example",
  "exp": 1704088800,
  "sub": "katherine_janeway",
  "profile:coffee_nebula": ["Owner"],
  "profile:uss_voyager": ["Owner"],
  "profile:seven9": ["Guest"]
}.signature
```

In this case, API level authorization could allow requests affecting the `seven9` profile to proceed, with additional authorization happening later on in processing the request, once the necessary information is available and the set of current claims have all been gathered.

And a third (somewhat hypothetical) scenario, where `katherine_janeway` has provisioned an API key to delegate machine access to an explicit subset of their regular claims, which would support some automated moderation of their own threads and profile:

```json
{
  "kid": "blah",
  "alg": "blah"
}.{
  "iss": "https://letterbook.example",
  "exp": 1704088800,
  "sub": "katherine_janeway",
  "act" {"sub": "apikey:main_computer"},
  "profile:uss_voyager": ["Owner", "ApproveFollower", "Block", "Post:ApproveReply"]
}.signature
```

For machine users, it likely makes more sense to encode specifically granted claims. We can expect that API keys would be mainly used by an account owner, rather than a 2nd party in whom they may need to revoke trust.

## Non-claim attributes

Not all attributes can be expressed as claims on an auth token. For one thing, it's not practical. Tokens are usually passed as HTTP headers, and there are limitations on header size. For another, we may just not want to represent them as claims. For example. Members of a group should be able to read messages presented to the group. But it's possible that profiles could join and leave many groups, somewhat frequently. Claims attached to authentication tokens would need to trace back and update account-level models in order to express that, and that seems unwise. So, some attributes will need to be deduced from run-time data.

## Context

This decision doc is an opportunity to collect and frame some discussion around how we'll perform authorization throughout the app. As we're starting to build out more functionality, it's getting harder to defer this decion into the future. Changing the fundamental authorization scheme later on would be a huge undertaking (i.e. going from ABAC to RBAC). And having no codified scheme will cause no end of trouble. So, this should help us head off those worst case outcomes. It's also great if we can establish a relatively stable set of claims to work from. It should be easier to reorganize the claims themselves, so this doesn't have to be perfect.

## Discussion

[See the original PR for an extended discussion][PR]

### Relationality of claims in the authz token

This encoding of claims minimizes the amount of relational information that's encoded in the authorization token itself. It should be adequate to fully authorize the most common scenarios, and it should also allow efficient denial for actions which are definitely not authorized. The middle ground, where an action _might_ be authorized, would always require actively checking claims at the source.

It would always be the case that an account could have claims on multiple profiles, and profiles are the main object Letterbook uses to model communication. So including that relationship at some level seems hard to avoid.

---
[^1]: I imagine coordinating that approval would involve an exchange of single-purpose authorization tokens, but that's a topic for its own ADR (and likely a FEP, afterward)

[PR]: https://github.com/Letterbook/Letterbook/pull/183