//HintName: ITypedId.g.cs
namespace Letterbook.Generators
{
	public interface ITypedId<T>
	{
		T Id { get; set; }
		static abstract T FromString(string s);
	}
}