using Bogus;
using Medo;

namespace Letterbook.Core.Tests.Fakes;

public static class Extensions
{
    public static Uuid7 Uuid7(this Randomizer randomizer) => Medo.Uuid7.FromGuid(randomizer.Guid());
}