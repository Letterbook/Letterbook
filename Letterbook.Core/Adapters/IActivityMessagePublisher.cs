using ActivityPub.Types.AS;
using Letterbook.Core.Events;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Deliver an ActivityPub message to its recipient out of band from the current context.
/// Messages are placed in a work queue for subsequent processing.
/// This allows ActivityPub Http requests (and responses) to be processed outside the current Asp action, for example.
/// </summary>
public interface IActivityMessagePublisher : IEventChannel
{
	public const string DestinationKey = "destination";
	public const string ProfileKey = "profile";
	public const string ActivityTypesKey = "activity";
	public Task Deliver(Uri inbox, ASType activity, Profile? onBehalfOf);
}