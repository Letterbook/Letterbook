using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core;

public interface ITimelineService
{
	public Task HandlePublish(Post post);
	public Task HandleShare(Post post, Profile sharedBy);
	public Task HandleUpdate(Post post, Post oldPost);
	public Task HandleDelete(Post note);
	public Task<IEnumerable<Post>> GetFeed(Uuid7 recipientId, DateTime begin, int limit = 40);
}