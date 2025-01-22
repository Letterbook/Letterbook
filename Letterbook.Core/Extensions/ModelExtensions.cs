using Letterbook.Core.Models;

namespace Letterbook.Core.Extensions;

public static class ModelExtensions
{
	public static void ConvergeMentions(this Post post, Dictionary<ProfileId, Profile> profiles)
	{
		post.AddressedTo = post.AddressedTo.Where(mention => profiles.ContainsKey(mention.Subject.Id)).ToHashSet();
		foreach (var mention in post.AddressedTo)
		{
			if (profiles.TryGetValue(mention.Subject.Id, out var profile))
			{
				mention.Subject = profile;
			}
		}
	}
}