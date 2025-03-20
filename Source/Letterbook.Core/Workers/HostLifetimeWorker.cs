using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Letterbook.Core.Workers;

public class HostLifetimeWorker : IHostedService
{
	private readonly ILogger<HostLifetimeWorker> _logger;
	private readonly IHostApplicationLifetime _appLifetime;
	private readonly IServer _server;

	public HostLifetimeWorker(ILogger<HostLifetimeWorker> logger, IHostApplicationLifetime appLifetime, IServer server)
	{
		_logger = logger;
		_appLifetime = appLifetime;
		_server = server;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_appLifetime.ApplicationStarted.Register(OnStarted);
		_appLifetime.ApplicationStopping.Register(OnStopping);
		_appLifetime.ApplicationStopped.Register(OnStopped);

		return Task.CompletedTask;
	}

	private void OnStopped() => _logger.LogInformation("Application stopped");

	private void OnStopping() => _logger.LogInformation("Application stop scheduled");

	private void OnStarted()
	{
		_logger.LogInformation("Application started");

		if (_server.Features.Get<IServerAddressesFeature>() is not { } feature)
		{
			_logger.LogWarning("Couldn't identity any listening address");
			return;
		}

		foreach (var address in feature.Addresses)
		{
			_logger.LogInformation("Listening on {Address}", address);
		}
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}