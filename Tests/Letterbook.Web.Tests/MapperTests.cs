using AutoMapper;
using Bogus;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Mappers;
using Xunit.Abstractions;

namespace Letterbook.Web.Tests;

public class MapperTests : WithMocks
{
	private readonly Mapper _mapper;

	public MapperTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_mapper = new Mapper(FormsProfileProvider.Profile);
	}

	[Fact]
	public void Valid()
	{
		FormsProfileProvider.Profile.AssertConfigurationIsValid();
	}

	[Fact]
	public void ShouldMapPostEditorForm()
	{
		var fake = new Faker();

		var form = new Forms.PostEditorFormData
		{
			Id = fake.Random.Uuid7(),
			Authors = [fake.Random.Uuid7()],
			Note = new()
			{
				Id = fake.Random.Guid(),
				Contents = fake.Lorem.Sentences(3)
			},
			Audience = new Dictionary<string, string>() { { fake.Internet.UrlWithPath(), fake.Lorem.Word() } }
		};

		var actual = _mapper.Map<Models.Post>(form);
		Assert.Equal(form.Id, actual.Id);
		Assert.Equal(form.Authors, actual.Creators.Select(a => a.Id));
		Assert.Equal(form.Note.Id, actual.Contents.FirstOrDefault()?.Id);
		Assert.Equal(form.Note.Contents, actual.Contents.FirstOrDefault()?.Html);
		var actualNote = Assert.IsType<Models.Note>(actual.Contents.FirstOrDefault());
		Assert.Equal(form.Note.Contents, actualNote.SourceText);
	}
}