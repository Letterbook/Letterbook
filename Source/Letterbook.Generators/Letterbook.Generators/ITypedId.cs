namespace Letterbook.Generators;

public interface ITypedId<T>
{
	T Id { get; set; }

#if NET // netstandard2.0 doesn't support static abstract members in interfaces, so only include this when building for use at runtime
	static abstract T FromString(string s);
#endif
}