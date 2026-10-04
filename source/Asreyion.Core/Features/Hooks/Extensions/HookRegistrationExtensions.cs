using Asreyion.Core.Features.Hooks.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Features.Hooks.Extensions;

/// <summary>
/// Defines the extension methods for the <see cref="IServiceCollection"/> to discover and register hooks.
/// </summary>
public static class HookRegistrationExtensions
{
    /// <summary>
    /// Registers all hooks in the specified module assembly.
    /// </summary>
    /// <param name="services">The service collection to register the hooks to.</param>
    /// <param name="moduleAssembly">The assembly to scan for hooks.</param>
    public static void AddHooks(this IServiceCollection services, Assembly moduleAssembly)
    {
        // Find all classes that implement the IHookSubscriber interface.
        IEnumerable<Type> hookTypes = moduleAssembly.GetTypes()
            .Where(t => typeof(IHookSubscriber).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        // Iterate over the hook types found.
        foreach (Type type in hookTypes)
        {
            // Find all hook interfaces the class implements (e.g., IOnBeforeUserRegisterHook)
            IEnumerable<Type> interfaces = type.GetInterfaces().Where(i => i != typeof(IHookSubscriber) && typeof(IHookSubscriber).IsAssignableFrom(i));

            // Iterate over the hook interfaces found.
            foreach (Type @interface in interfaces)
            {
                // Register it to the DI container scoped or transient
                _ = services.AddScoped(@interface, type);
            }
        }
    }
}
