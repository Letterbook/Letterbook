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
            .ForMember(dest => dest.Authority, opt => opt.MapFrom(src => src.Id!.Authority))
            .ForMember(dest => dest.Audiences, opt => opt.Ignore())
            .ForMember(dest => dest.LocalId, opt => opt.MapFrom((actor, profile) =>
            {
                // TODO: Mapper in DI so it has access to config
                return profile.Authority.ToString() == "letterbook.example" ? "999" : default;
            }));
        cfg.CreateMap<DTO.Link, Models.Profile>();
    }

    private static void ConfigureNote(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<DTO.Object, Note>()
            .IncludeBase<DTO.Object, IObjectRef>()
            .ForMember(dest => dest.Creators, opt =>
            {
                opt.UseDestinationValue();
                opt.MapFrom(src => src.AttributedTo);
            })
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Published))
            .ForMember(dest => dest.InReplyTo, opt => opt.Ignore())
            // .ForMember(dest => dest.InReplyTo, opt => opt.MapFrom(src => src.InReplyTo.FirstOrDefault()))
            .ForMember(dest => dest.Client, opt => opt.Ignore()) // TODO: take from Activity, somehow
            .ForMember(dest => dest.Visibility, opt => opt.Ignore()) // TODO: ugh, this will be complicated
            .ForMember(dest => dest.Replies, opt => opt.Ignore()) // TODO: List<> to (paged) Collection
            .ForMember(dest => dest.Mentions, opt => opt.Ignore()); // same
    }

    private static void ConfigureDtoResolvables(IMapperConfigurationExpression cfg)
    {
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
            .ForMember(dest => dest.Authority, opt => opt.MapFrom( src => src.Id!.Authority))
            .ForMember(dest => dest.LocalId, opt => opt.MapFrom((src, dest) =>
            {
                // TODO: Mapper in DI so it has access to config
                return dest.Authority.ToString() == "letterbook.example" ? "999" : default;
            }));
        cfg.CreateMap<DTO.Link, ObjectRef>()
            .ForMember(dest => dest.LocalId, opt => opt.Ignore());
    }
}