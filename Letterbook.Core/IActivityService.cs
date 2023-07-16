namespace Letterbook.Core;

public interface IActivityService
{
    DTO.Activity Create();
    Task<bool> Receive(DTO.Activity activity);
    void Deliver(DTO.Activity activity);
}