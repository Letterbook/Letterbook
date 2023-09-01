namespace Letterbook.Api.Tests.Support.Extensions;

public static class StringExtensions
{
    public static void MustMatchJson(this string self, string expected)
    {
        var diff = new JsonDiffPatchDotNet.JsonDiffPatch();

        var differences = diff.Diff(self, expected);

        Assert.True(differences == null, differences ?? "");
    }
}