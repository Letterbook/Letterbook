using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Core.Models;

namespace Letterbook.Api.Mappers;

public class DtoMapper
{
    public static MapperConfiguration Default = new(cfg =>
    {
        cfg.CreateMap<Post, PostDto>();
        cfg.CreateMap<Mention, MentionDto>();
        cfg.CreateMap<Audience, AudienceDto>();
        cfg.CreateMap<Content, ContentDto>();

        cfg.CreateMap<PostDto, Post>();
        cfg.CreateMap<MentionDto, Mention>();
        cfg.CreateMap<AudienceDto, Audience>();
        cfg.CreateMap<ContentDto, Content>();
    });
}
