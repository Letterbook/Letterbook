using System.Text.Json;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Letterbook.Core.Adapters;

public interface IHostSigningKeyProvider
{
	Task<SigningKey> GetSigningKey();
}

public class DevelopmentHostSigningKeyProvider : IHostSigningKeyProvider
{
	private readonly ILogger<DevelopmentHostSigningKeyProvider> _logger;
	private readonly IHostEnvironment _webHostEnvironment;
	private readonly IOptions<CoreOptions> _coreOptions;
	private readonly Lazy<Task<SigningKey>> _lazyKey;
	public DevelopmentHostSigningKeyProvider(
		ILogger<DevelopmentHostSigningKeyProvider> logger,
		IHostEnvironment webHostEnvironment,
		IOptions<CoreOptions> coreOptions)
	{
		_logger = logger;
		_webHostEnvironment = webHostEnvironment;
		_coreOptions = coreOptions;
		_lazyKey = new Lazy<Task<SigningKey>>(GetOrCreateDevelopmentKey);
	}

	public async Task<SigningKey> GetSigningKey() => await _lazyKey.Value;

	private static readonly string DevelopmentKeyFileName = "development.key";
	private async Task<SigningKey> GetOrCreateDevelopmentKey()
	{
		var keyFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, DevelopmentKeyFileName);

		var existingKey = await ReadDevelopmentKey(keyFilePath);

		return existingKey ?? await GenerateDevelopmentKey(keyFilePath);
	}

	private async Task<SigningKey?> ReadDevelopmentKey(string keyFilePath)
	{
		try
		{
			_logger.LogDebug("Looking for development host signing key at {KeyFilePath}", keyFilePath);
			using var fileStream = File.OpenRead(keyFilePath);
			var deserializedKey = await JsonSerializer.DeserializeAsync<SigningKey>(fileStream);
			if (deserializedKey != null)
			{
				_logger.LogDebug("Found existing signing key with ID {Id}", deserializedKey.FediId);
			}

			return deserializedKey;
		}
		catch (IOException ioEx)
		{
			_logger.LogDebug("Development key file does not exist on disk or is not readable ({Exception})", ioEx.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogError("Error deserializing development key: {Exception}", ex);
		}

		return null;
	}

	private async Task<SigningKey> GenerateDevelopmentKey(string keyFilePath)
	{
		_logger.LogDebug("Generating new development host signing key");
		var key = SigningKey.Rsa(0, _coreOptions.Value.BaseUri());
		try
		{
			using var fileStream = File.OpenWrite(keyFilePath);
			_logger.LogDebug("Writing generated signing key to {KeyFilePath}", keyFilePath);
			await JsonSerializer.SerializeAsync(fileStream, key);
		}
		catch (Exception ex)
		{
			_logger.LogError("Error serializing development key: {Exception}", ex);
		}

		return key;
	}
}