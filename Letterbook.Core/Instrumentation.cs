using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Letterbook.Core;

public class Instrumentation : IDisposable
{
	internal const string ActivitySourceName = "Letterbook";
	internal const string ActivitySourceVersion = "1.0.0";

	public ActivitySource ActivitySource { get; }

	public Instrumentation()
	{
		ActivitySource = new ActivitySource(ActivitySourceName, ActivitySourceVersion);
	}

	public Activity? Span<T>([CallerMemberName] string method = "")
	{
		return ActivitySource.StartActivity($"{typeof(T).Name} {method}");
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		ActivitySource.Dispose();
	}
}