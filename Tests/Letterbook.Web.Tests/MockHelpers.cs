using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Letterbook.Web.Tests;

public static class MockHelpers
{
	public static Mock<IUrlHelper> CreateMockUrlHelper(ActionContext context)
	{
		var urlHelper = new Mock<IUrlHelper>();
		urlHelper.SetupGet(h => h.ActionContext)
			.Returns(context);
		return urlHelper;
	}
}