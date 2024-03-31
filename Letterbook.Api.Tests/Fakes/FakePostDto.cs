using Bogus;
using Letterbook.Api.Dto;
using Letterbook.Core;

namespace Letterbook.Api.Tests.Fakes;

public class FakePostDto : Faker<PostDto>
{
	public FakePostDto()
	{
		RuleFor(d => d.Contents, () => new FakeContentDto().Generate(1));
		RuleFor(d => d.CreatedDate, () => DateTimeOffset.Now);
	}

	public FakePostDto(Models.Profile profile) : this()
	{
		RuleFor(d => d.Creators, () => new List<MiniProfileDto>() { new()
			{
				Id = profile.GetId(),
				DisplayName = profile.DisplayName,
				FediId = profile.FediId,
				Handle = profile.Handle
			}
		});
	}

	public FakePostDto(FakeContentDto fakeContentDto, int count) : this()
	{
		RuleFor(d => d.Contents, () => fakeContentDto.Generate(count));
	}
}