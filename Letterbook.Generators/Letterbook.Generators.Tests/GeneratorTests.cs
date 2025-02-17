using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Letterbook.Generators.Tests;

public class GeneratorTests
{
	private const string TestIdRecords =
		"""
		namespace Letterbook.Generators;

		public interface ITypedId<T>
		{
			T Id { get; }
		}

		public partial record struct TestIdInt(int Id) : ITypedId<int>;
		public partial record struct TestIdUuid(Uuid7 Id) : ITypedId<Uuid7>;
		""";

	private static readonly string[] ExpectedFiles = new[]
	{
		"TestIdInt.g.cs",
		"TestIdUuid.g.cs",
		"TestIdIntConverter.g.cs",
		"TestIdUuidConverter.g.cs",
		"TestIdUuidJsonConverter.g.cs",
		"TestIdIntJsonConverter.g.cs",
		"TestIdUuidEfConverter.g.cs",
		"TestIdIntEfConverter.g.cs",
		"TestIdUuidSwaggerSchemaFilter.g.cs",
		"TestIdIntSwaggerSchemaFilter.g.cs",
	};

	[Fact]
	public void GenerateTypedIdFiles()
	{
		var generator = new TypedIdGenerator();
		// Source generators should be tested using 'GeneratorDriver'
		var driver = CSharpGeneratorDriver.Create(generator);
		// Build a syntax tree and feed it to the compilation
		// A CSharpCompilation represents an invocation of the compiler. The compiler is normally invoked repeatedly over previous
		// compilations to produce a final build. Each compilation is immutable, and should serve as input to the next compilation
		var compilation = CSharpCompilation.Create(nameof(GeneratorTests), [CSharpSyntaxTree.ParseText(TestIdRecords)]);

		// At this point, the TypedIds should have been compiled, but the generator hasn't been run. So we haven't actually generated any
		// new source files.
		// Run it now to generate new source code.
		driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);

		// The starting compilation is immutable, so we make assertions on newCompilation
		var generatedFiles = newCompilation.SyntaxTrees
			.Select(t => Path.GetFileName(t.FilePath))
			.Where(p => !string.IsNullOrEmpty(p))
			.ToArray();

		// Assert that the expected source files were generated
		Assert.Equivalent(ExpectedFiles, generatedFiles, strict: true);
	}

	[Fact(Skip = "not needed")]
	public void GenerateTypedIdConverter()
	{
		// We could also get the text of the generated source files and make assertions about those.
		// But they're not that involved, so we don't gain much from doing that.
		// It would look like this:

		// var generator = new TypedIdGenerator();
		// var driver = CSharpGeneratorDriver.Create(generator);
		// var compilation = CSharpCompilation.Create(nameof(GeneratorTests), [CSharpSyntaxTree.ParseText(TestIdRecords)]);
		//
		// // Run generators and retrieve the results.
		// var runResult = driver.RunGenerators(compilation).GetRunResult();
		//
		// // Get our actual syntax tree from the generated trees.
		// var actualSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("TestIdInt.g.cs"));
		//
		// Assert.Equal(TODO_EXPECTED_CLASS_TEXT, actualSyntax.GetText().ToString(),
		// 	ignoreLineEndingDifferences: true);
	}
}