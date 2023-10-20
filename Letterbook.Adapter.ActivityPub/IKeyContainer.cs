namespace Letterbook.Adapter.ActivityPub;

public interface IKeyContainer
{
    public bool TryGetKey(out Models.SigningKey? key);
    public void SetKey(Models.SigningKey key);
}