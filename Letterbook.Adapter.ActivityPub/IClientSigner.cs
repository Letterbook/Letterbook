namespace Letterbook.Adapter.ActivityPub;

/// <summary>
/// Keep this one
/// </summary>
public interface IClientSigner
{
    public HttpRequestMessage SignRequest(Models.SigningKey signingKey, string keyId,
        HttpRequestMessage message, string signatureId);
}