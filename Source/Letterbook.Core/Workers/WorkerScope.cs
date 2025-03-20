using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Letterbook.Core.Workers;

/// <summary>
/// Hosted services get a singleton scope, which means most of the services we would want to consume won't be available
/// WorkerScope is responsible for creating and managing a scope, which will permit the actual worker to make use of
/// dependency injection as normal
/// </summary>
/// <typeparam name="T">A class encapsulating the worker</typeparam>
public class WorkerScope<T> : IHostedService where T : class, IScopedWorker
{
	private readonly IServiceProvider _serviceProvider;

	public WorkerScope(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		using var scope = _serviceProvider.CreateScope();
		var worker = scope.ServiceProvider.GetRequiredService<T>();

		await worker.DoWork(cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}