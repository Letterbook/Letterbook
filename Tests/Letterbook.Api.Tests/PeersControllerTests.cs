using Bogus;
using Letterbook.Api.Controllers;
using Letterbook.Api.Dto;
using Letterbook.Core;
using Letterbook.Core.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class PeersControllerTests : WithMockContext
{
	private readonly PeersController _controller;
	private readonly Guid _accountId;

	public PeersControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_accountId = new Faker().Random.Guid();
		MockAuthorizeAllowAll();
		Auth(_accountId);
		var logger = output.BuildLoggerFor<PeersController>();

		_controller = new PeersController(logger, ModerationServiceMock.Object, AuthorizationServiceMock.Object)
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
	public async Task ShouldImportMastodon_Defederate()
	{
		const string csv = """
		                   #domain,#severity,#reject_media,#reject_reports,#public_comment,#obfuscate
		                   ap.example,suspend,FALSE,FALSE,letterbook:test,TRUE
		                   """;
		var payload = BuildPayload(csv);

		var result = await _controller.ImportDenyList(payload, DenyListFormat.Mastodon, ModerationService.MergeStrategy.ReplaceAll);

		Assert.IsType<OkResult>(result);
		AuthzModerationServiceMock.Verify(m => m.ImportPeerRestrictions(It.Is<ICollection<Models.Peer>>(
			p => p.Any(peer => peer.Restrictions.ContainsKey(Models.Restrictions.Defederate)
			                   && peer.Restrictions.Count == 1)
		), ModerationService.MergeStrategy.ReplaceAll));
	}

	private static FormFile BuildPayload(string csv)
	{
		var stream = new MemoryStream();
		var streamWriter = new StreamWriter(stream);
		streamWriter.Write(csv);
		streamWriter.Flush();
		stream.Position = 0;
		var payload = new FormFile(stream, stream.Position, stream.Length, "csv", "blocklist.csv");
		return payload;
	}

	[Fact]
	public async Task ShouldImportMastodon_MultipleRestrictions()
	{
		const string csv = """
		                   #domain,#severity,#reject_media,#reject_reports,#public_comment,#obfuscate
		                   ap.example,silence,TRUE,TRUE,letterbook:test,FALSE
		                   """;

		var result = await _controller.ImportDenyList(BuildPayload(csv), DenyListFormat.Mastodon, ModerationService.MergeStrategy.KeepAll);

		Assert.IsType<OkResult>(result);
		AuthzModerationServiceMock.Verify(m => m.ImportPeerRestrictions(It.Is<ICollection<Models.Peer>>(
			p => p.Any(peer => peer.Restrictions.ContainsKey(Models.Restrictions.LimitDiscovery)
			                   && peer.Restrictions.ContainsKey(Models.Restrictions.DenyAttachments)
			                   && peer.Restrictions.ContainsKey(Models.Restrictions.DenyReports)
			                   && peer.Restrictions.ContainsKey(Models.Restrictions.Warn)
			                   && peer.Restrictions.Count == 4)
		), ModerationService.MergeStrategy.KeepAll));
	}

	[Fact]
	public async Task ShouldImportMastodon_MultiplePeers()
	{
		const string csv = """
		                   #domain,#severity,#reject_media,#reject_reports,#public_comment,#obfuscate
		                   ap.example,silence,TRUE,TRUE,letterbook:test,FALSE
		                   ap2.example,silence,TRUE,TRUE,letterbook:test,FALSE
		                   ap3.example,silence,TRUE,TRUE,letterbook:test,FALSE
		                   ap4.example,silence,TRUE,TRUE,letterbook:test,FALSE
		                   ap5.example,silence,TRUE,TRUE,letterbook:test,FALSE
		                   """;

		var result = await _controller.ImportDenyList(BuildPayload(csv), DenyListFormat.Mastodon, ModerationService.MergeStrategy.KeepAll);

		Assert.IsType<OkResult>(result);
		AuthzModerationServiceMock.Verify(m => m.ImportPeerRestrictions(It.Is<ICollection<Models.Peer>>(
			collection => collection.Count == 5
			              && collection.Any(peer => peer.Hostname == "ap.example")
			              && collection.Any(peer => peer.Hostname == "ap2.example")
			              && collection.Any(peer => peer.Hostname == "ap3.example")
			              && collection.Any(peer => peer.Hostname == "ap4.example")
			              && collection.Any(peer => peer.Hostname == "ap5.example")
		), ModerationService.MergeStrategy.KeepAll));
	}
}