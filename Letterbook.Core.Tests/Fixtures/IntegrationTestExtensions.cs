using Letterbook.Core.Tests.Fakes;
using Letterbook.Core.Values;

namespace Letterbook.Core.Tests.Fixtures;

public static class IntegrationTestExtensions
{
	public static void InitTestData(this IIntegrationTestData data)
	{
		data.Accounts.AddRange(new FakeAccount(false).Generate(2));
		data.Profiles.AddRange(new FakeProfile("letterbook.example", data.Accounts[0]).Generate(3));
		data.Profiles.Add(new FakeProfile("letterbook.example", data.Accounts[1]).Generate());
		data.Profiles.AddRange(new FakeProfile().Generate(3));

		// P0 follows P4 and P5
		// P4 follows P0
		data.Profiles[0].Follow(data.Profiles[4], FollowState.Accepted);
		data.Profiles[0].Follow(data.Profiles[5], FollowState.Accepted);
		data.Profiles[0].AddFollower(data.Profiles[4], FollowState.Accepted);

		// P0 creates posts 0-2
		data.Posts.Add(data.Profiles[0], new FakePost(data.Profiles[0]).Generate(3));
		// P0 creates post 3, as reply to 2
		data.Posts[data.Profiles[0]].Add(new FakePost(data.Profiles[0], data.Posts[data.Profiles[0]][2]).Generate());
		// P2 creates post 0
		data.Posts.Add(data.Profiles[2], new FakePost(data.Profiles[2]).Generate(1));
		// P4 creates post 0, as reply to P0:3
		data.Posts.Add(data.Profiles[4], new FakePost(data.Profiles[3], data.Posts[data.Profiles[0]][3]).Generate(1));
	}
}