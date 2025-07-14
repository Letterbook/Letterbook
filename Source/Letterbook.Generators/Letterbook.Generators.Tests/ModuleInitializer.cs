using System.Runtime.CompilerServices;
using VerifyTests;

namespace Letterbook.Generators.Tests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Init() =>
		VerifySourceGenerators.Initialize();
}