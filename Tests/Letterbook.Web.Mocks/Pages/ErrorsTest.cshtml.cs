using Letterbook.Core.Authorization;
using Letterbook.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Mocks.Pages;

public class ErrorsTest(ILogger<ErrorsTest> logger) : PageModel
{
	public void OnGet(string err)
	{
		logger.LogInformation("Page ErrorsTest ({Error})", err);
		switch (err)
		{
			case null:
				return;
			case "missingdata":
				throw CoreException.MissingData<object>("SomeId");
			case "duplicate":
				throw CoreException.Duplicate("Duplicate message", "some-id");
			case "invalid":
				throw CoreException.InvalidRequest("Invalid mssage");
			case "unauthorized":
				throw CoreException.Unauthorized(Decision.Deny("denied", []));
			case "wrong":
				throw CoreException.WrongAuthority("You've come to the wrong server", new Uri("https://other-server.example"));
			case "error":
				throw CoreException.InternalError("Internal error");
					default:
				throw new Exception("Generic Exception");
		}
	}
}