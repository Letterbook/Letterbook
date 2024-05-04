using ActivityPub.Types.AS;
using Letterbook.Core.Models;

namespace Letterbook.Core.Adapters;

/// <summary>
/// Deliver an ActivityPub messages to its recipient out of band from the current context.
/// Messages are placed in a work queue, for subsequent processing.
/// This allows ActivityPub Http requests (and responses) to be processed outside the current Asp action, for example.
/// </summary>
public interface IActivityMessage : IEventType
{
	public const string DestinationKey = "destination";
	public const string ProfileKey = "profile";
	public const string ActivityTypesKey = "activity";
	public void Deliver(Uri inbox, ASType activity, Profile? onBehalfOf);
}