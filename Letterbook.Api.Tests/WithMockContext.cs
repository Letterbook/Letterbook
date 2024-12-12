using System.Security.Claims;
using System.Security.Principal;
using Letterbook.Core.Tests;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Letterbook.Api.Tests;

public abstract class WithMockContext : WithMocks
{
	protected Mock<HttpContext> MockHttpContext;

	public WithMockContext()
	{
		MockHttpContext = new Mock<HttpContext>();
	}

	public ClaimsPrincipal Auth(params Claim[] claims)
	{
		var identity = new ClaimsIdentity(claims, "MockContenxt");
		var principal = new ClaimsPrincipal(identity);

		MockHttpContext.Setup(m => m.User).Returns(principal);

		return principal;
	}
}