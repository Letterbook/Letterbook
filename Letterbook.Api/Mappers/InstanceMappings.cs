using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Core;
using Letterbook.Core.Models;
using Medo;
using Microsoft.Extensions.Options;
using Models = Letterbook.Core.Models;

namespace Letterbook.Api.Mappers;

public class InstanceMappings : AutoMapper.Profile
{
	public InstanceMappings(IOptions<CoreOptions> options)
	{
		CreateMap<MentionDto, Mention>(MemberList.Source)
			.ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Mentioned));
		CreateMap<AudienceDto, Audience>(MemberList.Source)
			.ForSourceMember(dto => dto.SourceType, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.SourceId, opt => opt.DoNotValidate());
		CreateMap<Audience, AudienceDto>(MemberList.Destination)
			.ForMember(dto => dto.SourceType, opt => opt.Ignore())
			.ForMember(dto => dto.SourceId, opt => opt.Ignore());
		CreateMap<IFederated, Uuid7>()
			.ConvertUsing<IdUuid7Converter>();
		CreateMap<Mention, Uuid7>()
			.ConvertUsing<IdUuid7Converter>();
		CreateMap<ThreadContext, Uuid7>()
			.ConvertUsing<IdUuid7Converter>();
		CreateMap<Uuid7, Models.Profile>(MemberList.None)
			.ConstructUsing(uuid7 => Models.Profile.CreateEmpty(uuid7));
		CreateMap<Uuid7, Post>(MemberList.None);
		CreateMap<Uuid7, Guid>().ConstructUsing(uuid7 => uuid7.ToGuid());
		CreateMap<Guid, Uuid7>().ConstructUsing(guid => Uuid7.FromGuid(guid));
		CreateMap<Uuid7, string>().ConstructUsing(uuid => uuid.ToId25String());
		CreateMap<string?, Uuid7>().ConstructUsing(str => str == null ? Uuid7.NewUuid7() : Uuid7.FromId25String(str));
		CreateMap<string?, Guid>().ConstructUsing(str => str == null ? Uuid7.NewUuid7().ToGuid() : Uuid7.FromId25String(str).ToGuid());
		CreateMap<string, Models.Profile>(MemberList.None)
			.ConstructUsing(s => Models.Profile.CreateEmpty(Uuid7.FromId25String(s)));

		CreateMap<ThreadContext, ThreadDto>(MemberList.Destination);
		CreateMap<ThreadDto, ThreadContext>(MemberList.Source);

		CreateMap<Mention, MentionDto>(MemberList.Destination)
			.ForMember(dest => dest.Mentioned, opt => opt.MapFrom(src => src.GetId25()));

		CreateMap<Note, ContentDto>(MemberList.Destination)
			.IncludeBase<Content, ContentDto>()
			.ForMember(dto => dto.Text, opt => opt.MapFrom(src => src.Text));
		CreateMap<Content, ContentDto>(MemberList.Destination)
			.ForMember(dest => dest.Text, opt => opt.Ignore());
		CreateMap<ContentDto, Note>(MemberList.Source)
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

		CreateMap<DateTimeOffset?, DateTimeOffset?>()
			.ConvertUsing<DateTimeOffsetMapper>();
		CreateMap<DateTimeOffset, DateTimeOffset?>()
			.ConvertUsing<DateTimeOffsetMapper>();
		CreateMap<DateTimeOffset?, DateTimeOffset>()
			.ConvertUsing<DateTimeOffsetMapper>();
		CreateMap<DateTimeOffset, DateTimeOffset>()
			.ConvertUsing<DateTimeOffsetMapper>();
	}

	private static Post NewPost(CoreOptions options) => new(options);
}

public class DateTimeOffsetMapper : IValueConverter<DateTimeOffset?, DateTimeOffset>,
	IValueConverter<DateTimeOffset?, DateTimeOffset?>,
	ITypeConverter<DateTimeOffset, DateTimeOffset>,
	ITypeConverter<DateTimeOffset?, DateTimeOffset?>,
	ITypeConverter<DateTimeOffset, DateTimeOffset?>,
	ITypeConverter<DateTimeOffset?, DateTimeOffset>
{
	private DateTimeOffset ToUtc(DateTimeOffset? source) => source?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
	private DateTimeOffset? ToNullable(DateTimeOffset? source) => source == null ? null : ToUtc(source);

	public DateTimeOffset Convert(DateTimeOffset? sourceMember, ResolutionContext context) => ToUtc(sourceMember);

	DateTimeOffset? IValueConverter<DateTimeOffset?, DateTimeOffset?>.Convert(DateTimeOffset? sourceMember, ResolutionContext context)
		=> ToNullable(sourceMember);

	public DateTimeOffset Convert(DateTimeOffset source, DateTimeOffset destination, ResolutionContext context)
		=> ToUtc(source);

	public DateTimeOffset? Convert(DateTimeOffset? source, DateTimeOffset? destination, ResolutionContext context)
		=> ToNullable(source);

	public DateTimeOffset? Convert(DateTimeOffset source, DateTimeOffset? destination, ResolutionContext context)
		=> ToNullable(source);

	public DateTimeOffset Convert(DateTimeOffset? source, DateTimeOffset destination, ResolutionContext context)
		=> ToUtc(source);
}

public class BaseTypeMappings : AutoMapper.Profile
{
	public BaseTypeMappings()
	{

	}
}