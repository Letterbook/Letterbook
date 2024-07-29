namespace Letterbook.DocsSsg.StaticSite;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class StaticPageAttribute : Attribute
{
	public string? Path { get; }
	public StaticPageAttribute(){}
	public StaticPageAttribute(string path)
	{
		Path = path;
	}
}
