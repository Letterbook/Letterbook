using Letterbook.Core.Models;

namespace Letterbook.Core.Tests.Fixtures;

public interface IIntegrationTestData
{
	public List<Profile> Profiles { get; set; }
	public List<Account> Accounts { get; set; }
	public Dictionary<Profile, List<Post>> Posts { get; set; }
}