namespace Letterbook.Core.Adapters;

public interface IActivityMessageService
{
    public void Deliver(DTO.Activity activity, Uri inbox);
}