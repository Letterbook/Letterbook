using Bogus;
using Bogus.DataSets;
using Medo;

namespace Letterbook.Core.Tests.Fakes;

public static class Extensions
{
    public static Uuid7 Uuid7(this Randomizer randomizer) => Medo.Uuid7.FromGuid(randomizer.Guid());
    public static Guid Guid7(this Randomizer r) => r.Uuid7().ToGuid();

    public static Uri FediId(this Faker f, string domain, string path) =>
        new(f.Internet.UrlWithPath("https", domain, $"{path}/{f.Random.Int(1000, 9999)}"));

    public static Uri FediId(this Faker f, string domain, string type, Uuid7 id) =>
	    new($"https://{domain}/{type}/{id.ToId25String()}");
}