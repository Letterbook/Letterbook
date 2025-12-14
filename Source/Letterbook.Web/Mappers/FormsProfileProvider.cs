using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Models.Mappers;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Mappers;

public class FormsProfileProvider
{
	public FormsProfileProvider(IOptions<CoreOptions> options)
	{
		Profile = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<BaseMappings>();
			cfg.AddProfile(new FormMappings(options));
		});
	}

	public MapperConfiguration Profile { get; }
}