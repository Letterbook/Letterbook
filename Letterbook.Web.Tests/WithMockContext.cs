using System.Security.Claims;
using System.Security.Principal;
using Letterbook.Core.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Letterbook.Web.Tests;

public abstract class WithMockContext : WithMocks
{
	protected List<Claim> Claims;
	protected ClaimsIdentity Identity;
	protected ClaimsPrincipal ClaimsPrincipal;
	protected Mock<IPrincipal> MockPrincipal;
	protected Mock<HttpContext> MockHttpContext;
	protected PageContext PageContext;

	public WithMockContext()
	{
		Claims = new List<Claim>();
		Identity = new ClaimsIdentity(Claims, "MockContext");
		ClaimsPrincipal = new ClaimsPrincipal(Identity);

		MockPrincipal = new Mock<IPrincipal>();
		MockPrincipal.Setup(x => x.Identity).Returns(Identity);
		MockPrincipal.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);

		MockHttpContext = new Mock<HttpContext>();
		MockHttpContext.SetupGet(m => m.User).Returns(ClaimsPrincipal);

		var actionContext = new ActionContext(MockHttpContext.Object, new RouteData(), new ActionDescriptor());
		PageContext = new PageContext(actionContext);
	}
}