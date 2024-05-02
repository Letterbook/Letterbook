using System.Net;
using Moq;
using Moq.Language.Flow;

namespace Letterbook.Core.Tests.Mocks;

/// <summary>
/// An implementation of <see cref="HttpMessageHandler"/> that delegates to a public, abstract method with an identical
/// signature. Instead of mocking <see cref="HttpMessageHandler"/> directly, mocks from this class can be set up with direct
/// expressions instead of protected setups.
/// </summary>
public abstract class MockableMessageHandler : HttpMessageHandler
{
	/// <summary>
	/// Delegates to <see cref="SendMessageAsync"/>. This method is sealed here, so that mocking frameworks won't override it.
	/// </summary>
	protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		return await SendMessageAsync(request, cancellationToken);
	}

	// ReSharper disable once MemberCanBeProtected.Global
	public abstract Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}

public static class HttpHandlerMockExtensions
{
	public static IReturnsResult<MockableMessageHandler> SetupResponse(
		this Mock<MockableMessageHandler> mock,
		HttpStatusCode responseStatus,
		Func<HttpRequestMessage, bool>? predicate = null)
	{
		return mock.SetupResponse(r => r.StatusCode = responseStatus, predicate);
	}

	public static IReturnsResult<MockableMessageHandler> SetupResponse(
		this Mock<MockableMessageHandler> mock,
		Action<HttpResponseMessage> setup,
		Func<HttpRequestMessage, bool>? predicate = null)
	{
		var mockSetup = predicate == null
			? mock.Setup(m => m.SendMessageAsync(
				It.IsAny<HttpRequestMessage>(),
				It.IsAny<CancellationToken>()
			))
			: mock.Setup(m => m.SendMessageAsync(
				It.Is<HttpRequestMessage>(msg => predicate(msg)),
				It.IsAny<CancellationToken>()
			));

		return mockSetup.ReturnsAsync(() =>
		{
			var response = new HttpResponseMessage();
			setup(response);
			return response;
		});
	}
}