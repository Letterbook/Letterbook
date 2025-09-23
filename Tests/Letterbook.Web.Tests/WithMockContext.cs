using System.Security.Claims;
using Letterbook.Core.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Letterbook.Web.Tests;

public abstract class WithMockContext : WithMocks
{
	protected List<Claim> Claims;
	protected ClaimsIdentity Identity;
	protected ClaimsPrincipal ClaimsPrincipal;
	protected Mock<HttpContext> MockHttpContext;
	protected PageContext PageContext;
	protected ServiceCollection Services;
	protected RouteValueDictionary RouteValues;
	protected RouteData RouteData;
	protected ActionDescriptor Action;

	protected WithMockContext()
	{
		Claims = new List<Claim>();
		Identity = new ClaimsIdentity(Claims, "MockContext");
		ClaimsPrincipal = new ClaimsPrincipal(Identity);
		Services = new ServiceCollection();
		RouteValues = new RouteValueDictionary();
		RouteData = new RouteData();
		Action = new ActionDescriptor();
		var contextData = new Dictionary<object, object?>();
		var features = new FeatureCollection();

		Services.AddRazorPages();

		MockHttpContext = new Mock<HttpContext>();
		MockHttpContext.SetupGet(m => m.User).Returns(ClaimsPrincipal);
		MockHttpContext.SetupGet(m => m.RequestServices).Returns(Services.BuildServiceProvider());
		MockHttpContext.SetupGet(m => m.Items).Returns(contextData);
		MockHttpContext.SetupGet(m => m.Features).Returns(features);
		MockHttpContext.SetupGet(m => m.Request.Scheme).Returns("http");
		MockHttpContext.SetupGet(m => m.Request.Host).Returns(new HostString("letterbook.example"));
		MockHttpContext.SetupGet(m => m.Request.RouteValues).Returns(RouteValues);

		var actionContext = new ActionContext(MockHttpContext.Object, RouteData, Action);
		PageContext = new PageContext(actionContext);
	}
}