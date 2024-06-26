using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public interface IVerificationKeyProvider
{
	Task<Models.SigningKey?> GetKeyByIdAsync(string keyId, CancellationToken cancellationToken = default);
}

public class ActivityPubClientVerificationKeyProvider : IVerificationKeyProvider
{
	private readonly IActivityPubClient _apClient;
	private readonly IHostSigningKeyProvider _hostSigningKeyProvider;

	public ActivityPubClientVerificationKeyProvider(IActivityPubClient apClient, IHostSigningKeyProvider hostSigningKeyProvider)
	{
		_apClient = apClient;
		_hostSigningKeyProvider = hostSigningKeyProvider;
	}

	public async Task<Models.SigningKey?> GetKeyByIdAsync(string keyId, CancellationToken cancellationToken = default)
	{
		var keyIdUri = new Uri(keyId);

		var hostSigningKey = await _hostSigningKeyProvider.GetSigningKey();

		var application = await _apClient.Fetch<Models.IFederatedActor>(keyIdUri, hostSigningKey);

		return application.Keys.FirstOrDefault(k => k.FediId == keyIdUri);
	}
}