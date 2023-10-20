using System.Security.Cryptography;

namespace Letterbook.Core.Models;

public class SigningKey
{
    public Guid Id { get; set; }
    public int KeyOrder { get; set; }
    public string Label { get; set; }
    public KeyFamily Family { get; set; }
    public ReadOnlyMemory<byte> PublicKey { get; set; }
    public ReadOnlyMemory<byte>? PrivateKey { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset Expires { get; set; }

    public static SigningKey Rsa(int keyOrder, string label = "System generated key")
    {
        using RSA keyPair = RSA.Create();
        return new SigningKey()
        {
            Id = Guid.NewGuid(),
            KeyOrder = keyOrder,
            Label = label,
            Family = KeyFamily.Rsa,
            PublicKey = keyPair.ExportRSAPublicKey(),
            PrivateKey = keyPair.ExportRSAPrivateKey(),
            Created = DateTimeOffset.UtcNow,
            Expires = DateTimeOffset.MaxValue
        };
    }
    
    public static SigningKey Dsa(int keyOrder, string label = "System generated key")
    {
        using DSA keyPair = DSA.Create();
        return new SigningKey()
        {
            Id = Guid.NewGuid(),
            KeyOrder = keyOrder,
            Label = label,
            Family = KeyFamily.Dsa,
            PublicKey = keyPair.ExportPkcs8PrivateKey(),
            PrivateKey = keyPair.ExportSubjectPublicKeyInfo(),
            Created = DateTimeOffset.UtcNow,
            Expires = DateTimeOffset.MaxValue
        };
    }
    
    public static SigningKey Hmac(int keyOrder, string label = "System generated key")
    {
        using HMAC key = new HMACSHA256();
        return new SigningKey()
        {
            Id = Guid.NewGuid(),
            KeyOrder = keyOrder,
            Label = label,
            Family = KeyFamily.Hmac,
            PublicKey = key.Key,
            PrivateKey = key.Key,
            Created = DateTimeOffset.UtcNow,
            Expires = DateTimeOffset.MaxValue
        };
    }
    
    public enum KeyFamily
    {
        Rsa,
        Dsa,
        Hmac
    }
}