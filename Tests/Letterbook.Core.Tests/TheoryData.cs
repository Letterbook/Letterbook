using System.Collections;

namespace Letterbook.Core.Tests;

/// <summary>
/// A support class that allows us to have more idiomatic and type-safe ClassData classes to drive XUnit Theory tests
/// ClassData implementations should inherit one of the generic derivatives of this class.
/// </summary>
public abstract class TheoryData : IEnumerable<object[]>
{
	readonly List<object[]> data = new();

	protected void AddRow(params object[] values)
	{
		data.Add(values);
	}

	public IEnumerator<object[]> GetEnumerator()
	{
		return data.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

/// <summary>
/// Theory data with a single parameter of type T
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TheoryData<T> : TheoryData
	where T: notnull
{
	public void Add(T v)
	{
		AddRow(v);
	}
}

/// <summary>
/// Theory data with two parameters
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public abstract class TheoryData<T1, T2> : TheoryData
	where T1 : notnull
	where T2 : notnull
{
	public void Add(T1 v1, T2 v2)
	{
		AddRow(v1, v2);
	}
}

/// <summary>
/// Theory data wtih three parameters
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="T3"></typeparam>
public abstract class TheoryData<T1, T2, T3> : TheoryData
	where T1 : notnull
	where T2 : notnull
	where T3 : notnull
{
	public void Add(T1 v1, T2 v2, T3 v3)
	{
		AddRow(v1, v2, v3);
	}
}