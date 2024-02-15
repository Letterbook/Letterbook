using Bogus;
using Letterbook.Core.Models;
using Xunit.Sdk;

namespace Letterbook.Core.Tests.Fakes;

public class FakePost : Faker<Post>
{
    private IEnumerable<Profile> _creators;
    private string _authority;
    
    public FakePost(string authority) : this(new FakeProfile(authority).Generate())
    { }
    
    public FakePost(Profile creator) : this(new []{creator}, 1)
    { }
    
    public FakePost(IEnumerable<Profile> creators, int contents)
    {
        _creators = creators;
        _authority = _creators.First().Authority;
        
        RuleFor(p => p.Id, faker => faker.Random.Uuid7());
        RuleFor(p => p.FediId, faker => faker.FediId(_authority, "post"));
        RuleFor(p => p.Creators, () => _creators);
        RuleFor(p => p.CreatedDate, faker => faker.Date.Recent());
        RuleFor(p => p.PublishedDate, (faker, post) => post.CreatedDate);
        RuleFor(p => p.Thread, (faker, post) => new ThreadContext
        {
            FediId = faker.FediId(_authority, "thread"),
            Root = post
        });
        FinishWith((faker, post) =>
        {
            var note = new FakeNote(post);
            foreach (var n in note.Generate(contents))
            {
                post.AddContent(n);
            }
        });
    }

}