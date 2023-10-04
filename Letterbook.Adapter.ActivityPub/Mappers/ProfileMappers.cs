using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public static class ProfileMappers
{
    // Maps full property values/objects and Collections as Links
    public static MapperConfiguration DefaultConfig = new(cfg =>
    {
        ConfigureDefaultCollections(cfg);
    });

    public static MapperConfiguration DefaultProfile = new(cfg =>
    {
        ConfigureCommonTypes(cfg);
        ConfigureDefaultCollections(cfg);

        cfg.CreateMap<Models.Profile, AsAp.Actor>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Handle))
            .ForMember(dest => dest.Attachment, opt => opt.Ignore());
    });

    public static void ConfigureDefaultCollections(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Models.Profile, AsAp.Actor>()
            .ForMember(dest => dest.Followers,
                opt => opt.MapFrom<DefaultOrderedCollectionResolver, ObjectCollection<FollowerRelation>>(src => src.Followers))
            .ForMember(dest => dest.Following,
                opt => opt.MapFrom<DefaultOrderedCollectionResolver, ObjectCollection<FollowerRelation>>(src => src.Following));
    }

    public static void ConfigureCommonTypes(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Uri, CompactIri>()
            .ConstructUsing((uri) => CompactIri.FromUri(uri));
    }
}