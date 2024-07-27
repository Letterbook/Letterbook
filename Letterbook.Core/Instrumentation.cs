using System.Diagnostics;

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

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		ActivitySource.Dispose();
	}
}