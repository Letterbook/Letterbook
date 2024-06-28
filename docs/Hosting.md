# Hosting Configurations

## `Letterbook`

The Letterbook project is the host for a single, all-in-one configuration. This host project includes the Web, API, and Workers. This project can be scaled out by running multiple instances behind a load balancer. For very high availability/high capacity deployments, each of the main components can instead be run in isolation and scaled independently.

## `Letterbook.Api`

Just the API. This hosts the ActivityPub endpoints, and the first-party Letterbook and third-party Mastodon APIs.

### Scaling

The API handles inbound ActivityPub messages and API calls. Letterbook attempts to handle AP messages and reach an initially consistent state immediately. Received AP messages will usually produce side effects, which are handled by Workers.

## `Letterbook.Web`

Just the Web UI.

### Scaling

This can be scaled out for higher availability, but capacity is mostly constrained by the database.

## `Letterbook.Workers`

Workers provide most of the extended functionality that Letterbook offers. They compose posts into feeds, and deliver AP messages to peer instances. In the future, this will expand to include composing notification feeds, media post-processing, spam filtering, fetching link embeds, sending email, and most other things.

### Scaling

