using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSign;
using static NSign.Constants;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public partial class MastodonVerifier : ISignatureVerifier, ISignatureParser
{
	private readonly HashSet<string> DerivedComponents = new()
	{
		NSign.Constants.DerivedComponents.Authority,
		NSign.Constants.DerivedComponents.Status,
		NSign.Constants.DerivedComponents.RequestTarget,
		NSign.Constants.DerivedComponents.TargetUri,
		NSign.Constants.DerivedComponents.Path,
		NSign.Constants.DerivedComponents.Method,
		NSign.Constants.DerivedComponents.Query,
		NSign.Constants.DerivedComponents.Scheme,
		NSign.Constants.DerivedComponents.QueryParam,
		NSign.Constants.DerivedComponents.SignatureParams
	};

	[GeneratedRegex(@"\(.*\)")]
	private static partial Regex DerivedComponentsRegex();

	private ILogger<MastodonVerifier> _logger;

	public MastodonVerifier(ILogger<MastodonVerifier> logger)
	{
		_logger = logger;
	}

	public VerificationResult VerifyRequestSignature(HttpRequestMessage message, Models.SigningKey verificationKey)
	{
		return VerifyRequestSignature(new HttpRequestMessageComponentProvider(message), verificationKey);
	}
	public VerificationResult VerifyRequestSignature(HttpRequest request, Models.SigningKey verificationKey)
	{
		return VerifyRequestSignature(new HttpRequestComponentProvider(request), verificationKey);
	}

	private VerificationResult VerifyRequestSignature(RequestComponentProvider componentProvider, Models.SigningKey verificationKey)
	{
		var builder = new MastodonComponentBuilder(componentProvider);
		var components = ParseMastodonSignatureComponents(componentProvider);
		var result = VerificationResult.NoMatchingVerifierFound;
		foreach (var parsed in components)
		{
			if (!Uri.TryCreate(parsed.KeyId, UriKind.Absolute, out Uri? keyId)) continue;
			if (keyId != verificationKey.FediId) continue;
			if (VerifySignature(parsed, verificationKey, builder)) return VerificationResult.SuccessfullyVerified;
			result = VerificationResult.SignatureMismatch;
		}

		return result;
	}

	public IEnumerable<SignatureInputSpec> ParseRequestSignature(HttpRequestMessage message) =>
		ParseMastodonSignatureComponents(message).Select(v => v.Spec);

	public IEnumerable<MastodonSignatureComponents> ParseMastodonSignatureComponents(HttpRequestMessage requestMessage)
	{
		return ParseMastodonSignatureComponents(new HttpRequestMessageComponentProvider(requestMessage));
	}

	public IEnumerable<MastodonSignatureComponents> ParseMastodonSignatureComponents(HttpRequest request)
	{
		return ParseMastodonSignatureComponents(new HttpRequestComponentProvider(request));
	}

	private IEnumerable<MastodonSignatureComponents> ParseMastodonSignatureComponents(RequestComponentProvider componentProvider)
	{
		if (!componentProvider.TryGetHeaderValues(Headers.Signature, out var values))
			throw VerifierException.NoSignatures();

		var mastodonSignatures = values!
			.Select(header => header.Split(',', StringSplitOptions.RemoveEmptyEntries))
			.Where(parts => parts.Length > 1);

		if (!mastodonSignatures.Any()) throw VerifierException.NoValidSignatures(componentProvider);

		return mastodonSignatures.Select(ParseSignatureValue);
	}

	private MastodonSignatureComponents ParseSignatureValue(IEnumerable<string> parts)
	{
		var pairs = parts.Select(ParsePart);
		var components = new MastodonSignatureComponents();
		foreach (var pair in pairs)
		{
			switch (pair.Key)
			{
				case "keyId":
					components.KeyId = pair.Value.Trim('"');
					break;
				case "signature":
					components.Signature = pair.Value.Trim('"');
					break;
				case "headers":
					ParseSpec(pair.Value, components.Spec);
					break;
				case "algorithm":
					ParseAlg(pair.Value, components.Spec);
					break;
				default:
					_logger.LogWarning(
						"Unknown component {Component} in apparently Mastodon-compatible HTTP signature {Parts}",
						pair.Key, parts);
					break;
			}
		}

		return components;

		KeyValuePair<string, string> ParsePart(string part)
		{
			var innerParts = part.Split('=', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			return new KeyValuePair<string, string>(innerParts[0], innerParts[1]);
		}
		// keyId="https://my-example.com/actor#main-key",headers="(request-target) host date",signature="Y2FiYW...IxNGRiZDk4ZA=="

		void ParseSpec(string headersString, SignatureInputSpec spec)
		{
			_logger.LogDebug("Parsing Mastodon signature headers '{Headers}'", headersString);
			var match = DerivedComponentsRegex().Match(headersString);
			if (match.Success)
			{
				foreach (var token in match.Value.Split(new[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries))
				{
					spec.SignatureParameters.AddComponent(new DerivedComponent("@" + token));
				}
			}

			var comps = headersString
				.Substring(match.Length + 1)
				.Split(new[] { ' ', '"' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Select<string, SignatureComponent>(s =>
				{
					if (DerivedComponents.Contains(s)) return new DerivedComponent(s);
					if (DerivedComponents.Contains("@" + s)) return new DerivedComponent("@" + s);
					return new HttpHeaderComponent(s);
				});
			foreach (var component in comps)
			{
				spec.SignatureParameters.AddComponent(component);
			}

			_logger.LogDebug("Parsed Mastodon signature headers as {@Spec}", spec);
		}
	}

	private void ParseAlg(string alg, SignatureInputSpec spec)
	{
		switch (alg.Trim('"'))
		{
			case "rsa-sha256":
				spec.SignatureParameters.Algorithm = SignatureAlgorithms.RsaPkcs15Sha256;
				break;
			case SignatureAlgorithms.EcdsaP256Sha256:
			case SignatureAlgorithms.EcdsaP384Sha384:
			case SignatureAlgorithms.RsaPssSha512:
			case SignatureAlgorithms.RsaPkcs15Sha256:
				spec.SignatureParameters.Algorithm = alg;
				break;
			default:
				_logger.LogWarning("Unrecognized signature algorithm {Algorithm} in mastodon-compatible signature", alg);
				spec.SignatureParameters.Algorithm = SignatureAlgorithms.RsaPkcs15Sha256;
				break;
		}
	}

	private bool VerifySignature(MastodonSignatureComponents components, Models.SigningKey verificationKey,
		MastodonComponentBuilder builder)
	{
		if (components.Signature == null)
		{
			return false;
		}

		var algorithm = verificationKey.GetRsa();
		builder.Visit(components.Spec.SignatureParameters);
		return algorithm.VerifyData(Encoding.ASCII.GetBytes(builder.SigningDocument),
			// TODO: support other algorithms
			Convert.FromBase64String(components.Signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
	}

	public class MastodonSignatureComponents
	{
		public SignatureInputSpec Spec { get; set; } = new SignatureInputSpec("spec");
		public string? KeyId { get; set; }
		public string? Signature { get; set; }
	}
}

public interface ISignatureVerifier
{
	public VerificationResult VerifyRequestSignature(HttpRequestMessage message, Models.SigningKey verificationKey);
}

public interface ISignatureParser
{
	public IEnumerable<SignatureInputSpec> ParseRequestSignature(HttpRequestMessage message);
}