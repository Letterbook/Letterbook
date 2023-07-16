using AutoMapper;
using Letterbook.Core.Models;

namespace Letterbook.Core.Mappers;

public static class DtoMapper
{
    public static MapperConfiguration Config = new (cfg =>
    {
        configureObject(cfg);
        configureProfile(cfg);
    });

    private static void configureObject(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<DTO.Activity, IEnumerable<IObjectRef>>();
    }

    private static void configureProfile(IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<DTO.Actor, Models.Profile>();
    }
}