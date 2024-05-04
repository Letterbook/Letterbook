using System.Security.Claims;
using ActivityPub.Types;
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
	protected Mock<IActivityAdapter> ActivityAdapterMock;
	protected Mock<IPostAdapter> PostAdapterMock;
	protected Mock<IAccountProfileAdapter> AccountProfileMock;
	protected Mock<IMessageBusAdapter> MessageBusAdapterMock;
	protected Mock<IAccountEventService> AccountEventServiceMock;
	protected Mock<IActivityPubClient> ActivityPubClientMock;
	protected Mock<IActivityPubAuthenticatedClient> ActivityPubAuthClientMock;
	protected Mock<IProfileService> ProfileServiceMock;
	protected Mock<IAuthzProfileService> ProfileServiceAuthMock;
	protected IOptions<CoreOptions> CoreOptionsMock;
	protected Mock<MockableMessageHandler> HttpMessageHandlerMock;
	protected ServiceCollection MockedServiceCollection;
	protected Mock<IPostEvents> PostEventServiceMock;
	protected Mock<IPostService> PostServiceMock;
	protected Mock<IAuthzPostService> PostServiceAuthMock;
	protected Mock<IAuthorizationService> AuthorizationServiceMock;

	protected WithMocks()
	{
		HttpMessageHandlerMock = new Mock<MockableMessageHandler>();
		ActivityAdapterMock = new Mock<IActivityAdapter>();
		PostAdapterMock = new Mock<IPostAdapter>();
		AccountProfileMock = new Mock<IAccountProfileAdapter>();
		MessageBusAdapterMock = new Mock<IMessageBusAdapter>();
		AccountEventServiceMock = new Mock<IAccountEventService>();
		ActivityPubClientMock = new Mock<IActivityPubClient>();
		ActivityPubAuthClientMock = new Mock<IActivityPubAuthenticatedClient>();
		ProfileServiceMock = new Mock<IProfileService>();
		ProfileServiceAuthMock = new Mock<IAuthzProfileService>();
		PostEventServiceMock = new Mock<IPostEvents>();
		PostServiceMock = new Mock<IPostService>();
		PostServiceAuthMock = new Mock<IAuthzPostService>();
		AuthorizationServiceMock = new Mock<IAuthorizationService>();

		ActivityPubClientMock.Setup(m => m.As(It.IsAny<Profile>())).Returns(ActivityPubAuthClientMock.Object);
		PostServiceMock.Setup(m => m.As(It.IsAny<IEnumerable<Claim>>(), It.IsAny<Uuid7>())).Returns(PostServiceAuthMock.Object);
		ProfileServiceMock.Setup(m => m.As(It.IsAny<IEnumerable<Claim>>())).Returns(ProfileServiceAuthMock.Object);
		var mockOptions = new CoreOptions
		{
			DomainName = "letterbook.example",
			Port = "80",
			Scheme = "http"
		};
		CoreOptionsMock = Options.Create(mockOptions);

		MockedServiceCollection = new ServiceCollection();
		MockedServiceCollection.AddScoped<IAccountProfileAdapter>(_ => AccountProfileMock.Object);
		MockedServiceCollection.AddScoped<IMessageBusAdapter>(_ => MessageBusAdapterMock.Object);
		MockedServiceCollection.AddScoped<IAccountEventService>(_ => AccountEventServiceMock.Object);
		MockedServiceCollection.AddScoped<IActivityPubClient>(_ => ActivityPubClientMock.Object);
		MockedServiceCollection.AddScoped<IActivityPubAuthenticatedClient>(_ => ActivityPubAuthClientMock.Object);
		MockedServiceCollection.AddScoped<IProfileService>(_ => ProfileServiceMock.Object);
		MockedServiceCollection.TryAddTypesModule();
	}

	public void MockAuthorizeAllowAll()
	{
		// TODO: reflect over the Decision methods instead of individual setups
		AuthorizationServiceMock.Setup(s => s.View(It.IsAny<IEnumerable<Claim>>(), It.IsAny<IFederated>(), It.IsAny<Uuid7>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Create(It.IsAny<IEnumerable<Claim>>(), It.IsAny<IFederated>(), It.IsAny<Uuid7>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Delete(It.IsAny<IEnumerable<Claim>>(), It.IsAny<IFederated>(), It.IsAny<Uuid7>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Publish(It.IsAny<IEnumerable<Claim>>(), It.IsAny<IFederated>(), It.IsAny<Uuid7>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Update(It.IsAny<IEnumerable<Claim>>(), It.IsAny<IFederated>(), It.IsAny<Uuid7>()))
			.Returns(Allow);
		AuthorizationServiceMock.Setup(s => s.Report(It.IsAny<IEnumerable<Claim>>(), It.IsAny<IFederated>(), It.IsAny<Uuid7>()))
			.Returns(Allow);
		return;

		Decision Allow(IEnumerable<Claim> claims, IFederated _, Uuid7 __) => Decision.Allow("Mock", claims);
	}
}