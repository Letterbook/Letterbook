using System.Reflection;
using Letterbook.Core.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Core.Extensions;

public static class AutoInjectors
{
    public static IServiceCollection AddAdapters(this IServiceCollection svc)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var adapterTypes = GetTypesWithAutoAdapter(assembly);
            foreach (var adapter in adapterTypes)
            {
                var attrs = adapter.GetCustomAttributes();
                foreach (var attr in attrs)
                {
                    if (attr is AutoAdapterAttribute a)
                    {
                        switch (a.Scope)
                        {
                            case InjectableScope.Scoped:
                                svc.AddScoped(a.Type, adapter);
                                break;
                            case InjectableScope.Singleton:
                                svc.AddSingleton(a.Type, adapter);
                                break;
                            case InjectableScope.Transient:
                                svc.AddTransient(a.Type, adapter);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(a.Scope), a.Scope,
                                    $"Unknown scope {a.Scope}");
                        }
                    }
                }
            }
        }

        return svc;
    }
    
    public static IServiceCollection AddContexts(this IServiceCollection svc)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var contextTypes = GetTypesWithAutoContext(assembly);
            foreach (var context in contextTypes)
            {
                var attrs = context.GetCustomAttributes();
                foreach (var attr in attrs)
                {
                    if (attr is AutoContextAttribute a)
                    {
                        switch (a.Scope)
                        {
                            case InjectableScope.Scoped:
                                svc.AddScoped(context);
                                break;
                            case InjectableScope.Singleton:
                                svc.AddSingleton(context);
                                break;
                            case InjectableScope.Transient:
                                svc.AddTransient(context);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(a.Scope), a.Scope,
                                    $"Unknown scope {a.Scope}");
                        }
                    }
                }
            }
        }

        return svc;
    }

    internal static IEnumerable<Type> GetTypesWithAutoAdapter(Assembly assembly)
    {
        return assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(AutoAdapterAttribute), true).Length > 0);
    }

    internal static IEnumerable<Type> GetTypesWithAutoContext(Assembly assembly)
    {
        return assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(AutoContextAttribute), true).Length > 0);
    }
}