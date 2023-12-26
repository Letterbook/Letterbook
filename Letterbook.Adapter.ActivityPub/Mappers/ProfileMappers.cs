using AutoMapper;
using Letterbook.ActivityPub;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.ActivityPub.Mappers;

public static class ProfileMappers
{
    /// <summary>
    /// Includes all expected properties. Collections have an ID, but no items
    /// </summary>
    public static MapperConfiguration DefaultProfile = new(cfg =>
    {
        ConfigureCommonTypes(cfg);
        ConfigureSigningKeys(cfg);

        cfg.CreateMap<Models.Profile, AsAp.Actor>(MemberList.Destination)
            .IncludeBase<Models.Profile, AsAp.Object>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.PreferredUsername, opt => opt.MapFrom(src => src.Handle))
            .ForMember(dest => dest.Attachment, opt => opt.Ignore())
            .ForMember(dest => dest.Liked, opt => opt.Ignore())
            .ForMember(dest => dest.PublicKey,
                opt => opt.MapFrom<SigningKeyConverter, IList<SigningKey>>(src => src.Keys))
            .ForMember(dest => dest.Endpoints, opt => opt.Ignore())
            .ForMember(dest => dest.Streams, opt => opt.Ignore())
            .AfterMap((_, actor) =>
            {
                actor.LdContext = AsAp.LdContext.SupportedContexts;
            });
    });

    public static MapperConfiguration DefaultLink = new(cfg =>
    {
        ConfigureCommonTypes(cfg);
        
        cfg.CreateMap<Models.IObjectRef, AsAp.Link>()
            .ConstructUsing(obj => new AsAp.Link(CompactIri.FromUri(obj.Id)));
            
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
        cfg.CreateMap<ActivityActorType, string>()
            .ConvertUsing(type => type.ToString());
        
        cfg.CreateMap<Models.Profile, AsAp.Object>()
            .ForAllMembers(opt => opt.Ignore());
        
        cfg.CreateMap<Uri, CompactIri?>()
            .ConstructUsing((uri) => CompactIri.FromUri(uri));

        cfg.CreateMap<Uri, AsAp.Link>(MemberList.None)
            .ConstructUsing(uri => new AsAp.Link(uri.ToString()));
        
        cfg.CreateMap<Uri, AsAp.Collection>()
            .ConvertUsing<EmptyCollectionConverter>();
    }
}