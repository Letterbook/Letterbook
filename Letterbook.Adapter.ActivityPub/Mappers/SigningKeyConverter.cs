using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Adapter.ActivityPub.Types;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public class SigningKeyConverter :
    ITypeConverter<Models.SigningKey, AsAp.PublicKey?>,
    ITypeConverter<IList<Models.SigningKey>, AsAp.PublicKey?>,
    IMemberValueResolver<Models.Profile, AsAp.Actor, IList<Models.SigningKey>, AsAp.PublicKey?>,
    IMemberValueResolver<Models.Profile, PersonActorExtension, IList<Models.SigningKey>, PublicKey?>,
    ITypeConverter<IList<Models.SigningKey>, PublicKey?>,
    ITypeConverter<Models.SigningKey, PublicKey?>
{
    public AsAp.PublicKey Convert(Models.SigningKey source, AsAp.PublicKey? destination, ResolutionContext context)
    {
        destination ??= new AsAp.PublicKey() { Id = CompactIri.FromUri(source.Id) };
        destination.Id = CompactIri.FromUri(source.Id);
        var pem = source.Family switch
        {
            Models.SigningKey.KeyFamily.Rsa => source.GetRsa().ExportSubjectPublicKeyInfoPem(),
            Models.SigningKey.KeyFamily.Dsa => source.GetDsa().ExportSubjectPublicKeyInfoPem(),
            Models.SigningKey.KeyFamily.EcDsa => source.GetEcDsa().ExportSubjectPublicKeyInfoPem(),
            _ => null
        };
        if (pem == null)
        {
            return default!;
        }
        destination.PublicKeyPem = pem;
        return destination;
    }

    public AsAp.PublicKey Convert(IList<Models.SigningKey> source, AsAp.PublicKey? destination,
        ResolutionContext context)
    {
        var signingKey = source.OrderBy(key => key.KeyOrder)
            .FirstOrDefault(key => key.Family == Models.SigningKey.KeyFamily.Rsa);
        if (signingKey == null)
        {
            return default!;
        }

        destination = context.Mapper.Map<AsAp.PublicKey>(signingKey);
        return destination;
    }

    public AsAp.PublicKey Resolve(Models.Profile source, AsAp.Actor destination, IList<Models.SigningKey> sourceMember,
        AsAp.PublicKey? destMember, ResolutionContext context)
    {
        destMember = context.Mapper.Map<AsAp.PublicKey>(sourceMember);
        destMember.Owner = context.Mapper.Map<AsAp.Link>(source.Id);
        return destMember;
    }

    // TODO(now): for APSharp
    public PublicKey Resolve(Models.Profile source, PersonActorExtension destination, IList<Models.SigningKey> sourceMember, PublicKey? destMember,
        ResolutionContext context)
    {
        destMember = context.Mapper.Map<PublicKey>(sourceMember);
        return destMember;
    }

    public PublicKey? Convert(IList<Models.SigningKey> source, PublicKey? destination, ResolutionContext context)
    {
        var signingKey = source.OrderBy(key => key.KeyOrder)
            .FirstOrDefault(key => key.Family == Models.SigningKey.KeyFamily.Rsa);
        if (signingKey == null)
        {
            return default!;
        }

        destination = context.Mapper.Map<PublicKey>(signingKey);
        return destination;
    }

    public PublicKey? Convert(Models.SigningKey source, PublicKey? destination, ResolutionContext context)
    {
        // Null-override is safe, because both properties are set here.
        destination ??= new PublicKey
        {
            Id = null!,
            PublicKeyPem = null!
        };
        
        destination.Id = source.Id.ToString();
        var pem = source.Family switch
        {
            Models.SigningKey.KeyFamily.Rsa => source.GetRsa().ExportSubjectPublicKeyInfoPem(),
            Models.SigningKey.KeyFamily.Dsa => source.GetDsa().ExportSubjectPublicKeyInfoPem(),
            Models.SigningKey.KeyFamily.EcDsa => source.GetEcDsa().ExportSubjectPublicKeyInfoPem(),
            _ => null
        };
        if (pem == null)
        {
            return default!;
        }
        destination.PublicKeyPem = pem;
        return destination;
    }
}