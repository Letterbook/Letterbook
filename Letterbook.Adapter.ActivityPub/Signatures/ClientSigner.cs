using System.Security.Cryptography;
using Letterbook.Adapter.ActivityPub.Exceptions;
using NSign;
using NSign.Providers;
using NSign.Signatures;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public sealed class ClientSigner : ISigner
{
    private readonly IKeyContainer _keyContainer;
    private readonly byte[] _signingKey;
    private readonly string _keyId;
    private readonly ISigner _innerSigner;

    public ClientSigner(IKeyContainer keyContainer)
    {
        _keyContainer = keyContainer;

        if (!_keyContainer.TryGetKey(out var key)) throw ClientException.SignatureError();
        
        RSA rsa = OperatingSystem.IsWindows() ? new RSACng() : new RSAOpenSsl();
        rsa.ImportRSAPrivateKey((key.PrivateKey ?? throw ClientException.SignatureError(key.Id, key.Label)).Span, out _);
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