using AutoMapper;
using Microsoft.Extensions.Options;

namespace Letterbook.Core.Models.Mappers;

/// <summary>
/// This class exists to allow AutoMapper configs to benefit from Dependency Injection. Specifically, from IOptions.
///
/// At some level, it is redundant with the built-in UseAutoMapper() DI extension. We're not using that, because it's likely that we'll want
/// multiple mappings from one type to another. We already need it in ActivityPub. The built-in doesn't support that. It scans for
/// Mapper Profiles in the given assemblies, and composes them all into a single MapperConfig. A config can only support 1 mapping of
/// Tx -> Ty. So we need to have multiple configs, and hence this class.
/// </summary>
public class MappingConfigProvider
{
	public MappingConfigProvider(IOptions<CoreOptions> options)
	{
		Posts = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<BaseMappings>();
			cfg.AddProfile(new PostMappings(options));
		});

		Profiles = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<BaseMappings>();
			cfg.AddProfile<ProfileMappings>();
		});

		ModerationReports = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<BaseMappings>();
			cfg.AddProfile(new ModerationReportMappings(options));
		});
	}

	public MapperConfiguration Posts { get; }

	public MapperConfiguration Profiles { get; }

	public MapperConfiguration ModerationReports { get; }
}