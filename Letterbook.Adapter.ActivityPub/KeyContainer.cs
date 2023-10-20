namespace Letterbook.Adapter.ActivityPub;

public class KeyContainer : IKeyContainer
{
    private Models.SigningKey? _key;
    
    public bool TryGetKey(out Models.SigningKey? key)
    {
        if (_key != null)
        {
            key = _key;
            return true;
        }

        key = default;
        return false;
    }

    public void SetKey(Models.SigningKey key)
    {
        if (_key != null) throw new Exception();
        _key = key;
    }
}