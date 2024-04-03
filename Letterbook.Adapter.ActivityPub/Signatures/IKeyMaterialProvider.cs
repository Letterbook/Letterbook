using System.Security.Cryptography.X509Certificates;

namespace Letterbook.Adapter.ActivityPub.Signatures;

public interface IKeyMaterialProvider
{
	Task<X509Certificate2?> GetKeyByIdAsync(string? signatureParamsKeyId);
}