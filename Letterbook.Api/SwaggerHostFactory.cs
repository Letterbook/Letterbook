using JetBrains.Annotations;

namespace Letterbook.Api;

/// <summary>
/// Used by the dotnet swagger tool to generate static OpenAPI spec files at build time
/// </summary>
public static class SwaggerHostFactory
{
	[UsedImplicitly]
	public static IHost CreateHost()
	{
		var builder = WebApplication.CreateBuilder(new WebApplicationOptions
		{
			// Normally this is set by convention, based on the project name. But the swagger cli doesn't do that.
			// So we have to set it ourselves. Otherwise, the tool won't include the paths and components properly,
			// and the generated spec will be mostly empty
			ApplicationName = "Letterbook.Api",
		});
		var host = builder.ConfigureHostBuilder().Build();

		host.UsePathBase(new PathString("/api/v1"));
		host.MapControllers();

		return host;
	}
}