using AutoMapper;
using Letterbook.Api.Dto;
using Letterbook.Core.Models;
using Medo;
using Profile = Letterbook.Core.Models.Profile;

namespace Letterbook.Api.Mappers;

internal class Guid7Converter : IMemberValueResolver<Mention, MentionDto, Uuid7, Uuid7>
{
	public Uuid7 Resolve(Mention source, MentionDto destination, Uuid7 sourceMember, Uuid7 destMember, ResolutionContext context)
	{
		return source.GetId();
	}
}

internal class IdUuid7Converter :
	ITypeConverter<IFederated, Uuid7>,
	ITypeConverter<Mention, Uuid7>,
	ITypeConverter<ThreadContext, Uuid7>
{
	public Uuid7 Convert(IFederated source, Uuid7 destination, ResolutionContext context)
	{
		return source.GetId();
	}

	public Uuid7 Convert(Mention source, Uuid7 destination, ResolutionContext context)
	{
		return source.GetId();
	}

	public Uuid7 Convert(ThreadContext source, Uuid7 destination, ResolutionContext context)
	{
		return source.GetId();
	}
}