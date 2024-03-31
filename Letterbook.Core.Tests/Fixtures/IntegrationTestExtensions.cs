using Letterbook.Core.Extensions;
using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;

namespace Letterbook.Core.Tests.Fixtures;

public static class IntegrationTestExtensions
{
	public static void InitTestData(this IIntegrationTestData data, CoreOptions? options = null)
	{
		var authority = options?.BaseUri().Authority ?? "letterbook.example";
		data.Accounts.AddRange(new FakeAccount(false).Generate(2));
		data.Profiles.AddRange(new FakeProfile(authority, data.Accounts[0]).Generate(3));
		data.Profiles.Add(new FakeProfile(authority, data.Accounts[1]).Generate());
		data.Profiles.AddRange(new FakeProfile().Generate(3));

		// P0 follows P4 and P5
		// P4 follows P0
		data.Profiles[0].Follow(data.Profiles[4], FollowState.Accepted);
		data.Profiles[0].Follow(data.Profiles[5], FollowState.Accepted);
		data.Profiles[0].AddFollower(data.Profiles[4], FollowState.Accepted);

		// Local profiles
		// P0 creates posts 0-2
		data.Posts.Add(data.Profiles[0], new FakePost(data.Profiles[0]).Generate(3));
		// P0 creates post 3, as reply to 2
		data.Posts[data.Profiles[0]].Add(new FakePost(data.Profiles[0], data.Posts[data.Profiles[0]][2]).Generate());
		// P1 creates posts 0-7
		data.Posts.Add(data.Profiles[1], new FakePost(data.Profiles[0], opts: options).Generate(8));
		// P2 creates post 0, and draft 1
		data.Posts.Add(data.Profiles[2], new FakePost(data.Profiles[2]).Generate(1));
		data.Posts[data.Profiles[2]].Add(new FakePost(data.Profiles[2], draft: true).Generate());

		// Remote profiles
		// P4 creates post 0, as reply to post P0:3
		data.Posts.Add(data.Profiles[4], new FakePost(data.Profiles[3], data.Posts[data.Profiles[0]][3]).Generate(1));
	}
}