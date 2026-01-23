using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Mocks;
using Moq;
using Neovolve.Logging.Xunit;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

public class WebfingerProviderTests : WithMocks
{
	private readonly ITestOutputHelper _output;
	private readonly HttpClient _httpClient;
	private readonly WebFingerClient _webfinger;
	private readonly CancellationTokenSource _cancel;
	private readonly Models.Profile _profile;
	private readonly ICacheLogger<WebFingerClient> _logger;

	public WebfingerProviderTests(ITestOutputHelper output)
	{
		_output = output;
		output.WriteLine($"Bogus Seed: {Init.WithSeed()}");

		_logger = output.BuildLoggerFor<WebFingerClient>();
		_httpClient = new HttpClient(HttpMessageHandlerMock.Object);
		_webfinger = new WebFingerClient(_logger, _httpClient, ActivityPubClientMock.Object);
		_cancel = new CancellationTokenSource();
		_profile = new FakeProfile("peer.example").Generate();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_webfinger);
	}

	[Fact(DisplayName = "Should return results from successful query")]
	public async Task CanSearchSuccess()
	{
		var json = $$"""
		           {
		             "subject" : "acct:{{_profile.Handle}}@{{_profile.FediId.Authority}}",
		             "aliases" : [ "{{_profile.FediId}}" ],
		             "properties" : { },
		             "links" : [ {
		               "rel" : "self",
		               "type" : "application/activity+json",
		               "href" : "{{_profile.FediId}}",
		               "titles" : { },
		               "properties" : { }
		             } ]
		           }
		           """;
		HttpMessageHandlerMock.SetupResponse(m =>
		{
			m.StatusCode = HttpStatusCode.OK;
			m.Content = new StringContent(json, new UTF8Encoding(), new MediaTypeHeaderValue("application/jrd+json"));
		});
		ActivityPubClientMock.Setup(m => m.Fetch<Models.Profile>(_profile.FediId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(_profile);
		var actual = await _webfinger.SearchProfiles($"@{_profile.Handle}@{_profile.FediId.Authority}", _cancel.Token);

		Assert.Equal(0, _logger.Count);
		Assert.Equal([_profile], actual);
	}

	[Fact(DisplayName = "Should deduplicate fetch URIs")]
	public async Task CanDedupe()
	{
		var json = $$"""
		             {
		               "subject" : "acct:{{_profile.Handle}}@{{_profile.FediId.Authority}}",
		               "aliases" : [ "{{_profile.FediId}}" ],
		               "properties" : { },
		               "links" : [ {
		                 "rel" : "self",
		                 "type" : "application/activity+json",
		                 "href" : "{{_profile.FediId}}",
		                 "titles" : { },
		                 "properties" : { }
		               },
		               {
		               "rel" : "self",
		               "type" : "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"",
		               "href" : "{{_profile.FediId}}",
		               "titles" : { },
		               "properties" : { }
		             } ]
		             }
		             """;
		HttpMessageHandlerMock.SetupResponse(m =>
		{
			m.StatusCode = HttpStatusCode.OK;
			m.Content = new StringContent(json, new UTF8Encoding(), new MediaTypeHeaderValue("application/jrd+json"));
		});
		await _webfinger.SearchProfiles($"@{_profile.Handle}@{_profile.FediId.Authority}", _cancel.Token);

		ActivityPubClientMock.Verify(m => m.Fetch<Models.Profile>(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Once());
	}

	[Fact(DisplayName = "Should return partial results when cancelled")]
	public async Task CanPartialCancel()
	{
		var json = $$"""
		             {
		               "subject" : "acct:{{_profile.Handle}}@{{_profile.FediId.Authority}}",
		               "aliases" : [ "{{_profile.FediId}}" ],
		               "properties" : { },
		               "links" : [ {
		                 "rel" : "self",
		                 "type" : "application/activity+json",
		                 "href" : "{{_profile.FediId}}",
		                 "titles" : { },
		                 "properties" : { }
		               },
		               {
		               "rel" : "self",
		               "type" : "application/activity+json",
		               "href" : "https://some.other.example/value",
		               "titles" : { },
		               "properties" : { }
		             } ]
		             }
		             """;
		HttpMessageHandlerMock.SetupResponse(m =>
		{
			m.StatusCode = HttpStatusCode.OK;
			m.Content = new StringContent(json, new UTF8Encoding(), new MediaTypeHeaderValue("application/jrd+json"));
		});
		ActivityPubClientMock.Setup(m => m.Fetch<Models.Profile>(_profile.FediId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(_profile);
		_output.WriteLine("setup with delay");
		ActivityPubClientMock.Setup(m => m.Fetch<Models.Profile>(It.Is<Uri>(u => u != _profile.FediId), It.IsAny<CancellationToken>()))
			.Returns(DelayResponse(_cancel.Token));
		_cancel.CancelAfter(50);

		_output.WriteLine("call search");
		var actual = await _webfinger.SearchProfiles($"@{_profile.Handle}@{_profile.FediId.Authority}", _cancel.Token);

		Assert.Equal([_profile], actual);
		return;

		async Task<Models.Profile> DelayResponse(CancellationToken cancellationToken)
		{
			_output.WriteLine("delay");
			await Task.Delay(1000, cancellationToken);
			_output.WriteLine("delay complete");
			return default!;
		}
	}

	// https://docs.joinmastodon.org/spec/webfinger/
	[Fact(DisplayName = "Should translate query into correct URL")]
	public async Task TranslatesQueryIntoUrl()
	{
		HttpMessageHandlerMock.SetupResponse(m =>
		{
			m.StatusCode = HttpStatusCode.OK;
			m.Content = new StringContent("{}", new UTF8Encoding(), new MediaTypeHeaderValue("application/jrd+json"));
		});

		ActivityPubClientMock.Setup(m => m.Fetch<Models.Profile>(_profile.FediId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(_profile);

		await _webfinger.SearchProfiles($"@{_profile.Handle}@{_profile.FediId.Authority}", _cancel.Token);

		HttpMessageHandlerMock.Verify(it => it.SendMessageAsync(
			It.Is<HttpRequestMessage>(message =>
				message.RequestUri ==
				new Uri($"https://{_profile.FediId.Authority}/.well-known/webfinger?resource=acct%3A{_profile.Handle}%40{_profile.FediId.Authority}")
			), It.IsAny<CancellationToken>()));
	}
}