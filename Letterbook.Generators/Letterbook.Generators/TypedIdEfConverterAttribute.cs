using System;

namespace Letterbook.Generators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class TypedIdEfConverterAttribute<T> : TypedIdEfConverterAttribute
{
	public override Type IdType { get; } = typeof(T);
}

public abstract class TypedIdEfConverterAttribute : Attribute
{
	public abstract Type IdType { get; }
}