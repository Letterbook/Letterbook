# Setup Entity Framework

The Entity Framework Core CLI tools are set up as a local tool for the project. You can restore it using the dotnet CLI.

```shell
dotnet tool restore
```

# Migrations

You should either run these commands from this subdirectory or add the `--project Letterbook.Adapter.DB` flag.

```shell
cd Letterbook.Adapter.Db
```

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
