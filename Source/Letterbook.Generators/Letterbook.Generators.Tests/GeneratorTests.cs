using System.Threading;
using System.Threading.Tasks;
using Medo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using Xunit;

namespace Letterbook.Generators.Tests;

public class GeneratorTests
{
	private const string TestInt =
		"""
		namespace Letterbook.Generators.Tests;
		[GenerateTypedId]
		public partial record TestIdInt(int Id);
		""";

	private const string TestUuid7 =
		"""
		using medo;

		namespace Letterbook.Generators.Tests;
		[GenerateTypedId]
		public partial record struct TestIdUuid(Uuid7 Id);
		""";

	public GeneratorTests()
	{
	}

	[Fact]
	public async Task Test_Int()
	{
		var generator = new TypedIdGenerator();
		// Source generators should be tested using 'GeneratorDriver'
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
		// Build a syntax tree and feed it to the compilation
		// A CSharpCompilation represents an invocation of the compiler. The compiler is normally invoked repeatedly over previous
		// compilations to produce a final build. Each compilation is immutable, and should serve as input to the next compilation
		var compilation = CSharpCompilation.Create(nameof(GeneratorTests), [CSharpSyntaxTree.ParseText(TestInt)]);

		// At this point, the TypedIds should have been compiled, but the generator hasn't been run. So we haven't actually generated any
		// new source files.
		// Run it now to generate new source code.
		driver = driver.RunGenerators(compilation, CancellationToken.None);

		// Verify the generate files are as expected
		await Verifier.Verify(driver);
	}

	[Fact]
	public async Task Test_Uuid7()
	{
		var generator = new TypedIdGenerator();
		GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
		var compilation = CSharpCompilation.Create(
			nameof(GeneratorTests),
			[CSharpSyntaxTree.ParseText(TestUuid7)],
			[MetadataReference.CreateFromFile(typeof(Uuid7).Assembly.Location)]);

		driver = driver.RunGenerators(compilation, CancellationToken.None);

		await Verifier.Verify(driver);
	}
}