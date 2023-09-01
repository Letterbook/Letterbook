# Running Integration Tests

While I do like pure and well isolated unit tests, there are times where you can't effectively test things unless you can actually interact with real dependencies. Especially when it's a database.

That means integration tests require those services to be available. All of our currently required external services are managed in the docker-compose file. Any further service dependencies added in the future should also be included there, if possible. The tests themselves assume the same configuration as the docker-compose provides. It would be great if they were actually linked together, rather than just magically the same, but that's harder to do. It would likewise be great if the test runner could start and stop those services, but that's also harder to do.

So, to run the tests, you must first run the services. That's usually as simple as one command.

```shell
docker-compose up
```

You can then run tests in the `Letterbook.IntegrationTests` solution. Integration tests are managed in a dedicated solution because they're slower and have extra dependencies compared to unit tests.

```shell
dotnet test Letterbook.IntegrationTests.sln
```
