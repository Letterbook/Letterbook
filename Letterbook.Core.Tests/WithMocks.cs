using System.Security.Claims;
using ActivityPub.Types;
using ActivityPub.Types.Conversion;
using Letterbook.Core.Adapters;
using Letterbook.Core.Authorization;
using Letterbook.Core.Models;
using Letterbook.Core.Tests.Mocks;
using Medo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Letterbook.Core.Tests;

public abstract class WithMocks
{
	public Mock<IDataAdapter> DataAdapterMock;
	public Mock<IAccountEventPublisher> AccountEventServiceMock;
	public Mock<IActivityPubClient> ActivityPubClientMock;
	public Mock<IActivityPubAuthenticatedClient> ActivityPubAuthClientMock;
	public Mock<IProfileService> ProfileServiceMock;
	public Mock<IAuthzProfileService> ProfileServiceAuthMock;
	public Mock<IProfileEventPublisher> ProfileEventServiceMock;
	public IOptions<CoreOptions> CoreOptionsMock;
	public Mock<MockableMessageHandler> HttpMessageHandlerMock;
	public Mock<IPostEventPublisher> PostEventServiceMock;
	public Mock<IPostService> PostServiceMock;
	public Mock<IAuthzPostService> PostServiceAuthMock;
	public Mock<IAuthorizationService> AuthorizationServiceMock;
	public Mock<ITimelineService> TimelineServiceMock;
	public Mock<IAuthzTimelineService> AuthzTimelineServiceMock;
	public Mock<IAccountService> AccountServiceMock;
	public Mock<IJsonLdSerializer> JsonLdSerializerMock;
	public Mock<IActivityScheduler> ActivityPublisherMock;
	public Mock<IApCrawlScheduler> ApCrawlerSchedulerMock;
	public Mock<IModerationService> ModerationServiceMock;
	public Mock<IAuthzModerationService> AuthzModerationServiceMock;

	public ServiceCollection MockedServiceCollection;

	protected WithMocks()
	{
		HttpMessageHandlerMock = new Mock<MockableMessageHandler>();
		DataAdapterMock = new Mock<IDataAdapter>();
		AccountEventServiceMock = new Mock<IAccountEventPublisher>();
		ActivityPubClientMock = new Mock<IActivityPubClient>();
		ActivityPubAuthClientMock = new Mock<IActivityPubAuthenticatedClient>();
		ProfileServiceMock = new Mock<IProfileService>();
		ProfileServiceAuthMock = new Mock<IAuthzProfileService>();
		ProfileEventServiceMock = new Mock<IProfileEventPublisher>();
		PostEventServiceMock = new Mock<IPostEventPublisher>();
		PostServiceMock = new Mock<IPostService>();
		PostServiceAuthMock = new Mock<IAuthzPostService>();
		AuthorizationServiceMock = new Mock<IAuthorizationService>();
		TimelineServiceMock = new Mock<ITimelineService>();
		AuthzTimelineServiceMock = new Mock<IAuthzTimelineService>();
		AccountServiceMock = new Mock<IAccountService>();
		JsonLdSerializerMock = new Mock<IJsonLdSerializer>();
		ActivityPublisherMock = new Mock<IActivityScheduler>();
		ApCrawlerSchedulerMock = new Mock<IApCrawlScheduler>();
		ModerationServiceMock = new Mock<IModerationService>();
		AuthzModerationServiceMock = new Mock<IAuthzModerationService>();

		ActivityPubClientMock.Setup(m => m.As(It.IsAny<Profile>())).Returns(ActivityPubAuthClientMock.Object);
		PostServiceMock.Setup(m => m.As(It.IsAny<IEnumerable<Claim>>())).Returns(PostServiceAuthMock.Object);
		ProfileServiceMock.Setup(m => m.As(It.IsAny<IEnumerable<Claim>>())).Returns(ProfileServiceAuthMock.Object);
		TimelineServiceMock.Setup(m => m.As(It.IsAny<IEnumerable<Claim>>())).Returns(AuthzTimelineServiceMock.Object);
		ModerationServiceMock.Setup(m => m.As(It.IsAny<IEnumerable<Claim>>())).Returns(AuthzModerationServiceMock.Object);
		var mockOptions = new CoreOptions
		{
			DomainName = "letterbook.example",
			Port = "80",
			Scheme = "http"
		};
		CoreOptionsMock = Options.Create(mockOptions);

		MockedServiceCollection = new ServiceCollection();
		MockedServiceCollection.AddScoped<IDataAdapter>(_ => DataAdapterMock.Object);
		MockedServiceCollection.AddScoped<IAccountEventPublisher>(_ => AccountEventServiceMock.Object);
		MockedServiceCollection.AddScoped<IActivityPubClient>(_ => ActivityPubClientMock.Object);
		MockedServiceCollection.AddScoped<IActivityPubAuthenticatedClient>(_ => ActivityPubAuthClientMock.Object);
		MockedServiceCollection.AddScoped<IProfileService>(_ => ProfileServiceMock.Object);
		MockedServiceCollection.TryAddTypesModule();
	}

	public void MockAuthorizeAllowAll()
	{
		// TODO: reflect over the Decision methods instead of individual setups
		AuthorizationServiceMock.Setup(s => s.View(It.IsAny<IEnumerable<Claim>>(), It.IsAny<object>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.View<object>(It.IsAny<IEnumerable<Claim>>()))
			.Returns(AllowNone);
		AuthorizationServiceMock.Setup(s => s.Create(It.IsAny<IEnumerable<Claim>>(), It.IsAny<object>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Create<object>(It.IsAny<IEnumerable<Claim>>()))
			.Returns(AllowNone);
		AuthorizationServiceMock.Setup(s => s.Delete(It.IsAny<IEnumerable<Claim>>(), It.IsAny<object>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Publish(It.IsAny<IEnumerable<Claim>>(), It.IsAny<object>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Update(It.IsAny<IEnumerable<Claim>>(), It.IsAny<object>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Update<object>(It.IsAny<IEnumerable<Claim>>()))
			.Returns(AllowNone);
		AuthorizationServiceMock.Setup(s => s.Attribute(It.IsAny<IEnumerable<Claim>>(), It.IsAny<object>(), It.IsAny<ProfileId>()))
			.Returns(AllowProfile);
		AuthorizationServiceMock.Setup(s => s.Any(It.IsAny<IEnumerable<Claim>>(), It.IsAny<ProfileId>()))
			.Returns(Allow);
		return;

		Decision Allow(IEnumerable<Claim> claims, object _) => Decision.Allow("Mock", claims);
		Decision AllowProfile(IEnumerable<Claim> claims, object _, ProfileId __) => Decision.Allow("Mock", claims);
		Decision AllowNone(IEnumerable<Claim> claims) => Decision.Allow("Mock", claims);
	}
}