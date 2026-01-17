using System.Diagnostics;
using System.Net;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Letterbook.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
	public string? RequestId { get; set; }

	public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

	public int Status
	{
		get => Response.StatusCode;
		set => Response.StatusCode = value;
	}

	public string LongMessage
	{
		get => _longMessage ?? GetLongMessage();
		set => _longMessage = value;
	}

	private readonly ILogger<ErrorModel> _logger;
	private string? _longMessage;

	public ErrorModel(ILogger<ErrorModel> logger)
	{
		_logger = logger;
	}

	public void OnGet()
	{
		var exceptionFeature =
			HttpContext.Features.Get<IExceptionHandlerPathFeature>();
		if (exceptionFeature?.Error is CoreException coreException)
		{
			Status = GetStatus(coreException);
		}
		RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
	}

	private int GetStatus(CoreException e)
	{
		if (e.Flagged(ErrorCodes.WrongAuthority)) return (int)HttpStatusCode.MisdirectedRequest;
		if (e.Flagged(ErrorCodes.InvalidRequest)) return (int)HttpStatusCode.BadRequest;
		if (e.Flagged(ErrorCodes.MissingData)) return (int)HttpStatusCode.NotFound;
		if (e.Flagged(ErrorCodes.DuplicateEntry)) return (int)HttpStatusCode.Conflict;
		if (e.Flagged(ErrorCodes.PermissionDenied)) return (int)HttpStatusCode.Forbidden;
		if (e.Flagged(ErrorCodes.PeerError))
		{
			LongMessage = "There was a failed communication with a peer service";
			return (int)HttpStatusCode.InternalServerError;
		}
		if (e.Flagged(ErrorCodes.NetworkError))
		{
			LongMessage = "There was a problem connecting to a peer service";
			return (int)HttpStatusCode.InternalServerError;
		}
		return (int)HttpStatusCode.InternalServerError;
	}

	public string GetLongMessage()
	{
		return Status switch
		{
			401 => "You must log in to view that page",
			403 => "You do not have access to that page",
			404 => "Page not found",
			400 or 402 or (> 404 and < 500) => "There was a problem with your request",
			_ => "Internal error",
		};
	}

	public string Message()
	{
		return string.Join(' ', StringFormatters.SplitWords().Split(((HttpStatusCode)Response.StatusCode).ToString()));
	}
}