using System.Security.Cryptography;

namespace Letterbook.Core.Models;

public class SigningKey
{
    public Guid? LocalId { get; set; }
    public int KeyOrder { get; set; }
    public string? Label { get; set; }
    public KeyFamily Family { get; set; }
    public ReadOnlyMemory<byte> PublicKey { get; set; }
    public ReadOnlyMemory<byte>? PrivateKey { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Expires { get; set; }
    public required Uri Id { get; set; }

    public static SigningKey Rsa(int keyOrder, Uri keyUri, string label = "System generated key")
    {
        using RSA keyPair = RSA.Create();
        return new SigningKey()
        {
            LocalId = Guid.NewGuid(),
            KeyOrder = keyOrder,
            Label = label,
            Family = KeyFamily.Rsa,
            PublicKey = keyPair.ExportSubjectPublicKeyInfo(),
            PrivateKey = keyPair.ExportPkcs8PrivateKey(),
            Created = DateTimeOffset.UtcNow,
            Expires = DateTimeOffset.MaxValue,
            Id = keyUri
        };
    }
    
    public static SigningKey Dsa(int keyOrder, Uri keyUri, string label = "System generated key")
    {
        using DSA keyPair = DSA.Create();
        return new SigningKey()
        {
            LocalId = Guid.NewGuid(),
            KeyOrder = keyOrder,
            Label = label,
            Family = KeyFamily.Dsa,
            PublicKey = keyPair.ExportSubjectPublicKeyInfo(),
            PrivateKey = keyPair.ExportPkcs8PrivateKey(),
            Created = DateTimeOffset.UtcNow,
            Expires = DateTimeOffset.MaxValue,
            Id = keyUri
        };
    }
    
    public static SigningKey EcDsa(int keyOrder, Uri keyUri, string label = "System generated key")
    {
        using ECDsa keyPair = ECDsa.Create(ECCurve.CreateFromFriendlyName("ed25519"));
        return new SigningKey()
        {
            LocalId = Guid.NewGuid(),
            KeyOrder = keyOrder,
            Label = label,
            Family = KeyFamily.EcDsa,
            PublicKey = keyPair.ExportSubjectPublicKeyInfo(),
            PrivateKey = keyPair.ExportPkcs8PrivateKey(),
            Created = DateTimeOffset.UtcNow,
            Expires = DateTimeOffset.MaxValue,
            Id = keyUri
        };
    }
    
    public RSA GetRsa()
    {
        RSA alg = OperatingSystem.IsWindows() ? new RSACng() : new RSAOpenSsl();
        if (PrivateKey is {} pk) alg.ImportPkcs8PrivateKey(pk.Span, out _);
        else alg.ImportSubjectPublicKeyInfo(PublicKey.Span, out _);
        return alg;
    }
    
    public DSA GetDsa()
    {
        DSA alg = OperatingSystem.IsWindows() ? new DSACng() : new DSAOpenSsl();
        if (PrivateKey is {} pk) alg.ImportPkcs8PrivateKey(pk.Span, out _);
        else alg.ImportSubjectPublicKeyInfo(PublicKey.Span, out _);
        return alg;
    }
    
    public ECDsa GetEcDsa()
    {
        ECDsa alg = OperatingSystem.IsWindows() ? new ECDsaCng() : new ECDsaOpenSsl();
        if (PrivateKey is {} pk) alg.ImportPkcs8PrivateKey(pk.Span, out _);
        else alg.ImportSubjectPublicKeyInfo(PublicKey.Span, out _);
        return alg;
    }
    
    public enum KeyFamily
    {
        Rsa,
        Dsa,
        EcDsa
    }
}