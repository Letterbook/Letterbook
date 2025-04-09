using Letterbook.Adapter.ActivityPub;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class WebfingerTests : WithMocks
{
	private readonly ITestOutputHelper _output;
	private WebfingerProvider _provider;
	private FakeProfile _fakeProfile;
	private Models.Profile _profile;

	public WebfingerTests(ITestOutputHelper output)
	{
		_output = output;
		_output.WriteLine($"Bogus Seed: {Init.WithSeed()}");
		_provider = new WebfingerProvider(Mock.Of<ILogger<WebfingerProvider>>(), CoreOptionsMock,
			ProfileServiceMock.Object);
		_fakeProfile = new FakeProfile("letterbook.example");
		_profile = _fakeProfile.Generate();
	}

	[Fact(DisplayName = "Should return the descriptor")]
	public async Task GetsDescriptor()
	{
		ProfileServiceAuthMock.Setup(m => m.FindProfiles(_profile.Handle, It.IsAny<string>()))
			.Returns(new List<Models.Profile> { _profile }.ToAsyncEnumerable());

		var actual = await _provider.GetResourceDescriptorAsync(new Uri($"acct:{_profile.Handle}@letterbook.example"),
			Array.Empty<string>(), Mock.Of<HttpRequest>(), CancellationToken.None);

		Assert.NotNull(actual);
		Assert.Single(actual.Links);
		Assert.Equal(_profile.FediId.ToString(), actual.Links.First().Href?.ToString());
	}

	[Fact(DisplayName = "Should not return a descriptor when no profiles are found")]
	public async Task GetsNoDescriptor()
	{
		ProfileServiceAuthMock.Setup(m => m.FindProfiles(_profile.Handle, It.IsAny<string>()))
			.Returns(Array.Empty<Models.Profile>().ToAsyncEnumerable());

		var actual = await _provider.GetResourceDescriptorAsync(new Uri($"acct:{_profile.Handle}@letterbook.example"),
			Array.Empty<string>(), Mock.Of<HttpRequest>(), CancellationToken.None);

		Assert.Null(actual);
	}

	[Fact(DisplayName = "Should not return a descriptor when the query is for the wrong domain")]
	public async Task GetsNoDescriptorWrongDomain()
	{
		var req = new Mock<HttpRequest>();
		req.SetupGet<IHeaderDictionary>(m => m.Headers).Returns(new HeaderDictionary());

		ProfileServiceAuthMock.Setup(m => m.FindProfiles(_profile.Handle))
			.Returns(Array.Empty<Models.Profile>().ToAsyncEnumerable());

		var actual = await _provider.GetResourceDescriptorAsync(new Uri($"acct:{_profile.Handle}"),
			Array.Empty<string>(), req.Object, CancellationToken.None);

		Assert.Null(actual);
	}

	[Fact(DisplayName = "Should not return a descriptor when the format is wrong")]
	public async Task WrongFormat()
	{
		var req = new Mock<HttpRequest>();
		req.SetupGet<IHeaderDictionary>(m => m.Headers).Returns(new HeaderDictionary());

		var actual = await _provider.GetResourceDescriptorAsync(new Uri("http://letterbook.example/actor/test"),
			Array.Empty<string>(), req.Object, CancellationToken.None);

		Assert.Null(actual);
	}
}