namespace Letterbook.Core.Adapters;

[AttributeUsage(AttributeTargets.Class)]
public class AutoContextAttribute : Attribute
{
    public InjectableScope Scope { get; }

    public AutoContextAttribute(InjectableScope scope = InjectableScope.Scoped)
    {
        Scope = scope;
    }
}