# Integration Tests

Guidance on how, when, and why to use integration tests.

## Status

- [x] Decided (2023-12-27)
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

There are a number of external services that we depend on, and so we should test those interactions are functioning as expected. At the moment, that includes our databases and federation peers. That list will grow over time to include object storage, message queues, and email, at least. Managing the state of those external services is not trivial. And in some cases, exercising them could even have financial costs. We want to test these things, but it can easily become more trouble than it's worth.

The solution is to divide our tests into unit tests, which have fully mocked out these external dependencies; and integration tests, which haven't. In order to keep tests fast and easy to use, the integration tests won't be configured to run by default. That way there's no need to keep a fleet of running services and databases in your personal development environment, except for when you actually need them.

## Impact

The immediate impact is already in place: we have two dotnet solution files. `Letterbook.sln` is the default solution for the project. That's the one we would expect contributors to run during normal development, and it's where we would manage CI, builds, publishing artifacts, and so on. 

Then there's also `Letterbook.IntegrationTests.sln`. That solution includes integration test projects, which need additional prep to run succesfully. As much as possible, those external services are encapsulated for development in one of our `docker-compose` files.

The other important thing to consider is the design of tests themselves. We want to test as much as we can in unit tests, because those are fast and easy to run. But at the same time, we shouldn't burden then project with highly complex unit tests just to avoid running some integration tests. And the flip side of that is to keep integration tests focussed on just the components and interactions that can't be well exercised in unit tests. Don't load up integration tests with a larger scope than is needed. Basically, use the right tool for the job; balance; judgement; etc.

Finally, when building integration tests, there's almost certainly some extra work necessary to arrange the test properly. You might need to load test data into a database, or host a long-ish running process. This extended arrangement work should usually be done with `IClassFixture` to configure the resource once across the whole test run. Ideally, the setup fixture would restore the resource to a clean state. But if that doesn't work, or there's some other cleanup that needs to happen, that can be managed implementing `IDisposable` in the test class. The cleanup should just be done in the `Dispose` method.

## Context

Some terminology, in the interest of helping us all use these words to mean the same things:

- Unit test - any test that runs in complete isolation, and with complete control over the execution environment.
- Integration test - any test that runs against a live external dependency (such as a database or web server). These can still be partially isolated, to eliminate other dependencies that are not relevant to the test scenario.
- End-to-end or functional tests. Tests run with a fully integrated system and no isolation between components. They use all live external dependencies.

## Discussion

