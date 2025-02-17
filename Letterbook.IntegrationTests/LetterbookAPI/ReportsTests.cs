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

	[Fact(DisplayName = "Should assign a report")]
	public async Task CanAssign()
	{
		var given = _host.Reports[2];
		var response = await _client.PutAsync($"/lb/v1/reports/moderator/report/{given.Id}/assign/{_host.Accounts[0].Id}", null);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		var actual = Assert.IsType<FullModerationReportDto>(await response.Content.ReadFromJsonAsync<FullModerationReportDto>(_json));
		Assert.NotNull(actual);
		Assert.Contains(_host.Accounts[0].Id, actual.Moderators);
		Assert.Fail("todo");
	}

	// [Fact(DisplayName = "Should update an existing report")]
	// public async Task CanUpdate()
	// {
	// 	var given = new FakeReport().Generate();
	// 	var dto = _mapper.Map<FullModerationReportDto>(given);
	// 	AuthzModerationServiceMock.Setup(m => m.UpdateReport(given.Id, It.IsAny<Models.ModerationReport>()))
	// 		.ReturnsAsync(given);
	//
	// 	var result = await _controller.UpdateReport(given.Id, dto);
	// 	var response = Assert.IsType<OkObjectResult>(result);
	// 	var actual = Assert.IsType<FullModerationReportDto>(response.Value);
	// 	Assert.Equal(given.Id, actual.Id);
	//
	// 	Assert.NotNull(actual);
	// 	AuthzModerationServiceMock.Verify(m => m.UpdateReport(given.Id, It.Is<Models.ModerationReport>(r => r.Summary == dto.Summary)));
	// }
	//
	// [Fact(DisplayName = "Should lookup an existing report")]
	// public async Task CanLookup()
	// {
	// 	var given = new FakeReport().Generate();
	// 	AuthzModerationServiceMock.Setup(m => m.LookupReport(given.Id)).ReturnsAsync(given);
	//
	// 	var result = await _controller.LookupReport(Uuid7.NewUuid7(), given.Id);
	// 	var response = Assert.IsType<OkObjectResult>(result);
	// 	var actual = Assert.IsType<FullModerationReportDto>(response.Value);
	//
	// 	Assert.Equal(given.Id, actual.Id);
	// 	AuthzModerationServiceMock.Verify(m => m.LookupReport(given.Id));
	// }
	//
	// [Fact(DisplayName = "Should add a remark to an existing report")]
	// public async Task CanAddRemark()
	// {
	// 	var given = new FakeReport().Generate();
	// 	var remark = new ModerationRemarkDto
	// 	{
	// 		Report = given.Id,
	// 		Author = Guid.NewGuid(),
	// 		Text = "test remark"
	// 	};
	// 	AuthzModerationServiceMock.Setup(m => m.AddRemark(given.Id, It.IsAny<Models.ModerationRemark>())).ReturnsAsync(given);
	//
	// 	var result = await _controller.Remark(given.Id, remark);
	// 	var response = Assert.IsType<OkObjectResult>(result);
	// 	var actual = Assert.IsType<FullModerationReportDto>(response.Value);
	//
	// 	Assert.Equal(given.Id, actual.Id);
	// 	AuthzModerationServiceMock.Verify(m => m.AddRemark(given.Id, It.Is<Models.ModerationRemark>(r => r.Text == remark.Text)));
	// }
	//
	// [InlineData(true)]
	// [InlineData(false)]
	// [Theory(DisplayName = "Should assign a moderator to an existing report")]
	// public async Task CanAssignModerator(bool assign)
	// {
	// 	var given = new FakeReport().Generate();
	// 	var mod = Guid.NewGuid();
	// 	AuthzModerationServiceMock.Setup(m => m.AssignModerator(given.Id, mod, assign)).ReturnsAsync(given);
	//
	// 	var result = await _controller.Assign(given.Id, mod, assign);
	// 	var response = Assert.IsType<OkObjectResult>(result);
	// 	var actual = Assert.IsType<FullModerationReportDto>(response.Value);
	//
	// 	Assert.Equal(given.Id, actual.Id);
	// 	AuthzModerationServiceMock.Verify();
	// }
	//
	// [InlineData(true)]
	// [InlineData(false)]
	// [Theory(DisplayName = "Should close an existing report")]
	// public async Task CanClose(bool close)
	// {
	// 	var given = new FakeReport().Generate();
	// 	AuthzModerationServiceMock.Setup(m => m.CloseReport(given.Id, close)).ReturnsAsync(given);
	//
	// 	var result = await _controller.Close(given.Id, close);
	// 	var response = Assert.IsType<OkObjectResult>(result);
	// 	var actual = Assert.IsType<FullModerationReportDto>(response.Value);
	//
	// 	Assert.Equal(given.Id, actual.Id);
	// 	AuthzModerationServiceMock.Verify();
	// }
	//
	// [ClassData(typeof(ListTheoryData))]
	// [Theory(DisplayName = "Should list existing reports")]
	// public async Task CanList(bool closed, Models.ProfileId? subjectId, Models.ProfileId? reporterId, Guid? moderatorId)
	// {
	// 	var given = new FakeReport().Generate(2);
	// 	if (moderatorId is {} moderator)
	// 		AuthzModerationServiceMock.Setup(m => m.FindAssigned(moderator, closed)).Returns(given.ToAsyncEnumerable());
	// 	if (subjectId is {} subject)
	// 		AuthzModerationServiceMock.Setup(m => m.FindRelatedTo(subject, closed)).Returns(given.ToAsyncEnumerable());
	// 	if (reporterId is {} reporter)
	// 		AuthzModerationServiceMock.Setup(m => m.FindCreatedBy(reporter, closed)).Returns(given.ToAsyncEnumerable());
	//
	// 	var result = _controller.ListReports(subjectId, reporterId, moderatorId, closed);
	// 	var response = Assert.IsType<OkObjectResult>(result);
	// 	var actual = Assert.IsAssignableFrom<IAsyncEnumerable<FullModerationReportDto>>(response.Value);
	//
	// 	Assert.Equal(2, await actual.CountAsync());
	// 	AuthzModerationServiceMock.Verify();
	// }
}