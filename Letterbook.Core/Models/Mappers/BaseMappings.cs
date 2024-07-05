using AutoMapper;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers.Converters;
using Medo;

namespace Letterbook.Core.Models.Mappers;

public class BaseMappings : AutoMapper.Profile
{
	public BaseMappings()
	{
		CreateMap<Models.IFederated, Uuid7>()
			.ConvertUsing<IdUuid7Converter>();
		CreateMap<Models.Mention, Uuid7>()
			.ConvertUsing<IdUuid7Converter>();
		CreateMap<Models.ThreadContext, Uuid7>()
			.ConvertUsing<IdUuid7Converter>();
		CreateMap<Uuid7, Models.Profile>(MemberList.None)
			.ConstructUsing(uuid7 => Models.Profile.CreateEmpty(uuid7));
		CreateMap<Uuid7, Models.Post>(MemberList.None);
		CreateMap<Uuid7, Guid>().ConstructUsing(uuid7 => uuid7.ToGuid());
		CreateMap<Guid, Uuid7>().ConstructUsing(guid => Uuid7.FromGuid(guid));
		CreateMap<Uuid7, string>().ConstructUsing(uuid => uuid.ToId25String());
		CreateMap<string?, Uuid7>().ConstructUsing(str => str == null ? Uuid7.NewUuid7() : Uuid7.FromId25String(str));
		CreateMap<string?, Guid>().ConstructUsing(str => str == null ? Uuid7.NewUuid7().ToGuid() : Uuid7.FromId25String(str).ToGuid());
		CreateMap<string, Models.Profile>(MemberList.None)
			.ConstructUsing(s => Models.Profile.CreateEmpty(Uuid7.FromId25String(s)));

		CreateMap<DateTimeOffset?, DateTimeOffset?>()
			.ConvertUsing<DateTimeOffsetMapper>();
		CreateMap<DateTimeOffset, DateTimeOffset?>()
			.ConvertUsing<DateTimeOffsetMapper>();
		CreateMap<DateTimeOffset?, DateTimeOffset>()
			.ConvertUsing<DateTimeOffsetMapper>();
		CreateMap<DateTimeOffset, DateTimeOffset>()
			.ConvertUsing<DateTimeOffsetMapper>();

		CreateMap<AudienceDto, Models.Audience>(MemberList.Source)
			.ForSourceMember(dto => dto.SourceType, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.SourceId, opt => opt.DoNotValidate());
		CreateMap<Models.Audience, AudienceDto>(MemberList.Destination)
			.ForMember(dto => dto.SourceType, opt => opt.Ignore())
			.ForMember(dto => dto.SourceId, opt => opt.Ignore());

		CreateMap<FollowerRelation, FollowerRelationDto>(MemberList.Destination);
	}
}