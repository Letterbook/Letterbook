namespace Letterbook.Api.Tests;


public class SnakeCaseRouteTransformerTest
{
    private SnakeCaseRouteTransformer transformer = new SnakeCaseRouteTransformer();

    [Theory]
    [InlineData("Test", "test")]
    [InlineData("TestCase", "test_case")]
    [InlineData("CPU", "c_p_u")]
    public void ValidateTransformPascalCaseToSnakeCase(string given, string expect)
    {
        Assert.Equal(expect, transformer.TransformOutbound(given));
    }
}