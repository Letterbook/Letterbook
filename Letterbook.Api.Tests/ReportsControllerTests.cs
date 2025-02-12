using AutoMapper;
using Letterbook.Api.Controllers;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Core.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit.Abstractions;

namespace Letterbook.Api.Tests;

public class ReportsControllerTests : WithMockContext
{
	private readonly ReportsController _controller;
	private readonly IMapper _mapper;

	public ReportsControllerTests(ITestOutputHelper output)
	{
		output.WriteLine($"Bogus seed: {Init.WithSeed()}");
		Auth();
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
	}
}