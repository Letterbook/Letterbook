using Bogus;
using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class SafetyControllerTests : WithMockContext
{
	private readonly SafetyController _controller;
	private readonly Guid _accountId;

	public SafetyControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_accountId = new Faker().Random.Guid();
		MockAuthorizeAllowAll();
		Auth(_accountId);
		var logger = output.BuildLoggerFor<SafetyController>();

		_controller = new SafetyController(logger, ModerationServiceMock.Object, AuthorizationServiceMock.Object)
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact]
	public async Task ShouldImportOne_Mastodon()
	{
		var csv = """
		          #domain,#severity,#reject_media,#reject_reports,#public_comment,#obfuscate
		          ap.example,suspend,FALSE,FALSE,letterbook:test,TRUE
		          """;

		var result = await _controller.ImportDenyList(csv, DenyListFormat.Mastodon, ModerationService.MergeStrategy.ReplaceAll);

		Assert.IsType<OkResult>(result);
		AuthzModerationServiceMock.Verify(m => m.ImportPeerRestrictions(It.Is<ICollection<Models.Peer>>(
			p => p.Any(peer => peer.Restrictions.ContainsKey(Models.Restrictions.Defederate)
				&& peer.Restrictions.Count == 1)
			), ModerationService.MergeStrategy.ReplaceAll));
	}
}