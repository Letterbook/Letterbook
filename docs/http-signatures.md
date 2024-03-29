# HTTP Signatures

## Root
GET "/"

## Actor
GET "/actor"
### Me
GET "/actor/me"
### Followers
GET "/actor/@{name}/followers"
### Inbox
GET "/actor/{name}/inbox"

## Inbox
GET "/inbox"

## Key
GET "/key"

## Object
GET "/object/{object_id}"

## Outbox
GET "/outbox"

## Note
### Thread
GET "/note/{note_id}/thread/"
