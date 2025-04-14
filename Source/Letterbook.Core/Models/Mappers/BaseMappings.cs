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
		CreateMap<Models.ThreadContext, Uuid7>()
			.ConvertUsing<IdUuid7Converter>();
		CreateMap<Uuid7, Models.Profile>(MemberList.None)
			.ConstructUsing(uuid7 => Models.Profile.CreateEmpty(new ProfileId(uuid7)));
		CreateMap<ProfileId, Models.Profile>(MemberList.None)
			.ConstructUsing(id => Models.Profile.CreateEmpty(id));
		CreateMap<Models.Profile, ProfileId>()
			.ConstructUsing(profile => profile.Id);

		CreateMap<Guid, Account>(MemberList.None)
			.ConstructUsing(id => new Account(){Id = id})
			.ReverseMap()
			.ConstructUsing(account => account.Id);

		CreateMap<ThreadId, ThreadContext>(MemberList.None)
			.ConstructUsing(id => new ThreadContext
			{
				Id = id,
				RootId = null
			})
			.ReverseMap()
			.ConstructUsing(thread => thread.Id);

		CreateMap<ModerationReportId, ModerationReport>(MemberList.None)
			.ConstructUsing(id => new ModerationReport
			{
				Id = id,
				Summary = default!,
				Context = default!,
			})
			.ReverseMap()
			.ConstructUsing(o => o.Id);

		CreateMap<ModerationRemarkId, ModerationRemark>(MemberList.None)
			.ConstructUsing(id => new ModerationRemark
			{
				Id = id,
				Report = default!,
				Author = default!,
				Text = default!,
			})
			.ReverseMap()
			.ConstructUsing(o => o.Id);

		CreateMap<ModerationPolicyId, ModerationPolicy>(MemberList.None)
			.ConstructUsing(id => new ModerationPolicy
			{
				Id = id,
			})
			.ReverseMap()
			.ConstructUsing(o => o.Id);

		CreateMap<PostId, Models.Post>(MemberList.None)
			.ConstructUsing(id => new Post { Id = id });
		CreateMap<Models.Post, PostId>()
			.ConstructUsing(post => post.Id);
		CreateMap<Uuid7, Models.Post>(MemberList.None);
		CreateMap<Uuid7, Guid>().ConstructUsing(uuid7 => uuid7.ToGuid());
		CreateMap<Guid, Uuid7>().ConstructUsing(guid => Uuid7.FromGuid(guid));
		CreateMap<Uuid7, string>().ConstructUsing(uuid => uuid.ToId25String());
		CreateMap<string?, Uuid7>().ConstructUsing(str => str == null ? Uuid7.NewUuid7() : Uuid7.FromId25String(str));
		CreateMap<string?, Guid>().ConstructUsing(str => str == null ? Uuid7.NewUuid7().ToGuid() : Uuid7.FromId25String(str).ToGuid());
		CreateMap<string, Models.Profile>(MemberList.None)
			.ConstructUsing(s => Models.Profile.CreateEmpty(new ProfileId(ProfileId.FromString(s))));

		CreateMap<Uuid7, ProfileId>(MemberList.None).ConvertUsing(id => new(id));
		CreateMap<ProfileId, Uuid7>(MemberList.None).ConvertUsing(id => id.Id);


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