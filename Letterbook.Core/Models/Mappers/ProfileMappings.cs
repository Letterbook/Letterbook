using AutoMapper;
using Letterbook.Core.Models.Dto;


namespace Letterbook.Core.Models.Mappers;

public class ProfileMappings : AutoMapper.Profile
{
	public ProfileMappings()
	{
		CreateMap<Models.Profile, FullProfileDto>(MemberList.Destination)
			.ForMember(dto => dto.Created, opt => opt.Ignore());
		CreateMap<FullProfileDto, Models.Profile>(MemberList.Source)
			.ForSourceMember(dto => dto.Updated, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.Created, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.Keys, opt => opt.DoNotValidate())
			.ForMember(profile => profile.Updated, opt => opt.Ignore())
			.ForMember(profile => profile.Keys, opt => opt.Ignore());

		CreateMap<Models.SigningKey, PublicKeyDto>()
			.ForMember(dto => dto.PublicKeyPem, opt => opt.MapFrom<PublicKeyPemConverter>());
	}
}