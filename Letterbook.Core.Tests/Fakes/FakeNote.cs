using Bogus;
using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fakes;

public class FakeNote : Faker<Note>
{
    private Post _post;
    
    public FakeNote(Post post)
    {
        _post = post;

        RuleFor(note => note.Post, () => _post);
        RuleFor(note => note.Id, faker => faker.Random.Guid7());
        RuleFor(note => note.FediId, faker => faker.FediId(_post.FediId.Authority, "note"));
        RuleFor(note => note.Text, faker => faker.Lorem.Paragraph());
        FinishWith((faker, note) => note.GeneratePreview());
    }
}