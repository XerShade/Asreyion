using Asreyion.Core.Hooks.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Hooks.Extensions;

/// <summary>
/// Defines the extension methods for the <see cref="IServiceCollection"/> to discover and register hooks.
/// </summary>
public static class HookDiscoveryExtensions
{
    /// <summary>
    /// Registers all hooks in the specified module assembly.
    /// </summary>
    /// <param name="services">The service collection to register the hooks to.</param>
    /// <param name="moduleAssembly">The assembly to scan for hooks.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection DiscoverHooks(this IServiceCollection services, Assembly moduleAssembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(moduleAssembly);

        List<Type> hookTypes = [.. moduleAssembly.GetTypes()
            .Where(t => typeof(IHookSubscriber).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)];

        IReadOnlyList<Type> sortedHookTypes = HookDependencySorter.Sort(hookTypes);

        foreach (Type type in sortedHookTypes)
        {
            IEnumerable<Type> interfaces = type.GetInterfaces()
                .Where(i => i != typeof(IHookSubscriber) && typeof(IHookSubscriber).IsAssignableFrom(i));

            Type[] hookInterfaces = [.. interfaces];

            foreach (Type @interface in hookInterfaces)
            {
                _ = services.AddScoped(@interface, type);
            }

            if (hookInterfaces.Length > 0)
            {
                _ = services.AddSingleton(new RegisteredHook(type, hookInterfaces));
            }
        }

        return services;
    }
}

/// <summary>
/// Describes a subscriber implementation registered by hook discovery.
/// </summary>
/// <param name="ImplementationType">The concrete hook implementation.</param>
/// <param name="ServiceTypes">The hook contracts registered for that implementation.</param>
public sealed record RegisteredHook(Type ImplementationType, IReadOnlyList<Type> ServiceTypes);
