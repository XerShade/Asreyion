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
    /// <param name="services">The application service collection whose configuration hooks should run.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance to use for hook configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown when an <see cref="IConfiguration"/> instance cannot be located within the service collection descriptors.</exception>
    /// <returns>The same <see cref="IServiceCollection"/> instance for fluent chaining.</returns>
    public static IServiceCollection ConfigureServicesWithHooks(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // These hooks must run before the final service provider is built, so resolve them in a small bootstrap provider.
        IServiceCollection bootstrapServices = new ServiceCollection();

        foreach (ServiceDescriptor descriptor in services.Where(s =>
                     (s.ImplementationType != null && typeof(IConfigureServicesHook).IsAssignableFrom(s.ImplementationType)) ||
                     (s.ServiceType != null && typeof(IConfigureServicesHook).IsAssignableFrom(s.ServiceType))))
        {
            bootstrapServices.Add(descriptor);
        }

        using ServiceProvider bootstrapProvider = bootstrapServices.BuildServiceProvider();

        IEnumerable<IConfigureServicesHook> hooks = bootstrapProvider.GetServices<IConfigureServicesHook>();

        foreach (IConfigureServicesHook hook in hooks)
        {
            hook.OnConfigureServices(services, configuration);
        }

        return services;
    }
}
