using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Letterbook.Adapter.ActivityPub.Exceptions;
using Microsoft.Extensions.Logging;
using NSign;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public partial class MastodonVerifier : ISignatureVerifier, ISignatureParser
{
    private HashSet<string> DerivedComponents = new()
    {
        Constants.DerivedComponents.Authority,
        Constants.DerivedComponents.Status,
        Constants.DerivedComponents.RequestTarget,
        Constants.DerivedComponents.TargetUri,
        Constants.DerivedComponents.Path,
        Constants.DerivedComponents.Method,
        Constants.DerivedComponents.Query,
        Constants.DerivedComponents.Scheme,
        Constants.DerivedComponents.QueryParam,
        Constants.DerivedComponents.SignatureParams
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
        var builder = new MastodonComponentBuilder(message);
        var components = ParseMastodonSignatureComponents(message);
        var result = VerificationResult.NoMatchingVerifierFound;
        foreach (var parsed in components)
        {
            if (parsed.keyId != verificationKey.KeyUri.ToString()) continue;
            if (VerifySignature(parsed, verificationKey, builder)) return VerificationResult.SuccessfullyVerified;
            result = VerificationResult.SignatureMismatch;
        }

        return result;
    }

    public IEnumerable<SignatureInputSpec> ParseRequestSignature(HttpRequestMessage message) =>
        ParseMastodonSignatureComponents(message).Select(v => v.spec);

    public IEnumerable<MastodonSignatureComponents> ParseMastodonSignatureComponents(HttpRequestMessage message)
    {
        if (!message.Headers.TryGetValues(Constants.Headers.Signature, out var values))
            throw VerifierException.NoSignatures();

        var mastodonSignatures = values
            .Select(header => header.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Where(parts => parts.Length > 1);

        if (!mastodonSignatures.Any()) throw VerifierException.NoValidSignatures(message.Headers);

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
                    components.keyId = pair.Value.Trim('"');
                    break;
                case "signature":
                    components.signature = pair.Value.Trim('"');
                    break;
                case "headers":
                    components.spec = ParseSpec(pair.Value);
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

        SignatureInputSpec ParseSpec(string headersString)
        {
            _logger.LogDebug("Parsing Mastodon signature headers '{Headers}'", headersString);
            var spec = new SignatureInputSpec("spec");
            var match = DerivedComponentsRegex().Match(headersString);
            if (match.Success)
            {
                foreach (var token in match.Value.Split(new []{' ', '(', ')'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    spec.SignatureParameters.AddComponent(new DerivedComponent("@" + token));
                }
            }

            var comps = headersString
                .Substring(match.Length + 1)
                .Split(new []{' ', '"'}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
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
            return spec;
        }
    }

    private bool VerifySignature(MastodonSignatureComponents components, Models.SigningKey verificationKey,
        MastodonComponentBuilder builder)
    {
        var algorithm = verificationKey.GetRsa();
        builder.Visit(components.spec.SignatureParameters);
        return algorithm.VerifyData(Encoding.ASCII.GetBytes(builder.SigningDocument),
            Convert.FromBase64String(components.signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public struct MastodonSignatureComponents
    {
        internal SignatureInputSpec spec;
        internal string keyId;
        internal string signature;
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