using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityMessageService
{
    public void Deliver(Uri inbox, DTO.Activity activity, Profile? onBehalfOf);
}