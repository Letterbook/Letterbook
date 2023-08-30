using System.Net;
using Letterbook.Api.Tests.Support;
using Letterbook.Api.Tests.Support.Extensions;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Models.WebFinger;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests.Resources.WebFinger;

/*

    https://docs.joinmastodon.org/spec/webfinger/

    WebFinger as described in RFC 7033 is a spec that defines a method for resolving links to a resource, given only a URI on a particular server. 
    
    This allows anyone to look up where a resource is located without having to know its exact location beforehand; for example, by email or phone number. 
    
    This lookup is directed at the endpoint /.well-known/webfinger, and a resource query parameter is passed along with the lookup. 
    
    The resource URI used with Mastodon is the acct: URI as described in RFC 7565, with the username of a profile that is hosted on a particular domain.

 */

public class Examples : IDisposable
{
    private readonly ITestOutputHelper _log;
    private readonly WebAdapter _web;
    private readonly HttpClient _client;

    public Examples(ITestOutputHelper log)
    {
        _log = log;
        _web = new WebAdapter();
        _client = _web.Client;

        _web.FakeAccountService.AlwaysReturn(Profile.CreatePerson(new Uri("https://mastodon.social"), ""));
    }

    [Fact]
    public async Task ItReturnsStatusCode200Okay()
    {
        // https://docs.joinmastodon.org/spec/webfinger/
        var reply = await _client.GetAsync("/.well-known/webfinger?resource=acct:gargron@mastodon.social");

        Assert.Equal(HttpStatusCode.OK, reply.StatusCode);
    }

    [Fact]
    public async Task ItQueriesForTheSuppliedQueryTarget()
    {
        await _client.GetAsync("/.well-known/webfinger?resource=acct:gargron@mastodon.social");

        _web.FakeAccountService.MustHaveBeenAskedToFind(new WebFingerQueryTarget
        {
            Username = "gargron",
            Domain = "mastodon.social"
        });
    }

    /*
    {
      "subject": "acct:Gargron@mastodon.social",
      "aliases": [
        "https://mastodon.social/@Gargron",
        "https://mastodon.social/users/Gargron"
      ],
      "links": [
        {
          "rel": "http://webfinger.net/rel/profile-page",
          "type": "text/html",
          "href": "https://mastodon.social/@Gargron"
        },
        {
          "rel": "self",
          "type": "application/activity+json",
          "href": "https://mastodon.social/users/Gargron"
        },
        {
          "rel": "http://ostatus.org/schema/1.0/subscribe",
          "template": "https://mastodon.social/authorize_interaction?uri={uri}"
        }
      ]
    }
     */
    [Fact]
    public async Task ItReturnsAJsonResourceDescriptor()
    {
        var profile = Profile.CreatePerson(new Uri("https://mastodon.social"), "Gargron");

        _web.FakeAccountService.AlwaysReturn(profile);

        var reply = await _client.GetAsync("/.well-known/webfinger?resource=acct:gargron@mastodon.social");

        var body = await reply.Content.ReadAsStringAsync();

        body.MustMatchJson(@"
            {
              ""subject"": ""acct:Gargron@mastodon.social"",
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