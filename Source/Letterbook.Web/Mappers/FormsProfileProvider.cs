using AutoMapper;
using Letterbook.Core.Models.Mappers;

namespace Letterbook.Web.Mappers;

public static class FormsProfileProvider
{
	public static MapperConfiguration Profile = new MapperConfiguration(cfg =>
	{
		cfg.AddProfile<BaseMappings>();
		cfg.AddProfile<FormMappings>();
	});
}