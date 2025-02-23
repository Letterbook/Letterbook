using AutoMapper;
using Bogus;
using Letterbook.Api.Controllers;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests.Fakes;
using Medo;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class ReportsControllerTests : WithMockContext
{
	private readonly ReportsController _controller;
	private readonly IMapper _mapper;
	private readonly Guid _accountId;

	public ReportsControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		_accountId = new Faker().Random.Guid();
		Auth(_accountId);
		var maps = new MappingConfigProvider(CoreOptionsMock);
		_mapper = new Mapper(maps.ModerationReports);

		MockAuthorizeAllowAll();
		_controller = new ReportsController(maps, ModerationServiceMock.Object,
			AuthorizationServiceMock.Object)
		{
			ControllerContext = new ControllerContext()
			{
				HttpContext = MockHttpContext.Object
			}
		};
	}

	#region TheoryData
	class ListTheoryData : TheoryData<bool,Models.ProfileId?, Models.ProfileId?, Guid?>
	{
		public ListTheoryData()
		{
			for (int i = 0; i < 2; i++)
			{
				var closed = i % 2 == 0;
				for (int j = 0; j < 2; j++)
				{
					Uuid7? subject = j % 2 == 0 ? Uuid7.NewUuid7() : null;
					for (int k = 0; k < 2; k++)
					{
						Uuid7? reporter = k % 2 == 0 ? Uuid7.NewUuid7() : null;
						for (int l = 0; l < 2; l++)
						{
							Guid? moderator = l % 2 == 0 ? Guid.NewGuid() : null;

							if (subject is not null || reporter is not null || moderator is not null)
								Add(closed, subject, reporter, moderator);
						}
					}
				}
			}
		}
	}
	#endregion

	[Fact]
	public void Exists()
	{
		Assert.NotNull(_controller);
	}

	[Fact(DisplayName = "Should create a new report")]
	public async Task CanCreate()
	{
		var given = new FakeReport().Generate();
		var dto = _mapper.Map<MemberModerationReportDto>(given);
		AuthzModerationServiceMock.Setup(m => m.CreateReport(dto.Reporter, It.IsAny<Models.ModerationReport>()))
			.ReturnsAsync(given);

		var result = await _controller.CreateReport(dto.Reporter, dto);
		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullModerationReportDto>(response.Value);
		Assert.Equal(given.Id, actual.Id);

		Assert.NotNull(actual);
		AuthzModerationServiceMock.Verify(m => m.CreateReport(dto.Reporter, It.Is<Models.ModerationReport>(r => r.Summary == dto.Summary)));
	}

	[Fact(DisplayName = "Should update an existing report")]
	public async Task CanUpdate()
	{
		var given = new FakeReport().Generate();
		var dto = _mapper.Map<FullModerationReportDto>(given);
		AuthzModerationServiceMock.Setup(m => m.UpdateReport(given.Id, It.IsAny<Models.ModerationReport>(), It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.ReturnsAsync(given);

		var result = await _controller.UpdateReport(given.Id, dto);
		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullModerationReportDto>(response.Value);
		Assert.Equal(given.Id, actual.Id);

		Assert.NotNull(actual);
		AuthzModerationServiceMock.Verify(m => m.UpdateReport(given.Id, It.Is<Models.ModerationReport>(r => r.Summary == dto.Summary), It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()));
	}

	[Fact(DisplayName = "Should lookup an existing report")]
	public async Task CanLookup()
	{
		var given = new FakeReport().Generate();
		AuthzModerationServiceMock.Setup(m => m.LookupReport(given.Id)).ReturnsAsync(given);

		var result = await _controller.LookupReport(Uuid7.NewUuid7(), given.Id);
		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullModerationReportDto>(response.Value);

		Assert.Equal(given.Id, actual.Id);
		AuthzModerationServiceMock.Verify(m => m.LookupReport(given.Id));
	}

	[Fact(DisplayName = "Should add a remark to an existing report")]
	public async Task CanAddRemark()
	{
		var given = new FakeReport().Generate();
		var remark = new ModerationRemarkDto
		{
			Report = given.Id,
			Author = Guid.NewGuid(),
			Text = "test remark"
		};
		AuthzModerationServiceMock.Setup(m => m.AddRemark(given.Id, It.IsAny<Models.ModerationRemark>())).ReturnsAsync(given);

		var result = await _controller.Remark(given.Id, remark);
		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullModerationReportDto>(response.Value);

		Assert.Equal(given.Id, actual.Id);
		AuthzModerationServiceMock.Verify(m => m.AddRemark(given.Id, It.Is<Models.ModerationRemark>(r => r.Text == remark.Text)));
	}

	[InlineData(true)]
	[InlineData(false)]
	[Theory(DisplayName = "Should assign a moderator to an existing report")]
	public async Task CanAssignModerator(bool assign)
	{
		var given = new FakeReport().Generate();
		var mod = Guid.NewGuid();
		AuthzModerationServiceMock.Setup(m => m.AssignModerator(given.Id, mod, assign)).ReturnsAsync(given);

		var result = await _controller.Assign(given.Id, mod, assign);
		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullModerationReportDto>(response.Value);

		Assert.Equal(given.Id, actual.Id);
		AuthzModerationServiceMock.Verify();
	}

	[InlineData(true)]
	[InlineData(false)]
	[Theory(DisplayName = "Should close an existing report")]
	public async Task CanClose(bool close)
	{
		var given = new FakeReport().Generate();
		AuthzModerationServiceMock.Setup(m => m.CloseReport(given.Id, _accountId, close)).ReturnsAsync(given);

		var result = await _controller.Close(given.Id, close);
		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsType<FullModerationReportDto>(response.Value);

		Assert.Equal(given.Id, actual.Id);
		AuthzModerationServiceMock.Verify();
	}

	[ClassData(typeof(ListTheoryData))]
	[Theory(DisplayName = "Should list existing reports")]
	public async Task CanList(bool closed, Models.ProfileId? subjectId, Models.ProfileId? reporterId, Guid? moderatorId)
	{
		var given = new FakeReport().Generate(2);
		if (moderatorId is {} moderator)
			AuthzModerationServiceMock.Setup(m => m.FindAssigned(moderator, closed)).Returns(given.ToAsyncEnumerable());
		if (subjectId is {} subject)
			AuthzModerationServiceMock.Setup(m => m.FindRelatedTo(subject, closed)).Returns(given.ToAsyncEnumerable());
		if (reporterId is {} reporter)
			AuthzModerationServiceMock.Setup(m => m.FindCreatedBy(reporter, closed)).Returns(given.ToAsyncEnumerable());

		var result = _controller.ListReports(subjectId, reporterId, moderatorId, closed);
		var response = Assert.IsType<OkObjectResult>(result);
		var actual = Assert.IsAssignableFrom<IAsyncEnumerable<FullModerationReportDto>>(response.Value);

		Assert.Equal(2, await actual.CountAsync());
		AuthzModerationServiceMock.Verify();
	}
}