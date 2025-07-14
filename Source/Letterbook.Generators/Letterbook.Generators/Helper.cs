namespace Letterbook.Generators;

public class Helper
{
	public const string Attribute =
		"""
		namespace Letterbook.Generators
		{
			[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
			public sealed class GenerateTypedIdAttribute : Attribute;
		}
		""";

	public const string Interface =
		"""
		namespace Letterbook.Generators
		{
			public interface ITypedId<T>
			{
				T Id { get; set; }
				static abstract T FromString(string s);
			}
		}
		""";
}