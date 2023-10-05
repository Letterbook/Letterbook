# Setup Entity Framework

Install the [Entity Framework Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet).

```shell
dotnet tool install --global dotnet-ef
```

# Migrations

To generate a new migration:
```shell
dotnet ef migrations add NAME
```

To run all unapplied migrations:
```shell
dotnet ef database update
```

To migrate (down) to a specific migration
```shell
dotnet ef database update NAME
```

To delete the last migration (ONLY DURING DEVELOPMENT)
```shell
dotnet ef migrations remove
```

# Advice and notes

I'm writing this down so that it's written down. I'll do something better with it once there's more to it.

## Identifiers
For anything that can be received from a remote server via federation (notes, images, and profiles, at least), the ID has to be URI. That means local records of the same type also have to have a URI as the ID. That ID is composed from a more conventional localID that only local content will have, and thus needs to be nullable.

The other option is to treat local and federated content differently, and that seems worse.
