# Add Codeberg support
 
Can we make the same build tasks as Github?

* [Hosted actions](https://codeberg.org/actions/meta)
* [Forgejo Actions | Reference](https://forgejo.org/docs/next/user/actions/reference/)

The runner can be installed locally.

## Log

### Follow quick start

https://forgejo.org/docs/next/user/actions/quick-start/

That does work only the runner must be `runs-on: codeberg-tiny`. A message in the UI says "No matching online runner with label: docker".

### Run build?

We want to run something like:

```sh
dotnet build Letterbook.sln
```

Got this error when using the smallrest runner: "The runner cancelled the job because it exceeds the maximum run time", so trying largest.

### Lazy runners

https://codeberg.org/actions/meta#lazy-runners

> ...we are considering experiments like offsite runners which only run when solar energy is available.

### repository 'https://data.forgejo.org/docker/setup-compose-action/' not found

This did not work:

```yaml
- name: Set up Docker Compose
    uses: docker/setup-compose-action@v1
```

### Use built-in services instead of docker compose

> PostgreSQL, MariaDB and other services can be run from container images -- [forgejo.org](https://forgejo.org/docs/latest/user/actions/advanced-features/#services).

The configuration can be directly copied and pasted from docker-compose.yml by the looks.

### failed to start container: Error response from daemon: rootlessport listen tcp 0.0.0.0:5432: bind: address already in use

```sh
medium@actions-tiny.aburayama.m.codeberg.org(version:v12.5.3) received task 2982672 of job test, be triggered by event: push
workflow prepared
🚀  Start image=ghcr.io/catthehacker/ubuntu:act-latest
  🐳  docker pull image=postgres:16-alpine platform=linux/amd64 username= forcePull=false
  🐳  docker pull image=timescale/timescaledb:2.17.2-pg15-oss platform=linux/amd64 username= forcePull=false
  🐳  docker pull image=ghcr.io/catthehacker/ubuntu:act-latest platform=linux/amd64 username= forcePull=false
Cleaning up services for job test
Cleaning up network for job test, and network name is: WORKFLOW-ef46779af3352479cc45538d237c581a
  🐳  docker pull image=postgres:16-alpine platform=linux/amd64 username= forcePull=false
  🐳  docker pull image=timescale/timescaledb:2.17.2-pg15-oss platform=linux/amd64 username= forcePull=false
  🐳  docker create image=postgres:16-alpine platform=linux/amd64 entrypoint=[] cmd=[] network="WORKFLOW-ef46779af3352479cc45538d237c581a"
  🐳  docker create image=timescale/timescaledb:2.17.2-pg15-oss platform=linux/amd64 entrypoint=[] cmd=[] network="WORKFLOW-ef46779af3352479cc45538d237c581a"
  🐳  docker run image=postgres:16-alpine platform=linux/amd64 entrypoint=[] cmd=[] network="WORKFLOW-ef46779af3352479cc45538d237c581a"
  🐳  docker run image=timescale/timescaledb:2.17.2-pg15-oss platform=linux/amd64 entrypoint=[] cmd=[] network="WORKFLOW-ef46779af3352479cc45538d237c581a"
failed to start container: Error response from daemon: rootlessport listen tcp 0.0.0.0:5432: bind: address already in use
```

You can set those ports to anything:

```yaml
pgsql:
        image: postgres:16-alpine
        command:
          - postgres
          - -c
          - config_file=/etc/postgresql/postgresql.conf
        environment:
          - POSTGRES_USER=letterbook
          - POSTGRES_PASSWORD=letterbookpw
          - POSTGRES_DB=letterbook
        ports:
          - "1337:1337"
        volumes:
          - db_data:/var/lib/postgresql/data
          - ./volumes/postgresql.conf:/etc/postgresql/postgresql.conf:z
          - ./volumes/pg_hba.conf:/etc/postgresql/pg_hba.conf:z
```

which is okay as long as you can configure the tests with it.


```sh
ConnectionStringPort=1337 ./scripts/integration-test.sh
```

### Npgsql.NpgsqlException : Failed to connect to 127.0.0.1:1337 No connection could be made because the target machine actively refused it.

```sh
---- Npgsql.NpgsqlException : Failed to connect to 127.0.0.1:5433
      -------- System.Net.Sockets.SocketException : No connection could be made because the target machine actively refused it.
```

#### The IP address of pgsql is on the same network as the container running the steps and there is no need for port binding

> The IP address of pgsql is on the same network as the container running the steps and there is no need for port binding. The postgres:15 image exposes the PostgreSQL port 5432 and a client will be able to connect as shown in this example. -- [forgejo.org](https://forgejo.org/docs/latest/user/actions/advanced-features/#services)

Iiiiiintersting -- I get the same locally if I stop the tailscale container.

So you have to have both running.

### Npgsql.NpgsqlException : Failed to connect to 127.0.0.1:5432 Connection refused

This is ia different error -- do we have password issue?

```sh
---- Npgsql.NpgsqlException : Failed to connect to 127.0.0.1:5432
-------- System.Net.Sockets.SocketException : Connection refused
```

No password issue because you get this with incorrect password:

```sh
Npgsql.PostgresException : 28P01: password authentication failed for user "letterbook"
```