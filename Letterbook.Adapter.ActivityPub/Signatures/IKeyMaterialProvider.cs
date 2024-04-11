using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Letterbook.Core.Adapters;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public interface IKeyMaterialProvider
{
	Task<Models.SigningKey?> GetKeyByIdAsync(string keyId, CancellationToken cancellationToken = default);
}

public class ActivityPubClientKeyMaterialProvider : IKeyMaterialProvider
{
	private readonly IActivityPubClient _apClient;

	public ActivityPubClientKeyMaterialProvider(IActivityPubClient apClient)
	{
		_apClient = apClient;
	}

	public async Task<Models.SigningKey?> GetKeyByIdAsync(string keyId, CancellationToken cancellationToken = default)
	{
		var keyIdUri = new Uri(keyId);

		var application = await _apClient.Fetch<Models.IFederatedActor>(keyIdUri);

		return application.Keys.FirstOrDefault(k => k.FediId == keyIdUri);
	}
}