# Notes on adding CLI

Other servers have a cli:

* https://docs.gotosocial.org/en/v0.20.3/admin/cli/
* https://docs.joinmastodon.org/admin/tootctl/#accounts-create

Can we implement one use case, for example [creating an account](https://docs.joinmastodon.org/admin/tootctl/#accounts-create):

```shell
cli accounts create USERNAME --email EMAIL
```

Using .NET because everything else is.

```shell
dotnet run --project Source/Letterbook.Cli/ 
```

Using [this guide to parameters](https://learn.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial).

## Processing arguments

## Dependency injection

Can we reuse everything from the web?

## Publish as a tool?

## Local only?

Assume that development configuration is all we need at the moment.

## Integration tests

The CLI is purely a client of the domain so we should be able to test it the same way.