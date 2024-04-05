using System.Security.Cryptography.X509Certificates;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public interface IKeyMaterialProvider
{
	Task<X509Certificate2?> GetKeyByIdAsync(string? signatureParamsKeyId, CancellationToken cancellationToken = default);
}

public class HttpClientKeyMaterialProvider : IKeyMaterialProvider
{
	public Task<X509Certificate2?> GetKeyByIdAsync(string? signatureParamsKeyId, CancellationToken cancellationToken = default)
	{
		return Task.FromResult<X509Certificate2?>(null);
	}
}