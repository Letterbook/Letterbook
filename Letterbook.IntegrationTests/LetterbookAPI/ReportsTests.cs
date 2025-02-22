using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests.Fakes;
using Letterbook.IntegrationTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Letterbook.IntegrationTests.LetterbookAPI;

[Trait("Infra", "Postgres")]
[Trait("Driver", "Api")]
public class ReportsTests : IClassFixture<HostFixture<ReportsTests>>, ITestSeed, IDisposable
{
	private readonly HostFixture<ReportsTests> _host;
	private readonly IServiceScope _scope;
	private readonly Mapper _mapper;
	private readonly JsonSerializerOptions _json;
	private readonly HttpClient _client;

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		_scope.Dispose();
		_client.Dispose();
	}

	public ReportsTests(HostFixture<ReportsTests> host, ITestOutputHelper output)
	{
		_host = host;
		_scope = host.CreateScope();
		_client = _host.CreateClient(_host.DefaultOptions);
		_client.DefaultRequestHeaders.Authorization = new("Test", $"{_host.Accounts[0].Id}");

		var mappings = _scope.ServiceProvider.GetRequiredService<MappingConfigProvider>().ModerationReports;
		_mapper = new Mapper(mappings);
		_json = new JsonSerializerOptions(JsonSerializerDefaults.Web)
		{
			// Converters = { new Uuid7JsonConverter() },
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		};
		_json.AddDtoSerializer();
	}

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_host);
	}

	[Fact(DisplayName = "Should create a new report")]
	public async Task CanCreate()
	{
		var given = new FakeReport(_host.Profiles[1], _host.Profiles[5]).Generate();
		var dto = _mapper.Map<MemberModerationReportDto>(given);
		var payload = JsonContent.Create(dto, options: _json);

		var response = await _client.PostAsync($"/lb/v1/reports/{_host.Profiles[1].Id}/report", payload);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<MemberModerationReportDto>(await response.Content.ReadFromJsonAsync<MemberModerationReportDto>(_json));
		Assert.NotNull(actual);
		Assert.Equal(given.Summary, actual.Summary);
	}

	[Fact(DisplayName = "Should get an existing report")]
	public async Task CanCGet()
	{
		var given = _host.Reports[0];

		var response = await _client.GetAsync($"/lb/v1/reports/{_host.Profiles[1].Id}/report/{given.Id}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullModerationReportDto>(await response.Content.ReadFromJsonAsync<FullModerationReportDto>(_json));
		Assert.NotNull(actual);
		Assert.Equal(given.Summary, actual.Summary);
		Assert.Equal(given.Id, actual.Id);
	}

	[Fact(DisplayName = "Should assign a report")]
	public async Task CanAssign()
	{
		var given = _host.Reports[2];
		var response = await _client.PutAsync($"/lb/v1/reports/moderator/report/{given.Id}/assign/{_host.Accounts[0].Id}", null);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullModerationReportDto>(await response.Content.ReadFromJsonAsync<FullModerationReportDto>(_json));
		Assert.NotNull(actual);
		Assert.Contains(_host.Accounts[0].Id, actual.Moderators);
	}

	[Fact(DisplayName = "Should add a remark to a report")]
	public async Task CanAddRemark()
	{
		var report = _host.Reports[1];
		var given = new ModerationRemarkDto
		{
			Report = report.Id,
			Author = _host.Accounts[0].Id,
			Text = $"intgration tests {nameof(CanAddRemark)}"
		};
		var payload = JsonContent.Create(given, options: _json);
		var response = await _client.PostAsync($"/lb/v1/reports/moderator/report/{report.Id}/remark", payload);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullModerationReportDto>(await response.Content.ReadFromJsonAsync<FullModerationReportDto>(_json));
		Assert.NotNull(actual);
		Assert.NotEmpty(actual.Remarks);
		Assert.Equivalent(given, actual.Remarks.FirstOrDefault());
		Assert.NotEqual(given.Id, actual.Remarks.FirstOrDefault()?.Id);
	}

	[Fact(DisplayName = "Should close a report")]
	public async Task CanClose()
	{
		var given = _host.Reports[1];
		var response = await _client.PutAsync($"/lb/v1/reports/moderator/report/{given.Id}/close", null);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullModerationReportDto>(await response.Content.ReadFromJsonAsync<FullModerationReportDto>(_json));
		Assert.NotNull(actual);
		Assert.True(actual.Closed < DateTimeOffset.MaxValue);
	}
}