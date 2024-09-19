using System.Diagnostics;
using System.Net.Http.Headers;
using OpenTelemetry.Context.Propagation;

namespace Letterbook.IntegrationTests.Fixtures;

public static class Traces
{
	public static ActivityTraceId TraceRequest(HttpContent request)
	{
		var traceId = ActivityTraceId.CreateRandom();
		var activityContext = new ActivityContext(traceId, ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded, traceState: null);
		var propagationContext = new PropagationContext(activityContext, default);
		var carrier = request.Headers;

		var f = new TraceContextPropagator();
		f.Inject(propagationContext, carrier, SetHeaders);

		return traceId;
	}

	private static void SetHeaders(HttpContentHeaders headers, string name, string value)
	{
		headers.Add(name, value);
	}

	public static async Task<List<Activity>> AssertNoErrors(ActivityTraceId traceId, IAsyncEnumerable<Activity> spans, TimeSpan timeout)
	{
		var list = new List<Activity>();
		await foreach (var a in spans)
		{
			list.Add(a);
			if (a.Status == ActivityStatusCode.Error)
				Assert.Null(a);
		}

		await Task.Delay(timeout);
		return list;
	}
}