using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSign;
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

    public HttpRequestMessage SignRequest(Models.SigningKey signingKey, HttpRequestMessage message,
        string signatureId = "mastodon")
    {
        if (signingKey.Family != KeyFamily.Rsa)
        {
            _logger.LogWarning("");
            return message;
        }

        var checkInput = new InputCheckingVisitor(message);
        var builder = new MastodonComponentBuilder(message);
        var inputSpec = new SignatureInputSpec(signatureId);
        foreach (var spec in _options.ComponentsToInclude)
        {
            checkInput.Visit(spec.Component);
            if (spec.Mandatory || checkInput.Found)
                inputSpec.SignatureParameters.AddComponent(spec.Component);
        }

        inputSpec.SignatureParameters.KeyId = signingKey.KeyUri.ToString();
        inputSpec.SignatureParameters.Algorithm = Constants.SignatureAlgorithms.RsaPkcs15Sha256;
        builder.Visit(inputSpec.SignatureParameters);

        var signature = SignRsa(signingKey.GetRsa(), builder.SigningDocument);

        message.Headers.Add(Constants.Headers.SignatureInput, $"{signatureId}={builder.SigningDocumentSpec}");
        message.Headers.Add(Constants.Headers.Signature,
            $"keyId=\"{signingKey.KeyUri}\",headers=\"{builder.SigningDocumentSpec}\",signature=\"{Convert.ToBase64String(signature)}\"");

        return message;
    }

    private byte[] SignRsa(RSA rsa, string document)
    {
        return rsa.SignData(Encoding.ASCII.GetBytes(document), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}