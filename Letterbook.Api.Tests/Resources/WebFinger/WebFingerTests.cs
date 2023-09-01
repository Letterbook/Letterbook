using System.Net;
using Letterbook.Api.Tests.Support;
using Letterbook.Api.Tests.Support.Extensions;
using Letterbook.Core.Models;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests.Resources.WebFinger;

/*

    https://docs.joinmastodon.org/spec/webfinger/

    WebFinger as described in RFC 7033 is a spec that defines a method for resolving links to a resource, given only a URI on a particular server. 
    
    This allows anyone to look up where a resource is located without having to know its exact location beforehand; for example, by email or phone number. 
    
    This lookup is directed at the endpoint /.well-known/webfinger, and a resource query parameter is passed along with the lookup. 
    
    The resource URI used with Mastodon is the acct: URI as described in RFC 7565, with the username of a profile that is hosted on a particular domain.

 */

public class WebFingerTests : IDisposable
{
    private readonly ITestOutputHelper _log;
    private readonly WebAdapter _web;
    private readonly HttpClient _client;

    public WebFingerTests(ITestOutputHelper log)
    {
        _log = log;
        _web = new WebAdapter();
        _client = _web.Client;

        _web.FakeAccountService.AlwaysReturn(Profile.CreatePerson(new Uri("https://uss-voyager.example"), ""));
    }

    [Fact]
    public async Task ItReturnsStatusCode200Okay()
    {
        var reply = await _client.GetAsync("/.well-known/webfinger?resource=acct:coffee_nebula@uss-voyager.example");

        Assert.Equal(HttpStatusCode.OK, reply.StatusCode);
    }

    [Fact]
    public async Task ItQueriesForTheSuppliedQueryTarget()
    {
        await _client.GetAsync("/.well-known/webfinger?resource=acct:coffee_nebula@uss-voyager.example");

        _web.FakeAccountService.MustHaveBeenAskedToFind("acct:coffee_nebula@uss-voyager.example");
    }

    /*
        See sample at: // https://docs.joinmastodon.org/spec/webfinger/
     */
    [Fact]
    public async Task ItReturnsAJsonResourceDescriptor()
    {
        var profile = Profile.CreatePerson(new Uri("https://uss-voyager.example"), "coffee_nebula");

        _web.FakeAccountService.AlwaysReturn(profile);

        var reply = await _client.GetAsync("/.well-known/webfinger?resource=acct:coffee_nebula@uss-voyager.example");

        var body = await reply.Content.ReadAsStringAsync();

        body.MustMatchJson(@"
            {
              ""subject"": ""acct:coffee_nebula@uss-voyager.example"",
            }");
    }

    // FACT: The "acct" part is important
    // FACT: The query component MAY contain one or more "rel" parameters.
    // FACT: it returns application/activity+json
    // FACT: it returns 404 when resource does not exist

    public void Dispose()
    {
        _web.Dispose();
    }
}