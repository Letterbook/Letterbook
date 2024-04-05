using AutoMapper;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Api.Mappers.Converters;

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