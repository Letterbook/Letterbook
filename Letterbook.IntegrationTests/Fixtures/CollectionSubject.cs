using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Letterbook.IntegrationTests.Fixtures;

/// <summary>
/// An Observable Subject that also implements ICollection, so it can be used as the backing collection for
/// OpenTelemetry in-memory exporter
/// </summary>
/// <remarks>Using an Observable for this is useful because the Observable behavior makes it easy to act on spans as they arrive. A regular
/// Collection doesn't do that, and we would need a lot more arbitrary waits and timeouts in order to be confident we had gotten the spans
/// we need.</remarks>
/// <typeparam name="T"></typeparam>
public class CollectionSubject<T> : SubjectBase<T>, ICollection<T>
{
	private readonly ReplaySubject<T> _subject = new();

	public override void Dispose()
	{
		GC.SuppressFinalize(this);
		_subject.Dispose();
	}

	public override void OnCompleted()
	{
		_subject.OnCompleted();
	}

	public override void OnError(Exception error)
	{
		_subject.OnError(error);
	}

	public override void OnNext(T value)
	{
		_subject.OnNext(value);
		Count++;
	}

	public override IDisposable Subscribe(IObserver<T> observer)
	{
		return _subject.Subscribe(observer);
	}

	public override bool HasObservers => _subject.HasObservers;

	public override bool IsDisposed => _subject.IsDisposed;

	public IEnumerator<T> GetEnumerator() => _subject.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => _subject.GetEnumerator();

	public void Add(T item) => _subject.OnNext(item);

	public void Clear()
	{
	}

	public bool Contains(T item) => _subject.Contains(item).FirstAsync().Wait();

	public void CopyTo(T[] array, int arrayIndex) => _subject.ToArray().FirstAsync().Wait().CopyTo(array, arrayIndex);

	public bool Remove(T item) => false;

	public IAsyncEnumerable<T> ToAsyncEnumerable() => _subject.ToAsyncEnumerable();

	public int Count { get; private set; }
	public bool IsReadOnly { get; } = false;
}