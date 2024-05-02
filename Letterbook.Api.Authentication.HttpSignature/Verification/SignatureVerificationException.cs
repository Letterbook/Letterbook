namespace Letterbook.Api.Authentication.HttpSignature.Verification;

public class SignatureVerificationException : Exception
{
	public SignatureVerificationException() { }

	public SignatureVerificationException(string? message)
		: base(message) { }

	public SignatureVerificationException(string? message, Exception? innerException)
		: base(message, innerException) { }
}