using Asreyion.Core.Features.Hooks.Hooks;

namespace Asreyion.Core.Features.Hooks.Extensions;

/// <summary>
/// Provides extension methods for executing early application configuration hooks.
/// </summary>
public static class HookServiceCollectionExtensions
{
    /// <summary>
    /// Discovers, builds an isolated bootstrap container for, and executes all registered service configuration hooks.
    /// </summary>
    /// <param name="services">The application service collection instance to pull hooks from and run configuration routines aainst.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to use for hook configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown when an <see cref="IConfiguration"/> instance cannot be located within the service collection descriptors.</exception>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection ConfigureServicesWithHooks(this IServiceCollection services, IConfiguration configuration)
    {
        // Create a fresh service collection for the mini bootstrap container
        IServiceCollection bootstrapServices = new ServiceCollection();

        // Extract and copy matching service descriptors into the bootstrap collection
        foreach (ServiceDescriptor descriptor in services.Where(s =>
                     (s.ImplementationType != null && typeof(IConfigureServicesHook).IsAssignableFrom(s.ImplementationType)) ||
                     (s.ServiceType != null && typeof(IConfigureServicesHook).IsAssignableFrom(s.ServiceType))))
        {
            bootstrapServices.Add(descriptor);
        }

        // Build the isolated mini-container
        using ServiceProvider bootstrapProvider = bootstrapServices.BuildServiceProvider();

        // Resolve all registered IConfigureServicesHook instances
        IEnumerable<IConfigureServicesHook> hooks = bootstrapProvider.GetServices<IConfigureServicesHook>();

        // Iterate and execute each configuration block against the main services collection.
        foreach (IConfigureServicesHook hook in hooks)
        {
            hook.OnConfigureServices(services, configuration);
        }

        // Return the modified service collection
        return services;
    }
}
