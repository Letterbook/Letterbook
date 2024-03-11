using Letterbook.Core.Models;

namespace Letterbook.Core;

public interface IPostEventService
{
    public void Created(Post post);
    public void Deleted(Post post);
    public void Updated(Post post);
    public void Published(Post post);
    public void Received(Post post, Profile actor);
    public void Liked(Post post, Profile actor);
    public void Shared(Post post, Profile actor);

}

public class PostEventService : IPostEventService
{
	public void Created(Post post)
	{
		throw new NotImplementedException();
	}

	public void Deleted(Post post)
	{
		throw new NotImplementedException();
	}

	public void Updated(Post post)
	{
		throw new NotImplementedException();
	}

	public void Published(Post post)
	{
		throw new NotImplementedException();
	}

	public void Received(Post post, Profile actor)
	{
		throw new NotImplementedException();
	}

	public void Liked(Post post, Profile actor)
	{
		throw new NotImplementedException();
	}

	public void Shared(Post post, Profile actor)
	{
		throw new NotImplementedException();
	}
}