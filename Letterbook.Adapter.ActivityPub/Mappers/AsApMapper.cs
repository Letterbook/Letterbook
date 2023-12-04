using ActivityPub.Types.AS.Extended.Actor;
using AutoMapper;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public static class AsApMapper
{
    public static MapperConfiguration Config = new(cfg =>
    {
        ConfigureDtoResolvables(cfg);
        ConfigureProfile(cfg);
        ConfigureNote(cfg);
    });

    private static void ConfigureActor(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Models.Profile, ActorExtensions>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.PreferredUsername, opt => opt.MapFrom(src => src.Handle))
            .ForMember(dest => dest.Attachment, opt => opt.Ignore())
            .ForMember(dest => dest.Liked, opt => opt.Ignore())
            .ForMember(dest => dest.PublicKey,
                opt => opt.MapFrom<SigningKeyConverter, IList<SigningKey>>(src => src.Keys))
            .ForMember(dest => dest.Endpoints, opt => opt.Ignore())
            .ForMember(dest => dest.Streams, opt => opt.Ignore());
    }

    private static void ConfigureProfile(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<AsAp.Actor, Models.Profile>()
            .IncludeBase<AsAp.IResolvable, Models.Profile>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Authority, opt => opt.MapFrom(src => src.Id!.Authority))
            .ForMember(dest => dest.Handle, opt => opt.MapFrom(src => src.PreferredUsername))
            .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers != null ? src.Followers.Id : null))
            .ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.Following != null ? src.Following.Id : null))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.CustomFields, opt => opt.MapFrom(src => src.Attachment))
            .ForMember(dest => dest.Inbox, opt => opt.MapFrom(src => src.Inbox.Id))
            .ForMember(dest => dest.Outbox, opt => opt.MapFrom(src => src.Outbox.Id))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Keys, opt => opt.MapFrom(src => src.PublicKey))
            .AfterMap((_, profile) => profile.Updated = DateTime.UtcNow);

        cfg.CreateMap<AsAp.PublicKey, Models.SigningKey>(MemberList.None)
            .ForMember(dest => dest.PublicKey,
                opt => opt.ConvertUsing(PublicKeyConverter.Instance, key => key.PublicKeyPem));

        cfg.CreateMap<AsAp.PublicKey, IList<SigningKey>>(MemberList.None)
            .ConvertUsing(PublicKeyConverter.Instance);
        
        cfg.CreateMap<AsAp.Object, Models.Profile>()
            .IncludeBase<AsAp.IResolvable, Models.Profile>();

        cfg.CreateMap<AsAp.Link, Models.Profile>()
            .IncludeBase<AsAp.IResolvable, Models.Profile>();

        cfg.CreateMap<AsAp.IResolvable, Models.Profile>()
            .IncludeBase<AsAp.IResolvable, IObjectRef>()
            // Handle these on concrete types
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Handle, opt => opt.Ignore())
            .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
            .ForMember(dest => dest.FollowersCollection, opt => opt.Ignore())
            .ForMember(dest => dest.FollowingCollection, opt => opt.Ignore())
            .ForMember(dest => dest.Followers, opt => opt.Ignore())
            .ForMember(dest => dest.Following, opt => opt.Ignore())
            .ForMember(dest => dest.Inbox, opt => opt.Ignore())
            .ForMember(dest => dest.Outbox, opt => opt.Ignore())
            .ForMember(dest => dest.SharedInbox, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.CustomFields, opt => opt.Ignore())
            .ForMember(dest => dest.Keys, opt => opt.Ignore())
            // Really ignore these, they don't exist in AP
            .ForMember(dest => dest.Authority, opt => opt.Ignore())
            .ForMember(dest => dest.LocalId, opt => opt.Ignore())
            .ForMember(dest => dest.RelatedAccounts, opt => opt.Ignore())
            .ForMember(dest => dest.OwnedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.Audiences, opt => opt.Ignore())
            ;

        cfg.CreateMap<AsAp.IResolvable, CustomField>()
            .ForAllMembers(opts => opts.Ignore());
    }

    private static void ConfigureNote(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<AsAp.Object, Note>()
            .IncludeBase<AsAp.IResolvable, IObjectRef>()
            .IncludeBase<AsAp.Object, IObjectRef>()
            .ForMember(dest => dest.Creators, opt => opt.MapFrom(src => src.AttributedTo))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.InReplyTo, opt => opt.Ignore())
            .ForMember(dest => dest.LikedBy, opt => opt.Ignore())
            .ForMember(dest => dest.BoostedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore()) // TODO: take from Activity, somehow
            .ForMember(dest => dest.Visibility, opt => opt.Ignore()) // TODO: ugh, this will be complicated
            .ForMember(dest => dest.Replies, opt => opt.Ignore()) // TODO: List<> to (paged) Collection
            .ForMember(dest => dest.Mentions, opt => opt.Ignore()); // same
    }

    private static void ConfigureDtoResolvables(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<AsAp.IResolvable, IObjectRef>()
            .ForMember(dest => dest.LocalId, opt => opt.Ignore());
        cfg.CreateMap<AsAp.Object, IObjectRef>().ConstructUsing((src, context) =>
            {
                Enum.TryParse<ActivityObjectType>(src.Type, out var type);
                switch (type)
                {
                    case ActivityObjectType.Note:
                        return context.Mapper.Map<Note>(src);
                    case ActivityObjectType.Image:
                        return context.Mapper.Map<Image>(src);
                    case ActivityObjectType.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(src.Type), $"Unsupported Object type {src.Type}");
                }
            })
            .IncludeBase<AsAp.IResolvable, IObjectRef>()
            .ForMember(dest => dest.Authority, opt => opt.MapFrom(src => src.Id!.Authority));
        cfg.CreateMap<AsAp.Link, ObjectRef>()
            .IncludeBase<AsAp.IResolvable, IObjectRef>()
            .ForMember(dest => dest.LocalId, opt => opt.Ignore());
    }
}