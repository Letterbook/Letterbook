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
	private readonly MapperConfiguration _posts;

	private readonly MapperConfiguration _profiles;

	public MappingConfigProvider(IOptions<CoreOptions> options)
	{
		_posts = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<BaseMappings>();
			cfg.AddProfile(new PostMappings(options));
		});

		_profiles = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<BaseMappings>();
			cfg.AddProfile<ProfileMappings>();
		});
	}

	public MapperConfiguration Posts => _posts;
	public MapperConfiguration Profiles => _profiles;
}