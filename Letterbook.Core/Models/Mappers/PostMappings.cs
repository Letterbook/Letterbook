using AutoMapper;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers.Converters;
using Microsoft.Extensions.Options;

namespace Letterbook.Core.Models.Mappers;

public class PostMappings : AutoMapper.Profile
{
	public PostMappings(IOptions<CoreOptions> options)
	{
		CreateMap<MentionDto, Mention>(MemberList.Source)
			.ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Mentioned));

		CreateMap<ThreadContext, ThreadDto>(MemberList.Destination);
		CreateMap<ThreadDto, ThreadContext>(MemberList.Source);

		CreateMap<Mention, MentionDto>(MemberList.Destination)
			.ForMember(dest => dest.Mentioned, opt => opt.MapFrom(src => src.GetId25()));

		CreateMap<Note, ContentDto>(MemberList.Destination)
			.IncludeBase<Content, ContentDto>()
			.ForMember(dto => dto.Text, opt => opt.MapFrom(src => src.SourceText));
		CreateMap<Content, ContentDto>(MemberList.Destination)
			.ForMember(dest => dest.Text, opt => opt.Ignore());
		CreateMap<ContentDto, Note>(MemberList.Source)
			.ForMember(note => note.SourceText, opt => opt.MapFrom(dto => dto.Text))
			.ForMember(note => note.SourceContentType, opt => opt.MapFrom((dto) => dto.ContentType ?? "text/plain"))
			.ForSourceMember(src => src.Type, opt => opt.DoNotValidate());
		CreateMap<ContentDto, Content>(MemberList.None)
			.ConstructUsing((dto, ctx) =>
			{
				return dto.Type switch
				{
					"Note" => ctx.Mapper.Map<Note>(dto),
					_ => ctx.Mapper.Map<Note>(dto)
				};
			})
			.AfterMap((_, ct) =>
			{
				ct.GeneratePreview();
				ct.SetLocalFediId(options.Value);
			});

		CreateMap<Post, PostDto>(MemberList.Destination);
		CreateMap<PostDto, Post>(MemberList.Source)
			.ConstructUsing(_ => NewPost(options.Value))
			.ForMember(post => post.Thread, opt => opt.Ignore())
			.ForMember(post => post.InReplyTo, opt => opt.Ignore())
			.ForMember(post => post.Id, opt => opt.PreCondition(dto => dto.Id != null))
			.ForMember(post => post.Id, opt => opt.MapFrom(dto => dto.Id))
			.ForMember(post => post.FediId, opt => opt.PreCondition(dto => dto.FediId != null))
			.ForMember(post => post.CreatedDate, opt => opt.ConvertUsing<DateTimeOffsetMapper, DateTimeOffset?>())
			.ForMember(post => post.PublishedDate, opt => opt.ConvertUsing<DateTimeOffsetMapper, DateTimeOffset?>())
			.ForMember(post => post.UpdatedDate, opt => opt.ConvertUsing<DateTimeOffsetMapper, DateTimeOffset?>())
			.ForSourceMember(src => src.InReplyTo, opt => opt.DoNotValidate())
			.ForSourceMember(src => src.Thread, opt => opt.DoNotValidate());

		CreateMap<Models.Profile, MiniProfileDto>(MemberList.Destination);
		CreateMap<MiniProfileDto, Models.Profile>(MemberList.Source);
	}

	private static Post NewPost(CoreOptions options) => new(options);
}

public class PublicKeyPemConverter : IValueResolver<SigningKey, PublicKeyDto, string>
{
	public string Resolve(SigningKey source, PublicKeyDto destination, string destMember, ResolutionContext context)
	{
		return source.Family switch
		{
			SigningKey.KeyFamily.Rsa => source.GetRsa().ExportSubjectPublicKeyInfoPem(),
			SigningKey.KeyFamily.Dsa => source.GetDsa().ExportSubjectPublicKeyInfoPem(),
			SigningKey.KeyFamily.EcDsa => source.GetEcDsa().ExportSubjectPublicKeyInfoPem(),
			_ => null!,
		};
	}
}
