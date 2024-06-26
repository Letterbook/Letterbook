using Medo;

namespace Letterbook.Core.Models;

/// <summary>
/// This class represents Letterbook as an Instance Actor. It is intended to be used with
/// <see cref="Letterbook.Core.Adapters.IActivityPubClient.As(InstanceActor)"/> to get a client instance
/// which has its requests signed with the instance actor signing credentials and with
/// <see cref="Letterbook.Core.Adapters.IActivityPubClient.Fetch{T}(Uri)"/> to read another instance actor's public key.
/// </summary>
public class InstanceActor : IFederatedActor, IEquatable<InstanceActor>
{
	public InstanceActor()
	{
	}

	public Uuid7 Id { get; init; } = Uuid7.NewUuid7();
	public required Uri FediId { get; set; }
	public string Authority => FediId.Authority;

	public ActivityActorType Type => ActivityActorType.Application;

	public IList<SigningKey> Keys { get; set; } = new List<SigningKey>();
	public Uuid7 GetId() => Id;

	public string GetId25() => Id.ToId25String();

	public bool Equals(InstanceActor? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Id.Equals(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((InstanceActor)obj);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}
}