using System.Runtime.CompilerServices;

namespace Letterbook.Workers.Tests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Init()
	{
		UseProjectRelativeDirectory("../../Snapshots");
	}
}