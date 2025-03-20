using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

	public ClaimsPrincipal Auth(Guid accountId, params Claim[] claims)
	{
		var identity = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, accountId.ToString()), ..claims], "MockContext");
		var principal = new ClaimsPrincipal(identity);

		MockHttpContext.Setup(m => m.User).Returns(principal);

		return principal;
	}
}