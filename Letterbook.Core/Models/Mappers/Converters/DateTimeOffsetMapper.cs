using AutoMapper;

namespace Letterbook.Core.Models.Mappers.Converters;

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