using Bogus;
using Bogus.DataSets;
using Medo;

namespace Letterbook.Core.Tests.Fakes;

public static class Extensions
{
    public static Uuid7 Uuid7(this Randomizer randomizer) => Medo.Uuid7.FromGuid(randomizer.Guid());

    public static Uri FediId(this Faker f, string domain, string path) =>
        new(f.Internet.UrlWithPath("https", domain, $"{path}/{f.Random.Int(1000, 9999)}"));
}