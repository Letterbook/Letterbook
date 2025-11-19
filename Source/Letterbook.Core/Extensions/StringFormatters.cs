using System.Text.RegularExpressions;

namespace Letterbook.Core.Extensions;

public static partial class StringFormatters
{
	//
	/// <summary>
	/// Split a PascalCase string into the component words, accounting for numbers and initialisms.
	/// Unlikely to work well with non-english characters.
	/// </summary>
	/// <example>
	/// DotNetSDK9 => Dot Net SDK 9
	/// Dotnet9SDK => Dotnet9 SDK
	/// IPAddress => IP Address
	/// </example>
	/// <returns></returns>
	[GeneratedRegex("(^[a-z]+|[A-Z]+(?![a-z])|[A-Z][^A-Z]+)")]
	public static partial Regex SplitExp();
}