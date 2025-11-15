using System.Net;
using Letterbook.Api.Dto;

namespace Letterbook.IntegrationTests.LetterbookAPI;

public partial class TrustAndSafetyTests
{
	[Fact(DisplayName = "Should import mastodon-format peer lists")]
	public async Task CanImportPeerList()
	{
		const string given = """
		            #domain,#severity,#reject_media,#reject_reports,#public_comment,#obfuscate
		            ap.example,silence,TRUE,TRUE,letterbook:test,FALSE
		            ap2.example,suspend,TRUE,TRUE,letterbook:test,FALSE
		            """;
		var payload = new MultipartFormDataContent();
		payload.Add(new StringContent(given));

		var response = await _client.PostAsync($"/lb/v1/peers/import?format={DenyListFormat.Mastodon}", payload);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}