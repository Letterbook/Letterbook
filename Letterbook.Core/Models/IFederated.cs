using Medo;

namespace Letterbook.Core.Models;

/// <summary>
/// IFederated is any object that we expect to be dereferenceable between federated peers.
/// </summary>
public interface IFederated
{
	public Uri FediId { get; set; }
	public string Authority { get; }

	public Uuid7 GetId();
	public string GetId25();

	public bool StrictEqual(IFederated other)
	{
		return GetType() == other.GetType() && GetId() == other.GetId() && FediId == other.FediId;
	}
}