using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IFediAdapter : IAdapter
{
    Task<IObjectRef> FollowLink(Uri href);
}