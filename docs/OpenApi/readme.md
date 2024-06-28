# REST API Docs

These json docs are OpenAPI specs, covering the Letterbook, Mastodon, and ActivityPub REST endpoints provided by Letterbook. They are generated at build time, from Release builds. They should not be hand edited, as the changes will be overwritten on the next build.

These files are most useful in combination with some other OpenAPI tooling. Most IDEs and full-featured code editors can display them in a Swagger UI, and use them to provide templates and code completion for building REST requests. This can be a good way to explore the APIs, or perform ad-hoc testing. They can also be used to generate API clients.

Note that these docs _do not_ cover the web project's controllers. Those endpoints may change frequently, to suit the needs of the web client. They should not be considered a stable development target for 3rd parties. The other endpoints will also change as-needed during early development. But they are expected to stabilize eventually.