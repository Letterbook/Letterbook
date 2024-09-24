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
			             Events: {string.Join("\n\t", span?.Events.SelectMany(e => e.Tags).Select(t => $"{t.Key}: {t.Value}") ?? [] )}
			             """);
	}
}