using Asreyion.Core.Features.Hooks.Interfaces;
using Asreyion.Core.Hooks;
using System.Reflection;

namespace Asreyion.Core.Features.Hooks.Extensions;

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
        // Find all classes that implement the IHookSubscriber interface.
        List<Type> hookTypes = [.. moduleAssembly.GetTypes()
            .Where(t => typeof(IHookSubscriber).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)];

        // Sort the hook types based on their dependencies using the updated Type-based sorter.
        IReadOnlyList<Type> sortedHookTypes = HookDependencySorter.Sort(hookTypes);

        // Iterate over the sorted hook types found.
        foreach (Type type in sortedHookTypes)
        {
            // Find all hook interfaces the class implements (e.g., IOnBeforeUserRegisterHook)
            IEnumerable<Type> interfaces = type.GetInterfaces()
                .Where(i => i != typeof(IHookSubscriber) && typeof(IHookSubscriber).IsAssignableFrom(i));

            // Iterate over the hook interfaces found.
            foreach (Type @interface in interfaces)
            {
                // Register it to the DI container scoped or transient.
                // The order of registration will match the dependency resolution sequence.
                _ = services.AddScoped(@interface, type);
            }
        }

        // Return the updated service collection.
        return services;
    }
}
