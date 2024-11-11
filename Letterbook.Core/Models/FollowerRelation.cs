using Letterbook.Core.Values;
using Medo;

namespace Letterbook.Core.Models;

public class FollowerRelation : IEquatable<FollowerRelation>
{
	private Uuid7 _id;

	public Guid Id
	{
		get => _id.ToGuid();
		set => _id = Uuid7.FromGuid(value);
	}

	/// <summary>
	/// This Profile is following another
	/// </summary>
	public Profile Follower { get; set; }
	/// <summary>
	/// The Profile that is being followed
	/// </summary>
	public Profile Follows { get; set; }
	public FollowState State { get; set; }
	public DateTimeOffset Date { get; set; }

	private FollowerRelation()
	{
		_id = Uuid7.NewUuid7();
		Follower = default!;
		Follows = default!;
		State = default;
		Date = DateTime.UtcNow;
	}

	public FollowerRelation(Profile follower, Profile follows, FollowState state)
	{
		_id = Uuid7.NewUuid7();
		Follower = follower;
		Follows = follows;
		State = state;
		Date = DateTime.UtcNow;
	}

	public Uuid7 GetId() => _id;
	public string GetId25() => _id.ToId25String();

	public bool Equals(FollowerRelation? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Follower.Equals(other.Follower) && Follows.Equals(other.Follows);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((FollowerRelation)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Follower, Follows);
	}

	public static bool operator ==(FollowerRelation? left, FollowerRelation? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(FollowerRelation? left, FollowerRelation? right)
	{
		return !Equals(left, right);
	}
}