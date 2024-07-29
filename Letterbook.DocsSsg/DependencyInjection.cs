using Letterbook.DocsSsg.Markdown;

namespace Letterbook.DocsSsg;

public static class DependencyInjection
{
	public static IServiceCollection AddMarkdown<T>(this IServiceCollection services, string key) where T : class, IMarkdownFiles
	{
		return services.AddKeyedScoped<T>(key, (provider, _) =>
		{
			var t = provider.GetRequiredService<T>();
			t.LoadFrom(key);
			return t;
		});
	}
}