using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Letterbook.Adapter.ActivityPub.Signatures;
using Letterbook.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSign;
using NSign.Http;
using NSign.Providers;

namespace Letterbook.Api.Authentication.HttpSignature.Verification;

[UsedImplicitly]
public class FederatedActorHttpSignatureVerifier(
	ILoggerFactory loggerFactory,
	IVerificationKeyProvider verificationKeyProvider) : IFederatedActorHttpSignatureVerifier
{
	private static readonly HttpFieldOptions HttpFieldOptions = new();

	private readonly ILogger _logger = loggerFactory.CreateLogger<FederatedActorHttpSignatureVerifier>();

	public async IAsyncEnumerable<Uri> VerifyAsync(
		HttpContext context,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var signingContext = new AspNetCoreMessageSigningContext(_logger, HttpFieldOptions, context);
		if (signingContext.HasSignaturesForVerification)
		{
			await foreach (var specVerified in VerifyRfcSignature(signingContext, cancellationToken))
			{
				yield return specVerified;
			}
		}
		else
		{
			await foreach (var mastodonVerified in VerifyMastodonSignature(context.Request, cancellationToken))
			{
				yield return mastodonVerified;
			}
		}
	}

	private async IAsyncEnumerable<Uri> VerifyMastodonSignature(HttpRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var mastodonVerifier = new MastodonVerifier(loggerFactory.CreateLogger<MastodonVerifier>());
		var signatureComponents = GetMastodonSignatureComponents();
		if (!signatureComponents.Any())
		{
			yield break;
		}

		foreach (var signatureComponent in signatureComponents)
		{
			var key = await GetKey(signatureComponent.KeyId, cancellationToken);
			if (key == null)
			{
				continue;
			}

			try
			{
				mastodonVerifier.VerifyRequestSignature(request, key);
			}
			catch (VerifierException)
			{
				// TODO: I didn't want to mess with the verifier interface too much yet, but we don't want to throw exceptions
				// for every unsigned request.
			}

			yield return key.FediId;
		}

		IList<MastodonVerifier.MastodonSignatureComponents> GetMastodonSignatureComponents()
		{
			try
			{
				var mastodonSignatureComponentsList = mastodonVerifier
					.ParseMastodonSignatureComponents(request)
					.ToList();
				return mastodonSignatureComponentsList;
			}
			catch (VerifierException)
			{
				// TODO: This exception is thrown if no Mastodon signature headers are found.
				// This will be the case for any unsigned request, so we can expect to see quite many of them.
				// Might want to revisit the parsing code for a way to avoid that cost on this path.
			}

			return Array.Empty<MastodonVerifier.MastodonSignatureComponents>();
		}
	}

	private async IAsyncEnumerable<Uri> VerifyRfcSignature(AspNetCoreMessageSigningContext signingContext, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		foreach (var signatureContext in signingContext.SignaturesForVerification)
		{
			var signatureParams = signatureContext.SignatureParams;
			var key = await GetKey(signatureParams.KeyId, cancellationToken);
			if (key is null)
			{
				continue;
			}

			var verifier = GetVerifier(signatureParams.Algorithm, signatureParams.KeyId, key);
			var input = signingContext.GetSignatureInput(signatureParams, out _);
			var verificationResult = await verifier.VerifyAsync(
				signatureParams,
				input,
				signatureContext.Signature,
				cancellationToken);

			if (verificationResult is VerificationResult.SuccessfullyVerified)
			{
				_logger.LogInformation("HTTP request signature validation succeeded");
				yield return key.FediId;
			}
		}
	}

	private async Task<SigningKey?> GetKey(string? keyId, CancellationToken cancellationToken)
	{
		var key = keyId == null ? default : await verificationKeyProvider.GetKeyByIdAsync(keyId, cancellationToken);
		if (key == null)
		{
			_logger.LogWarning($"Unable to verify signature: key with ID {keyId} was not found");
		}
		return key;
	}

	private IVerifier GetVerifier(string? algorithm, string? keyId, SigningKey? key)
	{
		if (key == null)
		{
			throw new SignatureVerificationException($"No public key for {keyId}");
		}

		return new RsaPssSha512SignatureProvider(null, key.GetRsa(), keyId);
	}
}