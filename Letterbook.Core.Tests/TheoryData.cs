using System.Collections;

namespace Letterbook.Core.Tests;

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

public abstract class TheoryData<T> : TheoryData
	where T: notnull
{
	public void Add(T v)
	{
		AddRow(v);
	}
}

public abstract class TheoryData<T1, T2> : TheoryData
	where T1 : notnull
	where T2 : notnull
{
	public void Add(T1 v1, T2 v2)
	{
		AddRow(v1, v2);
	}
}

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