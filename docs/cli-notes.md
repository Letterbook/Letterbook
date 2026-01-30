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