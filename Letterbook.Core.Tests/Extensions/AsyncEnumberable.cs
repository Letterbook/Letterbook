namespace Letterbook.Core.Tests.Extensions;

public static class AsyncEnumberable
{
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> input)
    {
        foreach(var value in input)
        {
            yield return value;
        }
    }
}