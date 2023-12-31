using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSign;
using static NSign.Constants;
using NSign.Signatures;
using static Letterbook.Core.Models.SigningKey;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public class MastodonSigner : IClientSigner
{
    private readonly ILogger<MastodonSigner> _logger;
    private readonly MessageSigningOptions _options;

    public MastodonSigner(ILogger<MastodonSigner> logger, IOptionsSnapshot<MessageSigningOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public HttpRequestMessage SignRequest(HttpRequestMessage message,
        Models.SigningKey signingKey)
    {
        if (signingKey.Family != KeyFamily.Rsa)
        {
            _logger.LogWarning("");
            return message;
        }

        var checkInput = new InputCheckingVisitor(message);
        var builder = new MastodonComponentBuilder(message);
        var inputSpec = new SignatureInputSpec("mastodon");
        foreach (var spec in _options.ComponentsToInclude)
        {
            spec.Component.Accept(checkInput);
            if (spec.Mandatory || checkInput.Found)
                inputSpec.SignatureParameters.AddComponent(spec.Component);
        }

        inputSpec.SignatureParameters.KeyId = signingKey.Id.ToString();
        inputSpec.SignatureParameters.Algorithm = SignatureAlgorithms.RsaPkcs15Sha256;
        builder.Visit(inputSpec.SignatureParameters);

        var signature = SignRsa(signingKey.GetRsa(), builder.SigningDocument);

        var headerValue =
            $"keyId=\"{signingKey.Id}\",headers=\"{builder.SigningDocumentSpec}\",signature=\"{Convert.ToBase64String(signature)}\"";
        message.Headers.Add(Headers.SignatureInput, $"mastodon={builder.SigningDocumentSpec}");
        message.Headers.Add(Headers.Signature, headerValue);
        
        _logger.LogDebug("Signed from {Spec} over {Document}", builder.SigningDocumentSpec, builder.SigningDocument);
        _logger.LogDebug("Signature {Signature}", signature);
        _logger.LogDebug("Signature Header {Header}", headerValue);

        return message;
    }

    private byte[] SignRsa(RSA rsa, string document)
    {
        return rsa.SignData(Encoding.ASCII.GetBytes(document), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}