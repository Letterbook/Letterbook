using AutoMapper;
using Letterbook.Core.Models.Dto;
using Microsoft.Extensions.Options;

namespace Letterbook.Core.Models.Mappers;

public class ModerationReportMappings : AutoMapper.Profile
{
	public ModerationReportMappings(IOptions<CoreOptions> opts)
	{
		CreateMap<MemberModerationReportDto, ModerationReport>(MemberList.Source)
			.MaxDepth(64)
			.ConstructUsing(dto => new ModerationReport(opts.Value, null!))
			.ForMember(report => report.Id, opt => opt.Ignore())
			.ForSourceMember(s => s.Id, opt => opt.DoNotValidate());

		CreateMap<ModerationReport, MemberModerationReportDto>(MemberList.Destination)
			.MaxDepth(64);

		CreateMap<ModerationReport, FullModerationReportDto>(MemberList.Source)
			.MaxDepth(64)
			.IncludeBase<ModerationReport, MemberModerationReportDto>();

		CreateMap<FullModerationReportDto, ModerationReport>()
			.MaxDepth(64)
			.IncludeBase<MemberModerationReportDto, ModerationReport>()
			.ForMember(report => report.Id, opt => opt.MapFrom(s => s.Id));

		CreateMap<ModerationRemark, ModerationRemarkDto>(MemberList.Destination)
			.MaxDepth(64);
		CreateMap<ModerationRemarkDto, ModerationRemark>(MemberList.Source)
			.MaxDepth(64)
			.ForMember(d => d.Id, opt => opt.Ignore())
			.ForSourceMember(s => s.Id, opt => opt.DoNotValidate());

		CreateMap<ModerationPolicy, ModerationPolicyDto>(MemberList.Destination)
			.MaxDepth(64);
		CreateMap<ModerationPolicyDto, ModerationPolicy>(MemberList.Source)
			.MaxDepth(64)
			.ForMember(d => d.Id, opt => opt.Ignore())
			.ForSourceMember(s => s.Id, opt => opt.DoNotValidate());
	}
}