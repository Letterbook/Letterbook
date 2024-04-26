using Microsoft.AspNetCore.Http;

namespace Letterbook.Api.Authentication.HttpSignature;

public interface IFederatedActorHttpSignatureVerifier
{
	IAsyncEnumerable<Uri> VerifyAsync(
		HttpContext context,
		CancellationToken cancellationToken);
}