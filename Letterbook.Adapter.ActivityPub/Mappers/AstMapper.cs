using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Util;
using AutoMapper;
using JetBrains.Annotations;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Models;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace Letterbook.Adapter.ActivityPub.Mappers;

/// <summary>
/// Map ActivityPubSharp objects to Model types 
/// </summary>
public static class AstMapper
{
    public static MapperConfiguration Default = new(cfg =>
    {
        ConfigureBaseTypes(cfg);
        FromActor(cfg);
    });

    private static void FromActor(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<PersonActorExtension, Models.Profile>(MemberList.Destination)
            .IncludeBase<ASType, IObjectRef>()
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.SharedInbox, opt => opt.Ignore())
            .ForMember(dest => dest.OwnedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Audiences, opt => opt.Ignore())
            .ForMember(dest => dest.RelatedAccounts, opt => opt.Ignore())
            .ForMember(dest => dest.FollowersCollection, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingCollection, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Handle, opt => opt.MapFrom(src => src.PreferredUsername))
            // .ForMember(dest => dest.CustomFields, opt => opt.MapFrom(src => src.Attachment))
            .ForMember(dest => dest.CustomFields, opt => opt.Ignore())
            .ForMember(dest => dest.Inbox, opt => opt.MapFrom(src => src.Inbox))
            .ForMember(dest => dest.Outbox, opt => opt.MapFrom(src => src.Outbox))
            .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Updated))
            .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers))
            .ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.Following))
            .ForMember(dest => dest.Keys, opt => opt.MapFrom<PublicKeyConverter, PublicKey?>(src => src.PublicKey!))
            .AfterMap((_, profile) =>
            {
                profile.Type = ActivityActorType.Person;
            });
        
        cfg.CreateMap<PublicKey?, SigningKey?>()
            .ConvertUsing<PublicKeyConverter>();
    }

    private static void FromNote(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<NoteObject, Post>(MemberList.Destination)
            .IncludeBase<ASType, IObjectRef>()
            // TODO (soon)
            // .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Creators, opt => opt.MapFrom(src => src.AttributedTo))
            .ForMember(dest => dest.InReplyTo, opt => opt.MapFrom(src => src.InReplyTo))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.AddressedTo, opt => opt.ConvertUsing<MentionsConverter, ASObject>())
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Generator))
            .ForMember(dest => dest.LikesCollection, opt => opt.MapFrom(src => src.Likes))
            .ForMember(dest => dest.SharesCollection, opt => opt.MapFrom(src => src.Shares))
            .ForMember(dest => dest.Audience, opt => opt.MapFrom(src => src.Audience));
    }

    private static void ConfigureBaseTypes(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<ASType, IObjectRef>(MemberList.Destination)
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Authority, opt => opt.Ignore())
            .ForMember(dest => dest.LocalId, opt => opt.Ignore());

        cfg.CreateMap<Linkable<ASObject>, Post>()
            .ConvertUsing<LinkableConverter>();
        
        cfg.CreateMap<LinkableList<ASObject>, Note>()
            .ConvertUsing<LinkableListPostConverter>();

        cfg.CreateMap<ASLink, Uri>()
            .ConvertUsing<ASLinkConverter>();

        cfg.CreateMap<string, ReadOnlyMemory<byte>>()
            .ConvertUsing<PublicKeyConverter>();
    }
}

internal class LinkableListPostConverter : ITypeConverter<LinkableList<ASObject>, Note>
{
    public Note Convert(LinkableList<ASObject> source, Note destination, ResolutionContext context)
    {
        throw new NotImplementedException();
    }
}

internal class LinkableConverter : ITypeConverter<Linkable<ASObject>, Post>
{
     public Post Convert(Linkable<ASObject> source, Post destination, ResolutionContext context)
     {
         throw new NotImplementedException();
         // return source.HasLink ? new Note(source.Link) : context.Mapper.Map<Note>(source.Value);
     }
}

[UsedImplicitly]
internal class MentionsConverter : IValueConverter<ASObject, ICollection<Mention>>
{
    public ICollection<Mention> Convert(ASObject sourceMember, ResolutionContext context)
    {
        throw new NotImplementedException();
    }
}

[UsedImplicitly]
internal class ASLinkConverter : ITypeConverter<ASLink, Uri>
{
    public Uri Convert(ASLink source, Uri destination, ResolutionContext context)
    {
        return source.HRef;
    }
}

[UsedImplicitly]
internal class PublicKeyConverter : 
    ITypeConverter<PublicKey?, SigningKey?>, 
    IMemberValueResolver<PersonActorExtension, Models.Profile, PublicKey?, IList<SigningKey>>, 
    ITypeConverter<string, ReadOnlyMemory<byte>>
{
    public SigningKey? Convert(PublicKey? source, SigningKey? destination, ResolutionContext context)
    {
        if (source is null) return default;
        using TextReader tr = new StringReader(source.PublicKeyPem);

        var reader = new PemReader(tr);
        var pemObject = reader.ReadObject();
        var alg = pemObject switch
        {
            RsaKeyParameters => SigningKey.KeyFamily.Rsa,
            DsaKeyParameters => SigningKey.KeyFamily.Dsa,
            ECKeyParameters => SigningKey.KeyFamily.EcDsa,
            _ => SigningKey.KeyFamily.Unknown
        };

        destination ??= new SigningKey() { Id = new Uri(source.Id) };

        destination.Id = new Uri(source.Id);
        destination.Label = "From federation peer";
        destination.PublicKey = context.Mapper.Map<ReadOnlyMemory<byte>>(source.PublicKeyPem);
        destination.Family = alg;
        destination.Created = DateTimeOffset.Now;

        return destination;
    }

    IList<SigningKey> IMemberValueResolver<PersonActorExtension, Models.Profile, PublicKey?, IList<SigningKey>>
        .Resolve(PersonActorExtension source, Models.Profile destination, PublicKey? sourceMember, 
            IList<SigningKey>? destMember, ResolutionContext context)
    {
        var key = context.Mapper.Map<SigningKey>(sourceMember);
        destMember ??= new List<SigningKey>();
        if(key is not null) destMember.Add(key);

        return destMember;
    }

    ReadOnlyMemory<byte> ITypeConverter<string, ReadOnlyMemory<byte>>
        .Convert(string source, ReadOnlyMemory<byte> destination, ResolutionContext context)
    {
        var b64 = string.Join('\n',
            source.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Skip(1)
                .SkipLast(1));
        return System.Convert.FromBase64String(b64);
    }
}