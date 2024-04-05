using Medo;

namespace Letterbook.Core.Models;

public class Application : IFederated, IEquatable<Application>
{
	private Uuid7 _id;

	public Application()
	{
		_id = Uuid7.NewUuid7();
	}
	public required Uri FediId { get; set; }
	public string Authority => FediId.Authority;

	public ActivityActorType Type => ActivityActorType.Application;

	public required Uri Inbox { get; set; }
	public required Uri Outbox { get; set; }
	public required string Handle { get; set; }
	public IList<SigningKey> Keys { get; set; } = new List<SigningKey>();
	public Uuid7 GetId() => _id;

	public string GetId25() => _id.ToId25String();

	public bool Equals(Application? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return _id.Equals(other._id);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((Application)obj);
	}

	public override int GetHashCode()
	{
		return _id.GetHashCode();
	}
}