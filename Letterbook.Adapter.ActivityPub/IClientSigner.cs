namespace Letterbook.Adapter.ActivityPub;

public interface IClientSigner
{
    public const string SigningKeysOptionsId = "SigningKeys";
    public HttpRequestMessage SignRequest(HttpRequestMessage message,
        Models.SigningKey signingKey);
}