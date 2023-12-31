using AutoMapper;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class PublicKeyConverter :
    IValueConverter<string, ReadOnlyMemory<byte>>,
    ITypeConverter<AsAp.PublicKey, Models.SigningKey>,
    ITypeConverter<AsAp.PublicKey, IList<Models.SigningKey>>
{
    private static readonly Lazy<PublicKeyConverter> Lazy = new();
    public static PublicKeyConverter Instance => Lazy.Value;

    public ReadOnlyMemory<byte> Convert(string sourceMember, ResolutionContext context)
    {
        var b64 = string.Join('\n',
            sourceMember.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Skip(1)
                .SkipLast(1));
        return System.Convert.FromBase64String(b64);
    }

    public IList<Models.SigningKey> Convert(AsAp.PublicKey source, IList<Models.SigningKey> destination,
        ResolutionContext context)
    {
        destination ??= new List<Models.SigningKey>();
        destination.Add(context.Mapper.Map<AsAp.PublicKey, Models.SigningKey>(source));
        return destination;
    }

    public Models.SigningKey Convert(AsAp.PublicKey source, Models.SigningKey destination, ResolutionContext context)
    {
        using TextReader tr = new StringReader(source.PublicKeyPem);

        var reader = new PemReader(tr);
        var pemObject = reader.ReadObject();
        var alg = pemObject switch
        {
            RsaKeyParameters => Models.SigningKey.KeyFamily.Rsa,
            DsaKeyParameters => Models.SigningKey.KeyFamily.Dsa,
            ECKeyParameters => Models.SigningKey.KeyFamily.EcDsa,
            _ => Models.SigningKey.KeyFamily.Unknown
        };

        destination ??= new Models.SigningKey() { Id = source.Id! };

        destination.Id = source.Id!;
        destination.Label = "From federation peer";
        destination.PublicKey = context.Mapper.Map<ReadOnlyMemory<byte>>(source.PublicKeyPem);
        destination.Family = alg;
        destination.Created = DateTimeOffset.Now;

        return destination;
    }
}