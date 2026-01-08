using System.Collections.Immutable;
using System.Security.Claims;
using Letterbook.Core.Exceptions;

namespace Letterbook.Core.Authorization;

public class Decision
{
	protected readonly HashSet<Claim> Supporting = new();
	protected readonly HashSet<Claim> Disqualifying = new();
	protected readonly HashSet<Claim> Other = new();
	protected bool IsAllowed;
	protected string? OverrideReason;

	protected Decision()
	{
		IsAllowed = false;
	}

	public static Decision Allow(string reason, IEnumerable<Claim>? claims) =>
		new DecisionBuilder(claims).Decide(true, reason);

	public static Decision Deny(string reason, IEnumerable<Claim>? claims) =>
		new DecisionBuilder(claims).Decide(false, reason);

	public void AssertAllowed()
	{
		if (!IsAllowed) throw CoreException.Unauthorized(this);
	}

	public bool Allowed => IsAllowed;

	public string? Reason => OverrideReason;

	public IReadOnlyCollection<Claim> SupportingClaims => Supporting.ToImmutableHashSet();

	public IReadOnlyCollection<Claim> DisqualifyingClaims => Disqualifying.ToImmutableHashSet();

	public IReadOnlyCollection<Claim> OtherClaims => Other.ToImmutableHashSet();

	public static implicit operator bool(Decision d) => d.IsAllowed;
}