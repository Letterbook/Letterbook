using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Letterbook.Web.Tests.E2E.Support;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class SkipUnlessPortEquals(int port) : NUnitAttribute, IApplyToTest
{
	public void ApplyToTest(Test test)
	{
		if (Settings.NoSkip || port == Settings.BaseUrl.Port) return;

		test.RunState = RunState.Ignored;
		test.Properties.Set(
			PropertyNames.SkipReason,
			$"Skipped because test is not running against port <{port}> ({Settings.BaseUrl})");
	}
}