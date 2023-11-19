using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

public interface IActivityBuilder
{
    public DTO.Activity Approve(Profile actor, Uri objectId);
    public DTO.Activity Reject(Profile actor, Uri objectId);
    public DTO.Activity Add(Profile actor, Uri objectId, Uri collectionId);
    public DTO.Activity Remove(Profile actor, Uri objectId, Uri collectionId);
}