using System.Security.Claims;

namespace Letterbook.Core.Authorization;

public class DecisionBuilder : Decision
{
    private readonly HashSet<Claim> _allClaims = new();

    public DecisionBuilder(IEnumerable<Claim>? allClaims = null)
    {
        if (allClaims is not null)
            _allClaims.UnionWith(allClaims);
    }

    public void SupportedBy(Claim claim)
    {
        Supporting.Add(claim);
        _allClaims.Add(claim);
    }

    public void DisqualifiedBy(Claim claim)
    {
        Disqualifying.Add(claim);
        _allClaims.Add(claim);
    }

    public Decision Decide(bool decision, string reason)
    {
        Decide();
        OverrideReason = reason;
        IsAllowed = decision;

        return this;
    }

    public Decision Decide()
    {
        Other.UnionWith(_allClaims);
        Other.ExceptWith(Supporting);
        Other.ExceptWith(Disqualifying);
        Supporting.ExceptWith(Disqualifying);
        Disqualifying.ExceptWith(Supporting);

        IsAllowed = Disqualifying.Count == 0;

        return this;
    }
    
}