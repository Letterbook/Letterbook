using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public class SignatureVerificationException : Exception
{
	public SignatureVerificationException() { }

	public SignatureVerificationException(string? message)
		: base(message) { }

	public SignatureVerificationException(string? message, Exception? innerException)
		: base(message, innerException) { }
}