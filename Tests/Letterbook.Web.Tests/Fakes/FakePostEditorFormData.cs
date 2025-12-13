using Bogus;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Forms;

namespace Letterbook.Web.Tests.Fakes;

public class FakePostEditorFormData : Faker<PostEditorFormData>
{
	public FakePostEditorFormData()
	{
		RuleFor(d => d.Id, faker => faker.Random.Uuid7());
		RuleFor(d => d.Authors, faker => [faker.Random.Uuid7()]);
		RuleFor(d => d.Note, faker => new()
		{
			Id = faker.Random.Guid(),
			Contents = faker.Lorem.Sentences(3)
		});
		RuleFor(d => d.Audience, faker => new Dictionary<string, string>() { { faker.Internet.UrlWithPath(), faker.Lorem.Word() } });
	}
}