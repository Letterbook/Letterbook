using AutoMapper;
using Letterbook.Core.Models.Dto;
using Microsoft.Extensions.Options;

namespace Letterbook.Core.Models.Mappers;

public class ModerationReportMappings : AutoMapper.Profile
{
	public ModerationReportMappings(IOptions<CoreOptions> opts)
	{
		CreateMap<MemberModerationReportDto, ModerationReport>(MemberList.Source)
			.ConstructUsing(dto => new ModerationReport(opts.Value, null!))
			.ForMember(report => report.Id, opt => opt.Ignore())
			.ForSourceMember(s => s.Id, opt => opt.DoNotValidate());

		CreateMap<ModerationReport, MemberModerationReportDto>(MemberList.Destination);

		CreateMap<ModerationReport, FullModerationReportDto>(MemberList.Source)
			.IncludeBase<ModerationReport, MemberModerationReportDto>();

		CreateMap<FullModerationReportDto, ModerationReport>()
			.IncludeBase<MemberModerationReportDto, ModerationReport>()
			.ForMember(report => report.Id, opt => opt.MapFrom(s => s.Id));

		CreateMap<ModerationRemark, ModerationRemarkDto>(MemberList.Destination);
		CreateMap<ModerationRemarkDto, ModerationRemark>(MemberList.Source);
	}
}