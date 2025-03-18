using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Letterbook.Generators;

public static class Extensions
{
	/// <summary>Indicates whether or not the class has a specific interface.</summary>
	/// <returns>Whether or not the SyntaxList contains the attribute.</returns>
	public static bool HasInterface(this TypeDeclarationSyntax source, string interfaceName)
	{
		IEnumerable<BaseTypeSyntax> baseTypes = source.BaseList?.Types.Select(baseType => baseType) ?? [];

		return baseTypes.Any(baseType => baseType.ToString().Contains(interfaceName));
	}
}