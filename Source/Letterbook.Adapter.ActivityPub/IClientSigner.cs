namespace Letterbook.Adapter.ActivityPub;

public interface IClientSigner
{
	public const string SigningKeysOptionsId = "SigningKeys";
	public const string ProfileOptionsId = "ProfileId";
	public HttpRequestMessage SignRequest(HttpRequestMessage message,
		Models.SigningKey signingKey);
}