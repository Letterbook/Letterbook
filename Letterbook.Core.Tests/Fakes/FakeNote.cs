using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public sealed class FakeNote : Faker<Note>
{
    public FakeNote()
    {
        CustomInstantiator(faker => new Note(new Uri(faker.Internet.UrlWithPath())));
    }
    
}