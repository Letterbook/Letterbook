using System.Text;
using AutoMapper;
using Letterbook.Core.Models.Dto;
using Org.BouncyCastle.Utilities.IO.Pem;
using PemWriter = Org.BouncyCastle.OpenSsl.PemWriter;


namespace Letterbook.Core.Models.Mappers;

public class ProfileMappings : AutoMapper.Profile
{
	public ProfileMappings()
	{
		CreateMap<Models.Profile, FullProfileDto>(MemberList.Destination)
			.ForMember(dto => dto.Followers, opt => opt.MapFrom(src => src.FollowersCount))
			.ForMember(dto => dto.Following, opt => opt.MapFrom(src => src.FollowingCount))
			.ForMember(dto => dto.Created, opt => opt.Ignore());
		CreateMap<FullProfileDto, Models.Profile>(MemberList.Source)
			.ForSourceMember(dto => dto.Updated, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.Created, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.Keys, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.Followers, opt => opt.DoNotValidate())
			.ForSourceMember(dto => dto.Following, opt => opt.DoNotValidate())
			.ForMember(profile => profile.Followers, opt => opt.Ignore())
			.ForMember(profile => profile.Following, opt => opt.Ignore())
			.ForMember(profile => profile.Updated, opt => opt.Ignore())
			.ForMember(profile => profile.Keys, opt => opt.Ignore());
		CreateMap<Models.Profile, MiniProfileDto>(MemberList.Destination);

		CreateMap<SigningKey, PublicKeyDto>()
			.ConvertUsing(src => new PublicKeyDto
			{
				Label = src.Label,
				Family = src.Family.ToString(),
				PublicKeyPem = PemStringBuilder(src.PublicKey).ToString().Trim().ReplaceLineEndings("\n"),
				Created = src.Created,
				Expires = src.Expires,
				FediId = src.FediId
			});
	}

	private static StringBuilder PemStringBuilder(ReadOnlyMemory<byte> publicKey)
	{
		var builder = new StringBuilder();
		using TextWriter writer = new StringWriter(builder);
		var pemWriter = new PemWriter(writer);
		var pem = new PemObject("PUBLIC KEY", publicKey.ToArray());
		pemWriter.WriteObject(pem);
		return builder;
	}
}