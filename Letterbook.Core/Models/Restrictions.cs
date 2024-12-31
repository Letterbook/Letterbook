namespace Letterbook.Core.Models;

public enum Restrictions
{
	/// No restrictions
	None,
	/// Click-through warnings on posts/profiles, require approval for follow requests
	Warn,
	/// Reject private messages
	DenyPrivate,
	/// Reject shared posts
	DenyShare,
	/// Reject posts in public/shared audiences
	NoPublic,
	/// Hide posts from non-followers
	NoDiscovery,
	/// Discard all posts (permit profile moves and updates)
	Ignore,
	/// Discard all messages
	Defederate
}