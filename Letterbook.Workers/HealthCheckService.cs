using System.Net;
using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Letterbook.Workers;

public class HealthcheckListener : BackgroundService
{
	private readonly ILogger<HealthcheckListener> _logger;
	private readonly HttpListener _httpListener;
	private readonly IConfiguration _configuration;
	private readonly HealthCheckService _healthChecks;


	public HealthcheckListener(ILogger<HealthcheckListener> logger, IConfiguration configuration, HealthCheckService healthChecks)
	{
		_logger = logger;
		_configuration = configuration;
		_healthChecks = healthChecks;
		_httpListener = new HttpListener();
	}


	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_httpListener.Prefixes.Add($"http://*:5001/healthz/live/");
		_httpListener.Prefixes.Add($"http://*:5001/healthz/ready/");

		_httpListener.Start();
		_logger.LogInformation($"Healthcheck listening...");

		while (!stoppingToken.IsCancellationRequested)
		{
			var ctx = await _httpListener.GetContextAsync();

			var response = ctx.Response;
			response.ContentType = "text/plain";
			response.Headers.Add(HttpResponseHeader.CacheControl, "no-store, no-cache");
			response.StatusCode = (int)HttpStatusCode.OK;

			var report = await _healthChecks.CheckHealthAsync(stoppingToken);
			// if (report)
			var message = $"{report}";
			var messageBytes = Encoding.UTF8.GetBytes("Healthy");
			response.ContentLength64 = messageBytes.Length;
			await response.OutputStream.WriteAsync(messageBytes, 0, messageBytes.Length);
			response.OutputStream.Close();
			response.Close();
		}
	}
}