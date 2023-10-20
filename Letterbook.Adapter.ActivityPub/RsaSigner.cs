using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using NSign;
using NSign.Providers;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub;

public sealed class RsaSigner : ISigner
{
    private readonly byte[] _signingKey;
    private readonly string _keyId;
    private readonly ISigner _innerSigner;

    public RsaSigner(byte[] signingKey, string keyId)
    {
        _signingKey = signingKey;
        _keyId = keyId;
        
        RSA rsa = OperatingSystem.IsWindows() ? new RSACng() : new RSAOpenSsl();
        rsa.ImportRSAPrivateKey(_signingKey, out _);
        _innerSigner = new RsaPkcs15Sha256SignatureProvider(rsa, rsa, _keyId);
    }

    public Task<ReadOnlyMemory<byte>> SignAsync(ReadOnlyMemory<byte> input, CancellationToken cancellationToken)
    {
        return _innerSigner.SignAsync(input, cancellationToken);
    }

    public void UpdateSignatureParams(SignatureParamsComponent signatureParams)
    {
        _innerSigner.UpdateSignatureParams(signatureParams);
    }
}