using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Letterbook.Adapter.ActivityPub.Signatures;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using static NSign.Constants;

namespace Letterbook.Adapter.ActivityPub.Exceptions;

public class VerifierException : CoreException
{
	public VerifierException(string? message) : base(message)
	{
	}

	public VerifierException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	public static VerifierException NoSignatures(Exception? innerEx = null,
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
	{
		var ex = new VerifierException("The request was not signed", innerEx)
		{
			Source = FormatSource(path, name, line),
		};
		ex.HResult |= (int)ErrorCodes.PermissionDenied;

		return ex;
	}

	public static VerifierException NoValidSignatures(RequestComponentProvider components, Exception? innerEx = null,
		[CallerMemberName] string name = "",
		[CallerFilePath] string path = "",
		[CallerLineNumber] int line = -1)
	{
		var ex = new VerifierException("None of the provided signatures is valid", innerEx)
		{
			Source = FormatSource(path, name, line),
		};
		ex.HResult |= (int)ErrorCodes.PermissionDenied.With(ErrorCodes.InvalidRequest);
		if (components.TryGetHeaderValues(Headers.Signature, out var signatures))
			ex.Data["Signature Header"] = signatures;
		if (components.TryGetHeaderValues(Headers.SignatureInput, out var inputs))
			ex.Data["Signature-Input Header"] = inputs;
		if (components.TryGetHeaderValues("User-Agent", out var userAgent))
			ex.Data["User-Agent Header"] = userAgent;

		return ex;
	}
}