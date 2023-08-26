# WebFinger

27-Aug-2023

> Translate `user@domain` mentions to actor profile URIs. -- [doc](https://docs.joinmastodon.org/spec/webfinger/)

[Issue 79](https://github.com/Letterbook/Letterbook/issues/79)

## Status

- [x] Decided (2023-07-10)
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

Implement the web adapter part that translates a request like this:

```
GET /.well-known/webfinger?resource=acct:gargron@mastodon.social
```

into a service call to do the actual work.

## Discussion

### Tests isolated to just the web adapter

The approach here is to verify the network part separately from the core part, isolating it carefully.

The network part is simple: it just needs to translate a network request into an evaluation of a core use case.

All we are verifying here is what the network call looks like and the shape of the data returned.

The actual implementation of the lookup is not (yet) implemented.

#### Overriding dependencies

An important part of this is allowing the web adapter to be isolated from its runtime dependencies.

This is done with the help of `Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory` and its `WithWebHostBuilder` extension.

```c#
[Fact]
public async Task ItQueriesForTheSuppliedQueryTarget()
{
    var fakeAccountService = new FakeAccountService();

    await using var web = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder => builder.ConfigureServices(s =>
            {
                s.RemoveAll<IAccountService>();
                s.AddScoped<IAccountService>(_ => fakeAccountService);
            }));

    var client = web.CreateClient();

    await client.GetAsync("/.well-known/webfinger?resource=acct:gargron@mastodon.social");

    fakeAccountService.MustHaveBeenAskedToFind(new WebFingerQueryTarget
    {
        Username = "gargron",
        Domain = "mastodon.social"
    });
}
```

This is what allows the web adapter to be isolated from dependencies and run in-memory.

### Authentication

No consideration made for authentication.
