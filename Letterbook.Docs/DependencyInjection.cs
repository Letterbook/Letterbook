namespace Letterbook.Docs;

public static class DependencyInjection
{
	public static IServiceCollection AddMarkdown<T>(this IServiceCollection services, string key) where T : class, IMarkdownPages
	{
		return services.AddKeyedSingleton<T>(key, (provider, _) =>
		{
			var t = provider.GetRequiredService<T>();
			t.LoadFrom(key);
			return t;
		});
	}
}