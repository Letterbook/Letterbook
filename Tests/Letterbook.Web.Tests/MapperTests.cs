using AutoMapper;
using Bogus;
using Letterbook.Core.Tests;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Web.Mappers;
using Letterbook.Web.Tests.Fakes;
using Medo;
using Microsoft.CodeAnalysis.Options;
using Xunit.Abstractions;

namespace Letterbook.Web.Tests;

public class MapperTests : WithMocks
{
	private readonly Mapper _mapper;
	private readonly FormsProfileProvider _provider;
	private readonly FakePostEditorFormData _faker;

	public MapperTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_provider = new FormsProfileProvider(CoreOptionsMock);
		_mapper = new Mapper(_provider.Profile);
		_faker = new FakePostEditorFormData();
	}

	[Fact]
	public void Valid()
	{
		_provider.Profile.AssertConfigurationIsValid();
	}

	[Fact]
	public void ShouldMapPostEditorForm()
	{
		var form = _faker.Generate();

		var actual = _mapper.Map<Models.Post>(form);
		Assert.NotNull(actual.FediId);
		Assert.Equal(form.Id, actual.Id);
		Assert.Equal(form.Authors, actual.Creators.Select(a => a.Id));
		Assert.Equal(form.Note.Id, actual.Contents.FirstOrDefault()?.Id);
		Assert.Equal(form.Note.Contents, actual.Contents.FirstOrDefault()?.Html);
		var actualNote = Assert.IsType<Models.Note>(actual.Contents.FirstOrDefault());
		Assert.Equal(form.Note.Contents, actualNote.SourceText);
	}

	[Fact]
	public void ShouldMapNewId()
	{
		var form = _faker.Generate();
		form.Id = Uuid7.Empty;

		var actual = _mapper.Map<Models.Post>(form);
		Assert.NotEqual(Uuid7.Empty, actual.Id);
	}

	[Fact]
	public void ShouldMapNewContentId()
	{
		var form = _faker.Generate();
		form.Note.Id = Guid.Empty;

		var actual = _mapper.Map<Models.Post>(form);
		var actualContent = actual.Contents.FirstOrDefault();
		Assert.NotNull(actualContent);
		Assert.NotEqual(Guid.Empty, actualContent.Id);
	}
}