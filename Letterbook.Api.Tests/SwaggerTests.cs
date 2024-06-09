namespace Letterbook.Api.Tests;

public class SwaggerTests
{
	[Fact]
	public void CanBuild()
	{
		SwaggerHostFactory.CreateHost();
	}
}