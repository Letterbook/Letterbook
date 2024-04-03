using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using NSign;
using NSign.Providers;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

[UsedImplicitly]
public class FederatedActorSignatureVerifier(IKeyMaterialProvider keyMaterialProvider) : IVerifier
{
	public async Task<VerificationResult> VerifyAsync(
		SignatureParamsComponent signatureParams,
		ReadOnlyMemory<byte> input,
		ReadOnlyMemory<byte> expectedSignature,
		CancellationToken cancellationToken)
	{
		var key = await GetKey(signatureParams);

		var verifier = GetVerifier(signatureParams.Algorithm, signatureParams.KeyId, key);

		return await verifier.VerifyAsync(signatureParams, input, expectedSignature, cancellationToken);
	}

	private async Task<X509Certificate2> GetKey(SignatureParamsComponent signatureParams)
	{
		var key = await keyMaterialProvider.GetKeyByIdAsync(signatureParams.KeyId);
		if (key == null)
		{
			throw new SignatureVerificationException(
				$"Unable to verify signature: key with ID {signatureParams.KeyId} was not found");
		}
		return key;
	}

	private IVerifier GetVerifier(string? algorithm, string? keyId, X509Certificate2 key)
	{
		var publicKey = key.GetRSAPublicKey();
		if (publicKey == null)
		{
			throw new SignatureVerificationException($"No RSA public key for {keyId}");
		}

		return new RsaPssSha512SignatureProvider(null, publicKey, keyId);
	}
}

public class SignatureVerificationException : Exception
{
	public SignatureVerificationException() { }

	public SignatureVerificationException(string? message)
		: base(message) { }

	public SignatureVerificationException(string? message, Exception? innerException)
		: base(message, innerException) { }
}