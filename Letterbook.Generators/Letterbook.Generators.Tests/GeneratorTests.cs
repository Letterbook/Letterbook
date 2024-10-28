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

		// Run generators
		driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);

		// The starting compilation is immutable, so we make assertions on newCompilation
		var generatedFiles = newCompilation.SyntaxTrees
			.Select(t => Path.GetFileName(t.FilePath))
			.Where(p => !string.IsNullOrEmpty(p))
			.ToArray();

		Assert.Equivalent(ExpectedFiles, generatedFiles, strict: true);
	}
}