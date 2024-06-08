using System.Security.Claims;
using Letterbook.Core.Models;
using Medo;

namespace Letterbook.Core;

public interface ITimelineService
{
	public IAuthzTimelineService As(IEnumerable<Claim> claims);
}

public interface IAuthzTimelineService
{
	public Task HandlePublish(Post post);
	public Task HandleShare(Post post, Profile sharedBy);
	public Task HandleUpdate(Post post, Post oldPost);
	public Task HandleDelete(Post note);
	public Task<IEnumerable<Post>> GetFeed(Uuid7 profileId, DateTimeOffset begin, int limit = 40);
}