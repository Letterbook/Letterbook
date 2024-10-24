using System.Diagnostics;
using System.Net.Http.Headers;
using OpenTelemetry.Context.Propagation;

namespace Letterbook.IntegrationTests.Fixtures;

public static class Traces
{
	public static ActivityTraceId TraceRequest(HttpHeaders carrier)
	{
		var traceId = ActivityTraceId.CreateRandom();
		var activityContext = new ActivityContext(traceId, ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded, traceState: null);
		var propagationContext = new PropagationContext(activityContext, default);

		var f = new TraceContextPropagator();
		f.Inject(propagationContext, carrier, SetHeaders);

		return traceId;
	}

	private static void SetHeaders(HttpHeaders headers, string name, string value)
	{
		headers.Add(name, value);
	}

	public static void SpanIsNotError(Activity? span)
	{
		if (span?.Status == ActivityStatusCode.Error)
			Assert.Fail($"""
			             Error span
			             Source: {span?.Source.Name}
			             DisplayName: {span?.DisplayName}
			             Status: {span?.Status}
			             Duration: {span?.Duration}
			             Kind: {span?.Kind}
			             OperationName: {span?.OperationName}
			             Events: {string.Join("\n\t", span?.Events.SelectMany(e => e.Tags).Select(t => $"{t.Key}: {t.Value}") ?? [])}
			             """);
	}

	public static async Task AssertNoTraceErrors(ActivityTraceId traceId, IAsyncEnumerable<Activity> spans, int timeoutMilliseconds = 350)
	{
		try
		{
			// Wait a little while for any subsequent work to happen, then check for exceptions
			var otherSpans = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMilliseconds));
			var span = await spans
				.Where(a => a.TraceId == traceId)
				.FirstOrDefaultAsync(span => span.Status == ActivityStatusCode.Error, otherSpans.Token);
			SpanIsNotError(span);
		}
		// We expect the otherSpans timeout to trigger. This would mean no error spans were found, which is the desired result.
		catch (TaskCanceledException) { }
		catch (OperationCanceledException) { }
	}


	/// Assert that a span with the expected display name is present
	public static async Task AssertSpan(ActivityTraceId traceId, IAsyncEnumerable<Activity> spans, string expected,
		int timeoutMilliseconds = 1000)
	{
		var trace = await FilterSpans(traceId, spans, expected, timeoutMilliseconds);
		Assert.Contains(expected, trace.Select(s => s.DisplayName));
	}

	/// Assert that a span with the expected display name is not present
	public static async Task AssertNoSpans(ActivityTraceId traceId, IAsyncEnumerable<Activity> spans, string expected,
		int timeoutMilliseconds = 500)
	{
		try
		{
			var trace = await FilterSpans(traceId, spans, expected, timeoutMilliseconds);
			Assert.DoesNotContain(expected, trace.Select(s => s.DisplayName));
		}
		// We expect to hit the timeout. That would mean no matching spans were found, which is the desired result.
		catch (TaskCanceledException) { }
		catch (OperationCanceledException) { }
	}

	private static async Task<List<Activity>> FilterSpans(ActivityTraceId traceId, IAsyncEnumerable<Activity> spans, string expected,
		int timeoutMilliseconds)
	{
		var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMilliseconds));
		var stop = false;
		var trace = await spans.Where(a => a.TraceId == traceId).TakeWhile(s =>
		{
			if (stop) return false;
			if (s.DisplayName == expected)
				stop = true;
			return true;
		}).ToListAsync(cancel.Token);
		return trace;
	}
}