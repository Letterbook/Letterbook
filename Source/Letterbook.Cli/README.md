```shell
dotnet run --project Source/Letterbook.Cli/ 
```

## Examples

### Add an account

Based on [the Mastodon CLI](https://docs.joinmastodon.org/admin/tootctl/#accounts-create).

```shell
dotnet run --project Source/Letterbook.Cli accounts create USERNAME --email EMAIL 
```

### List accounts

```shell
dotnet run --project Source/Letterbook.Cli accounts list
```

## Design

The CLI reuses the same dependency injection as the API to minimize duplication.

It executes at the service layer -- does not use the API.

## Configuration

See `appsettings.Development.json`.

## Logging
