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

The initial set of claims to work from

Profile 
- `Rename` (actions related to the display name and handle)
- `Edit` (actions related to other fields like description and pfp)
- `Follow` (follow other profiles)
- `Unfollow` (stop following other propfiles)
- `ApproveFollower` (approve, reject, or remove followers)
- `Block` (actions related to moderation tools like block, mute, etc)
- `ReadNotif` (view the notifications sent to the profile)
- `ClearNotif` (mark notifications as read)
- `Export` (actions related to exporting bulk data about the profile)
- `Sign` (signing client-provided data with a server-managed key)
- `ManageKeys` (add or revoke signing keys)
- `Grant` (grant claims to other agents)
- `Guest` (other claims are delegated, and some highly sensitive actions should still be prohibited)
- `Owner` (holder is the owner, useful to help limit the size of tokens)
- Post
  - `Draft`
  - `Publish`
  - `Delete`
  - `Read`
  - `ReadDraft`
  - `React`
  - `Share`
  - `SortReply`
  - `ApproveReply`
  - `Collect`
  - Coauthor stuff?
  - Mentions?
  - Private mentions?
  - Media stuff?
- Message
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

Claims have to be represented somehow, and we're overwhelmingly using JWTs for that. As a first pass, perhaps this kind of claims structure:

```json
{
  "kid": "blah",
  "alg": "blah"
}.{
  "iss": "https://letterbook.example",
  "exp": 1704088800,
  "profiles": {
    "coffee_nebula": ["Owner"],
    "seven9": ["ApproveReply", "SortReply", "ApproveFollower", "Block", "Guest"]
  },
  "accounts": {
    "katherine_janeway": ["Owner"]
  }
}.signature
```
In this example, this token was issued to `katherine_janeway`, the owner of profile `coffee_nebula`, and has also been granted some claims on profile `seven9`. Those additional claims would allow Janeway to assist with performing moderation tasks on the replies to posts made by `seven9`. This kind of thing is really helpful for the targets of internet pile-ons, and this authorization scheme allows us to facilitate that. In other apps, they would have had to just share passwords, which is bad for a lot of reasons.

## Non-claim attributes

Not all attributes can be expressed as claims on an auth token. For one thing, it's not practical. Tokens are usually passed as HTTP headers, and there are limitations on header size. For another, we may just not want to represent them as claims. For example. Members of a group should be able to read messages presented to the group. But it's possible that profiles could join and leave many groups, somewhat frequently. Claims attached to authentication tokens would need to trace back and update account-level models in order to express that, and that seems unwise. So, some attributes will need to be deduced from run-time data.

## Context

This decision doc is an opportunity to collect and frame some discussion around how we'll perform authorization throughout the app. As we're starting to build out more functionality, it's getting harder to defer this decion into the future. Changing the fundamental authorization scheme later on would be a huge undertaking (i.e. going from ABAC to RBAC). And having no codified scheme will cause no end of trouble. So, this should help us head off those worst case outcomes. It's also great if we can establish a relatively stable set of claims to work from. It should be easier to reorganize the claims themselves, so this doesn't have to be perfect.

## Discussion

A summary of the comments/issues/concerns that led up to the decision
