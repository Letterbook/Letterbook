﻿# Contributing to Letterbook

- [Code of Conduct](#code-of-conduct)
- [Collaboration](#collaboration)
  - [Github Issues](#github-issues)
- [Documentation](#documentation)
  - [Useful links](#useful-links)
- [Development](#development)
  - [Build and Run](#build-and-run)
    - [Using the `dotnet` CLI](#using-the-dotnet-cli)
    - [Using VS Code](#using-vs-code)
    - [Using Jetbrains Rider and VisualStudio](#using-jetbrains-rider-and-visualstudio)
  - [Secrets](#secrets)
  - [Dependencies](#dependencies)
- [License](#license)

It's easy to talk about contributing code, but there are many other very important ways to contribute. We would gladly welcome contributions in any of these areas:

1. Research
2. Visual design
3. Documentation
4. Feature requests
5. Code

And this list will only grow over time.


## Code of Conduct

Please read the [Code of Conduct][coc]. It's adapted from the commonly used contributor covenant, but it's not just boilerplate. Be sure you can uphold the project's shared values and abide by the standards laid out in that document. You're more than welcome to ask for clarification.

These values will have a direct impact on the features we do and don't implement, how we prioritize them, and how they are designed. For example:

* We believe tracking and mass data harvesting are dangerous and unethical; we won't build or support those activities and will attempt to limit the personal data we collect
* We believe that cryptocoins, NFTs, and related technologies are purposefully wasteful and deceptive; we will not incorporate them
* We believe so-called AI—especially generative AI—is extractive, wasteful, hazardous, and usually deceptive; we will view it with skepticism
* We will prefer opt-in rather than opt-out dynamics wherever possible
* We will seek and respect consent, by helping people to make well informed choices without manipulation
* We will build moderation and administration tools that preserve autonomy and privacy, as well as foster safety


## Collaboration

Before you jump in and start working, the first thing to do is to talk to us. We will likely need to talk through the project goals, or the system design and architecture. And even if not, there's a good chance that whatever seems most urgent to you also seems most urgent to someone else. We would rather coordinate our efforts, to minimize waste and frustration when multiple people do overlapping work.

### Github Issues

This is the first, best way to begin a discussion. We're trying to track our active and planned efforts here on the project repository, because this is where it's the most visible and approachable. There are two project boards that you should check to get a sense of what's already planned and in-progress.

1. [Minimum Functionality][minimum-board]
2. [Single User Preview][preview-board]

Please look through those boards before opening a new issue. But if you don't see an issue for the topic you want to discuss, then feel free to open a new one. If you do find an appropriate issue, please leave a comment to start or join the conversation. Either **creating or commenting on issues opens up communication**. It also allows issues to be assigned to you. We do that as a way to keep track of what people are working on, and who we should follow up with if we need to follow up.

## Documentation

Writing docs is both hard and valuable. Because a lot of things about the project are still being established, it's even harder than normal. This means there's not a lot of documentation yet. This will be a focus in the near future. What we do have is several [Architecture Decision Record][adr-what] docs. You should look throught them. This is likely the best way to get up to speed on the [design, goals, and constraints][adr] for the project.

### Useful links

We plan to support enough of the Mastodon API to provide good compatibility with Mastodon clients. So you may find these docs to be valuable. They also provide some useful information about how to interoperate with other federated services.

* [Mastodon Docs](https://docs.joinmastodon.org/)
* [Mastodon API routes](https://docs.joinmastodon.org/dev/routes/)
* [ActivityPub Spec](https://www.w3.org/TR/activitypub/)
* [ActivityStreams Vocabulary](https://www.w3.org/TR/activitystreams-vocabulary/)

## Development

We've tried to make the process to get up and running as easy as possible. But it's easy for the people who work on a project regularly to not notice when complexity drifts in. If it's not straightforward to at least build and run the app for development, let us know and we'll try to correct that.

Most things are not implemented, but some of it has been stubbed out to provide some structure to the project. Letterbook is still in the very early stages. So if something looks broken, it probably is; it's not just you.

### Build and Run

In all cases, you will need to have the dotnet 7.0 sdk installed on your system. [Microsoft publishes][dotnet] instructions to download it on every platform they support (Windows, MacOS, and several common linux distros).

To confirm it's available and working, do:

```shell
dotnet --version
```

#### Using the `dotnet` CLI

```shell
dotnet watch run --project Letterbook.Api
```
Then [open Swagger][swagger]

To run unit tests:

```shell
dotnet test
```

#### Using VS Code

There are recommended extensions and `launch.json` targets configured in the repo. Install the extensions and then run the `Letterbook.Api` configuration.

Tests can be run from the suggested test explorer extension.

#### Using Jetbrains Rider and VisualStudio

There are `launchSettings.json` targets configured in the repo. Open Letterbook.sln and then run the `Letterbook.Api: http` configuration.

Tests can be run from the built-in test runner.

### Secrets

The app needs to access some secrets. As a matter of good security practice, those are not, and will never be, committed to source control. Not even fake or sample values. That means you have to provide your own. There are multiple options, but the easiest way will be to use Dotnet User Secrets. You can add your own with this command.

```shell
dotnet user-secrets set "HostSecret" "$(openssl rand -base64 32)" --project Letterbook.Api
```

The actual value isn't important as long as you're just running and debugging locally. So if you don't have openssl you can use any string of 32 characters. But using cryptographically secure secrets is a good habit to build.

### Dependencies

You may need to run some external services to accomplish much during development. This will likely become more true over time. These are provided as a docker-compose spec. You will need a docker-compose compatible runtime. Docker desktop, and podman + the docker-compose CLI both work.

This includes

* Postgres
* Minio (not implemented yet)
* RabbitMQ (not implemented yet)

To run these dependencies, simply do 

```shell
docker-compose up
```

If it's your first time setting up the database you will need to apply the Database migrations by running the entity framework tools.

This processes is documented in [Letterbook.Adapter.Db](Letterbook.Adapter.Db/readme.md).

## License

Letterbook is licensed under the [AGPL, version 3.0][license]. The maintainers may, at our discretion, change the license to any future version. Anyone who contributes to the project must do so under the same terms. That is, you are licensing your work to the project and the other contributors under that same license and any future version as becomes necessary.

[dotnet]: https://dotnet.microsoft.com/en-us/download
[swagger]: http://localhost:5127/swagger/index.html
[coc]: https://github.com/Letterbook/Letterbook/blob/main/CODE_OF_CONDUCT.md
[preview-board]: https://github.com/orgs/Letterbook/projects/5/views/2
[minimum-board]: https://github.com/orgs/Letterbook/projects/1/views/2
[license]: https://github.com/Letterbook/Letterbook/blob/main/LICENSE
[adr-what]: https://www.redhat.com/architect/architecture-decision-records
[adr]: https://github.com/Letterbook/Letterbook/tree/main/docs/decisions
