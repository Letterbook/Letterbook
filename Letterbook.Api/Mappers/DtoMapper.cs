using AutoMapper;
using Letterbook.Core.Models;

namespace Letterbook.Core.Mappers;

public static class DtoMapper
{
    public static MapperConfiguration Config = new(cfg =>
    {
        ConfigureDtoResolvables(cfg);
        ConfigureProfile(cfg);
        ConfigureNote(cfg);
    });

    private static void ConfigureProfile(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<DTO.Actor, Models.Profile>()
            .IncludeBase<DTO.IResolvable, Models.Profile>()
            .ForMember(dest => dest.Authority, opt => opt.MapFrom(src => src.Id!.Authority))
            .ForMember(dest => dest.Handle, opt => opt.Ignore())
            .ForMember(dest => dest.FollowersCollection, opt => opt.MapFrom(src => src.Followers))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CustomFields, opt => opt.MapFrom(src => src.Attachment))
            .ForMember(dest => dest.Inbox, opt => opt.MapFrom(src => src.Inbox.Id))
            .ForMember(dest => dest.Outbox, opt => opt.MapFrom(src => src.Outbox.Id))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
            .AfterMap((_, profile) => profile.Updated = DateTime.UtcNow);
        
        cfg.CreateMap<DTO.Object, Models.Profile>()
            .IncludeBase<DTO.IResolvable, Models.Profile>();
        
        cfg.CreateMap<DTO.Link, Models.Profile>()
            .IncludeBase<DTO.IResolvable, Models.Profile>();
        
        cfg.CreateMap<DTO.IResolvable, Models.Profile>()
            .IncludeBase<DTO.IResolvable, IObjectRef>()
            // Handle these on concrete types
            .ForMember(dest => dest.Type, opt => opt.Ignore())
            .ForMember(dest => dest.Handle, opt => opt.Ignore())
            .ForMember(dest => dest.DisplayName, opt => opt.Ignore())
            .ForMember(dest => dest.FollowersCollection, opt => opt.Ignore())
            .ForMember(dest => dest.Following, opt => opt.Ignore())
            .ForMember(dest => dest.Inbox, opt => opt.Ignore())
            .ForMember(dest => dest.Outbox, opt => opt.Ignore())
            .ForMember(dest => dest.SharedInbox, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore())
            .ForMember(dest => dest.CustomFields, opt => opt.Ignore())
            // Really ignore these, they don't exist in AP
            .ForMember(dest => dest.Authority, opt => opt.Ignore())
            .ForMember(dest => dest.LocalId, opt => opt.Ignore())
            .ForMember(dest => dest.RelatedAccounts, opt => opt.Ignore())
            .ForMember(dest => dest.OwnedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Updated, opt => opt.Ignore())
            .ForMember(dest => dest.Audiences, opt => opt.Ignore());

        cfg.CreateMap<DTO.IResolvable, CustomField>()
            .ForAllMembers(opts => opts.Ignore());
    }

    private static void ConfigureNote(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<DTO.Object, Note>()
            .IncludeBase<DTO.IResolvable, IObjectRef>()
            .IncludeBase<DTO.Object, IObjectRef>()
            .ForMember(dest => dest.Creators, opt => opt.MapFrom(src => src.AttributedTo))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.InReplyTo, opt => opt.Ignore())
            .ForMember(dest => dest.LikedBy, opt => opt.Ignore())
            .ForMember(dest => dest.BoostedBy, opt => opt.Ignore())
            // .ForMember(dest => dest.InReplyTo, opt => opt.MapFrom(src => src.InReplyTo.FirstOrDefault()))
            .ForMember(dest => dest.Client, opt => opt.Ignore()) // TODO: take from Activity, somehow
            .ForMember(dest => dest.Visibility, opt => opt.Ignore()) // TODO: ugh, this will be complicated
            .ForMember(dest => dest.Replies, opt => opt.Ignore()) // TODO: List<> to (paged) Collection
            .ForMember(dest => dest.Mentions, opt => opt.Ignore()); // same
    }

    private static void ConfigureDtoResolvables(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<DTO.IResolvable, IObjectRef>()
            .ForMember(dest => dest.LocalId, opt => opt.Ignore());
        cfg.CreateMap<DTO.Object, IObjectRef>().ConstructUsing((src, context) =>
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
            .IncludeBase<DTO.IResolvable, IObjectRef>()
            .ForMember(dest => dest.Authority, opt => opt.MapFrom(src => src.Id!.Authority));
        cfg.CreateMap<DTO.Link, ObjectRef>()
            .IncludeBase<DTO.IResolvable, IObjectRef>()
            .ForMember(dest => dest.LocalId, opt => opt.Ignore());
    }
}