namespace Letterbook.Core.Adapters;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AutoAdapterAttribute : Attribute
{
    public InjectableScope Scope { get; }
    public Type Type { get; }

    public AutoAdapterAttribute(Type type, InjectableScope scope = InjectableScope.Scoped)
    {
        Scope = scope;
        Type = type;
    }

    public InjectableScope GetScope() => Scope;
}