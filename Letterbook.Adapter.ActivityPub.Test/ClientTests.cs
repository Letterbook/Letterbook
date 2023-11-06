using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Letterbook.ActivityPub;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit.Abstractions;

namespace Letterbook.Adapter.ActivityPub.Test;

[Trait("ActivityPub", "Client")]
public class ClientTests : WithMocks
{
    private Client _client;
    private ITestOutputHelper _output;
    private FakeProfile _fakeProfile;
    private Models.Profile _profile;

    private readonly MediaTypeWithQualityHeaderValue AcceptHeader = MediaTypeWithQualityHeaderValue
        .Parse("""
               application/ld+json; profile="https://www.w3.org/ns/activitystreams"
               """);


    public ClientTests(ITestOutputHelper output)
    {
        var httpClient = new HttpClient(HttpMessageHandlerMock.Object);
        _client = new Client(Mock.Of<ILogger<Client>>(), httpClient);
        _output = output;

        _output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
        _fakeProfile = new FakeProfile("letterbook.example");
        _profile = _fakeProfile.Generate();
    }

    [Fact]
    public void Exists()
    {
        Assert.NotNull(_client);
    }

    [Fact(DisplayName = "Should send a Follow")]
    public async Task ShouldFollow()
    {
        var target = new FakeProfile().Generate();
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

        var actual = await _client.As(_profile).SendFollow(target.Inbox);

        Assert.Equal(FollowState.Accepted, actual);
        HttpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(), 
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact(DisplayName = "Should handle a Reject")]
    public async Task ShouldFollowReject()
    {
        var target = new FakeProfile().Generate();
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

        var actual = await _client.As(_profile).SendFollow(target.Inbox);

        Assert.Equal(FollowState.Rejected, actual);
    }

    [Fact(DisplayName = "Should handle a PendingReject")]
    public async Task ShouldFollowPendingReject()
    {
        var target = new FakeProfile().Generate();
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

        var actual = await _client.As(_profile).SendFollow(target.Inbox);

        Assert.Equal(FollowState.Pending, actual);
    }

    [Fact(DisplayName = "Should handle a PendingAccept")]
    public async Task ShouldFollowPendingAccept()
    {
        var target = new FakeProfile().Generate();
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

        var actual = await _client.As(_profile).SendFollow(target.Inbox);

        Assert.Equal(FollowState.Pending, actual);
    }
    
    [Fact(DisplayName = "Should handle a nonsense response")]
    public async Task ShouldFollowNonsense()
    {
        var target = new FakeProfile().Generate();
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

        var actual = await _client.As(_profile).SendFollow(target.Inbox);

        Assert.Equal(FollowState.None, actual);
    }
    
    [Fact(DisplayName = "Should handle server errors")]
    public async Task ShouldHandleServerErrors()
    {
        var target = new FakeProfile().Generate();
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

        await Assert.ThrowsAsync<ClientException>(async () => await _client.As(_profile).SendFollow(target.Inbox));
    }
    
    [Fact(DisplayName = "Should handle client errors returned from peer servers")]
    public async Task ShouldHandleClientErrors()
    {
        var target = new FakeProfile().Generate();
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

        await Assert.ThrowsAsync<ClientException>(async () => await _client.As(_profile).SendFollow(target.Inbox));
    }
}