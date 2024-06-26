using Microsoft.AspNetCore.Http;

namespace Letterbook.Api.Authentication.HttpSignature.Verification;

public interface IFederatedActorHttpSignatureVerifier
{
	IAsyncEnumerable<Uri> VerifyAsync(
		HttpContext context,
		CancellationToken cancellationToken);
}