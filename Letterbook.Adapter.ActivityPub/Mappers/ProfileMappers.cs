using System.Globalization;
using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public static class ProfileMappers
{
    // Properties: include all values
    // Collections: ID only
    public static MapperConfiguration DefaultProfile = new(cfg =>
    {
        ConfigureCommonTypes(cfg);
        ConfigureBaseObject(cfg);
        ConfigureSigningKeys(cfg);

        cfg.CreateMap<Models.Profile, AsAp.Actor>(MemberList.Destination)
            .IncludeBase<Models.Profile, AsAp.Object>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Handle))
            .ForMember(dest => dest.Attachment, opt => opt.Ignore())
            .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.FollowersCollection))
            .ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.FollowingCollection))
            .ForMember(dest => dest.Liked, opt => opt.Ignore())
            .ForMember(dest => dest.PublicKey, opt => opt.MapFrom<SigningKeyConverter, IList<SigningKey>>(src => src.Keys))
            .ForMember(dest => dest.Endpoints, opt => opt.Ignore())
            .ForMember(dest => dest.Streams, opt => opt.Ignore());
    });

    public static void ConfigureSigningKeys(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<IList<SigningKey>, AsAp.PublicKey?>()
            .ConvertUsing<SigningKeyConverter>();
        
        cfg.CreateMap<SigningKey, AsAp.PublicKey?>()
            .ConvertUsing<SigningKeyConverter>();
    }
    
    public static void ConfigureCommonTypes(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<ObjectCollection<FollowerRelation>, AsAp.Collection>()
            .ConvertUsing<DefaultObjectCollectionResolver>();
        
        cfg.CreateMap<Uri, CompactIri?>()
            .ConstructUsing((uri) => CompactIri.FromUri(uri));

        cfg.CreateMap<Uri, AsAp.Link>(MemberList.None)
            .ConstructUsing(uri => new AsAp.Link(uri.ToString()));

        cfg.CreateMap<Uri, AsAp.Collection>(MemberList.None)
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src));

        // cfg.CreateMap<string, AsAp.ContentMap>()
            // .ConstructUsing(s => new AsAp.ContentMap(CultureInfo.InvariantCulture.Name){} );
    }

    public static void ConfigureBaseObject(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Models.Profile, AsAp.Object>()
            .ForAllMembers(opt => opt.Ignore());
    }
}