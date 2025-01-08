namespace Letterbook.Core.Models;

public enum Restrictions
{
	/// No restrictions
	None,
	/// Click-through warnings on posts/profiles, require approval for follow requests
	Warn,
	/// Hide posts from non-followers
	LimitDiscovery,
	/// Reject private messages
	DenyPrivate,
	/// Reject posts in public/shared audiences
	DenyPublic,
	/// Reject shared posts
	DenyShare,
	/// Discard all images, audio, video, quote posts, etc
	DenyAttachments,
	/// Discard all links
	DenyLinks,
	/// Discard all replies (permit top-level posts and self-replies)
	DenyReplies,
	/// Discard all posts (permit profile moves and updates)
	DenyPosts,
	/// Discard all messages
	Defederate
}