using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using Letterbook.ActivityPub;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Tests.Fixtures;
using Letterbook.Core.Values;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

public class ClientTests : WithMocks, IClassFixture<JsonLdSerializerFixture>
{
    private Client _client;
    private ITestOutputHelper _output;
    private FakeProfile _fakeProfile;
    private Models.Profile _profile;
    private Models.Profile _targetProfile;

    private readonly MediaTypeWithQualityHeaderValue AcceptHeader = MediaTypeWithQualityHeaderValue
        .Parse("""
               application/ld+json; profile="https://www.w3.org/ns/activitystreams"
               """);


    public ClientTests(ITestOutputHelper output, JsonLdSerializerFixture fixture)
    {
        var httpClient = new HttpClient(HttpMessageHandlerMock.Object);
        _client = new Client(Mock.Of<ILogger<Client>>(), httpClient, fixture.JsonLdSerializer);
        _output = output;

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile("letterbook.example");
        _profile = _fakeProfile.Generate();
        _targetProfile = new FakeProfile().Generate();
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_client);
    }

    [Fact(DisplayName = "Should send a Follow")]
    public async Task ShouldFollow()
    {
        var response = JsonSerializer.Serialize(new AsAp.Activity()
        {
            Type = "Accept",
            Object = new List<AsAp.IResolvable>() { new AsAp.Activity() { Type = "Follow" } }
        }, JsonOptions.ActivityPub);
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                }
            );

        var actual = await _client.As(_profile).SendFollow(_targetProfile);

        Assert.Equal(FollowState.Accepted, actual);
        HttpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact(DisplayName = "Should handle a Reject")]
    public async Task ShouldFollowReject()
    {
        var response = JsonSerializer.Serialize(new AsAp.Activity()
        {
            Type = "Reject",
            Object = new List<AsAp.IResolvable>() { new AsAp.Activity() { Type = "Follow" } }
        }, JsonOptions.ActivityPub);
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                }
            );

        var actual = await _client.As(_profile).SendFollow(_targetProfile);

        Assert.Equal(FollowState.Rejected, actual);
    }

    [Fact(DisplayName = "Should handle a PendingReject")]
    public async Task ShouldFollowPendingReject()
    {
        var response = JsonSerializer.Serialize(new AsAp.Activity()
        {
            Type = "PendingReject",
            Object = new List<AsAp.IResolvable>() { new AsAp.Activity() { Type = "Follow" } }
        }, JsonOptions.ActivityPub);
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                }
            );

        var actual = await _client.As(_profile).SendFollow(_targetProfile);

        Assert.Equal(FollowState.Pending, actual);
    }

    [Fact(DisplayName = "Should handle a PendingAccept")]
    public async Task ShouldFollowPendingAccept()
    {
        var response = JsonSerializer.Serialize(new AsAp.Activity()
        {
            Type = "PendingAccept",
            Object = new List<AsAp.IResolvable>() { new AsAp.Activity() { Type = "Follow" } }
        }, JsonOptions.ActivityPub);
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                }
            );

        var actual = await _client.As(_profile).SendFollow(_targetProfile);

        Assert.Equal(FollowState.Pending, actual);
    }

    [Fact(DisplayName = "Should handle a nonsense response")]
    public async Task ShouldFollowNonsense()
    {
        var response = JsonSerializer.Serialize(new AsAp.Activity() { Type = "Question", }, JsonOptions.ActivityPub);
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(response)
                }
            );

        var actual = await _client.As(_profile).SendFollow(_targetProfile);

        Assert.Equal(FollowState.None, actual);
    }

    [Fact(DisplayName = "Should handle server errors")]
    public async Task ShouldHandleServerErrors()
    {
        var response = """
                       {"Error": "This is not an ActivityPub object, so it shouldn't parse as one"}
                       """;
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(response)
                }
            );

        await Assert.ThrowsAsync<ClientException>(async () => await _client.As(_profile).SendFollow(_targetProfile));
    }

    [Fact(DisplayName = "Should handle client errors returned from peer servers")]
    public async Task ShouldHandleClientErrors()
    {
        var response = """
                       {"Error": "This is not an ActivityPub object, so it shouldn't parse as one"}
                       """;
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(response)
                }
            );

        await Assert.ThrowsAsync<ClientException>(async () => await _client.As(_profile).SendFollow(_targetProfile));
    }

    [Fact(DisplayName = "Should send an Accept:Follow")]
    public async Task SendAcceptFollow()
    {
        HttpRequestMessage? message = default;
        HttpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage m, CancellationToken _) => message = m)
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted });

        await _client.As(_profile).SendAccept(_targetProfile.Inbox, Models.ActivityType.Follow,
            _targetProfile.Id, _profile.Id);

        Assert.NotNull(message?.Content);

        var payload = await message.Content.ReadAsStringAsync();
        var actualAccept = JsonNode.Parse(payload);
     
        // Assert on the outer Accept activity
        Assert.NotNull(actualAccept);
        Assert.Equal("Accept", actualAccept["type"]!.GetValue<string>());
        Assert.Equal(_profile.Id.ToString(), actualAccept["actor"]!.GetValue<string>());

        // Assert on the inner Follow activity
        var actualFollow = actualAccept["object"];
        Assert.NotNull(actualFollow);
        Assert.Equal("Follow", actualFollow["type"]!.GetValue<string>());
        Assert.Equal(_targetProfile.Id.ToString(), actualFollow["actor"]!.GetValue<string>());
        Assert.Equal(_profile.Id.ToString(), actualFollow["object"]!.GetValue<string>());
    }
}