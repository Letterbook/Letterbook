# Third-Party HTTP Signatures

Key to federating Letterbook is the app's capacity to accept third-party  messages. Letterbook attempts to honor messages which meet the [RFC 9421](https://www.rfc-editor.org/rfc/rfc9421.html) standard, or its [2017 draft](https://datatracker.ietf.org/doc/draft-cavage-http-signatures/07/) which is used by Mastodon and other fediverse applications. The process of signature validation is basically the same regardless of whether RFC 9421 or its 2017 draft is being followed, which is:

1. HTTP Signatures verify the integrity of (part of) an HTTP message
2. They do this by defining a set of properties from the message (like specific headers or url components) to sign over. This property definition is used again in step 5.
3. They compose those properties into an ephemeral document, and sign that
4. Attach the signature and the definition, but not the ephemeral document, as new headers
5. The recipient must then use that definition to reconstruct the ephemeral document, and then verify the signature against that document

This process allows Letterbook to interact with its federated peers. For in-app client APIs Letterbook authorizes using conventional bearer tokens.

## Endpoints which honor third-party HTTP Signatures

- /actor/{id}
- /actor/{id}/inbox
- /actor/{id}/followers (etc for other collections)

## How we recompose ephemeral documents
## What fields we sign
## What fields we require to be signed
## How we lookup signing keys during verifications
## The algorithms we can sign and verify with
## How we handle unsigned or unverifiable requests
