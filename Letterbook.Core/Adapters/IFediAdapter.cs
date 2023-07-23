using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IFediAdapter
{
    Task<IObjectRef> FollowLink(Uri href);
}