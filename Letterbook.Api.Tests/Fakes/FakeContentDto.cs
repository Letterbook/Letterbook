using Bogus;
using Letterbook.Api.Dto;
using Letterbook.Core.Tests.Fakes;

namespace Letterbook.Api.Tests.Fakes;

public class FakeContentDto : Faker<ContentDto>
{
	public FakeContentDto()
	{
		RuleFor(d => d.Id, faker => faker.Random.Guid7());
		RuleFor(d => d.Type, () => "Note");
		RuleFor(d => d.Text, faker => faker.Lorem.Paragraph());
		RuleFor(d => d.Summary, faker => faker.Random.Bool() ? faker.Lorem.Sentence() : null);
	}
}