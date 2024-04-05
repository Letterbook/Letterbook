using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public interface IKeyMaterialProvider
{
	Task<X509Certificate2?> GetKeyByIdAsync(string keyId, CancellationToken cancellationToken = default);
}

public class ActivityPubClientKeyMaterialProvider : IKeyMaterialProvider
{
	private readonly IActivityPubClient _apClient;

	public ActivityPubClientKeyMaterialProvider(IActivityPubClient apClient)
	{
		_apClient = apClient;
	}

	public async Task<X509Certificate2?> GetKeyByIdAsync(string keyId, CancellationToken cancellationToken = default)
	{
		var keyIdUri = new Uri(keyId);

		var application = await _apClient.Fetch<Models.Application>(new Uri(keyId));
		var matchingKey = application.Keys.FirstOrDefault(k => k.FediId == keyIdUri);
		if (matchingKey == null)
		{
			return null;
		}

		return ReadKey(matchingKey);
	}

	private static X509Certificate2? ReadKey(Models.SigningKey matchingKey)
	{
		var keyChars = MemoryMarshal.Cast<byte, char>(matchingKey.PublicKey.Span);

		return X509Certificate2.CreateFromPem(keyChars);
	}
}