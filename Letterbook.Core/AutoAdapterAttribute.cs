using Letterbook.Core.Adapters;

namespace Letterbook.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AutoAdapterAttribute<T> : Attribute where T: IAdapter
{
    public InjectableScope Scope { get; }
    public Type Type => typeof(T);
    
    public AutoAdapterAttribute(InjectableScope scope = InjectableScope.Scoped)
    {
        Scope = scope;
    }
}