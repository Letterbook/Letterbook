using Letterbook.Core.Exceptions;

namespace Letterbook.Api.Dto;

public class ErrorMessage
{
	public string ErrorCode { get; private set; }
	public string Reason { get; private set; }

	public ErrorMessage(Exception e) : this((ErrorCodes)e.HResult, e.Message)
	{ }

	public ErrorMessage(ErrorCodes code, string message)
	{
		ErrorCode = $"{(uint)code:X8}";
		Reason = message;
	}
}