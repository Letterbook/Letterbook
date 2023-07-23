using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public sealed class FakeProfile : Faker<Profile>
{
    public FakeProfile()
    {
        CustomInstantiator(f => new Profile(new Uri(f.Internet.UrlWithPath())));
    }
}