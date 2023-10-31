namespace Letterbook.Adapter.ActivityPub;

/// <summary>
/// Keep this one
/// </summary>
public interface IClientSigner
{
    public HttpRequestMessage SignRequest(HttpRequestMessage message,
        Models.SigningKey signingKey);
}