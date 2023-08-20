using Bogus;

namespace Letterbook.Core.Tests.Fakes;

public static class Init
{
    public static int WithSeed(int? seed = null)
    {
        var randomSeed = seed ?? new Random().Next();
        Randomizer.Seed = new Random(randomSeed);
        return randomSeed;
    }
}