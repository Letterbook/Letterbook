using System.Security.Claims;
using Medo;

namespace Letterbook.Core.Models;

public class ProfileClaims : IEquatable<ProfileClaims>
{
	private Uuid7 _id;

	public Guid Id
	{
		get => _id.ToGuid();
		set => _id = Uuid7.FromGuid(value);
	}

	public Profile Profile { get; init; }
	public Account Account { get; init; }
	public List<ProfileClaim> Claims { get; set; } = [];

	private ProfileClaims()
	{
		Id = Uuid7.NewUuid7();
		Profile = default!;
		Account = default!;
	}

	public ProfileClaims(Account account, Profile profile, IEnumerable<ProfileClaim> claims)
	{
		Id = Uuid7.NewUuid7();
		Account = account;
		Profile = profile;
		Claims.AddRange(claims);
	}

	public Uuid7 GetId() => _id;
	public string GetId25() => _id.ToId25String();

	public bool Equals(ProfileClaims? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Account.Equals(other.Account) && Profile.Equals(other.Profile);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((ProfileClaims)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Account.GetHashCode(), Profile.GetHashCode());
	}

	public static bool operator ==(ProfileClaims? left, ProfileClaims? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(ProfileClaims? left, ProfileClaims? right)
	{
		return !Equals(left, right);
	}
}