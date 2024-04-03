using Letterbook.Core.Models;
using Microsoft.Extensions.Logging;

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
	private readonly ILogger<PostEventService> _logger;

	public PostEventService(ILogger<PostEventService> logger)
	{
		_logger = logger;
	}

	public void Created(Post post)
	{
		_logger.LogWarning($"{nameof(Created)} event not implemented");
	}

	public void Deleted(Post post)
	{
		_logger.LogWarning($"{nameof(Deleted)} event not implemented");
	}

	public void Updated(Post post)
	{
		_logger.LogWarning($"{nameof(Updated)} event not implemented");
	}

	public void Published(Post post)
	{
		_logger.LogWarning($"{nameof(Published)} event not implemented");
	}

	public void Received(Post post, Profile actor)
	{
		_logger.LogWarning($"{nameof(Received)} event not implemented");
	}

	public void Liked(Post post, Profile actor)
	{
		_logger.LogWarning($"{nameof(Liked)} event not implemented");
	}

	public void Shared(Post post, Profile actor)
	{
		_logger.LogWarning($"{nameof(Shared)} event not implemented");
	}
}