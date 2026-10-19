using Asreyion.Core.Hooks.Interfaces;

namespace Asreyion.Core.Hooks.Hooks;

/// <summary>
/// Configures services before the application's dependency injection container is built.
/// </summary>
public interface IConfigureServicesHook : IHookSubscriber, IHookDescriptors
{
    /// <summary>
    /// Adds or configures services using the host configuration available at startup.
    /// </summary>
    /// <param name="services">The application service collection to configure.</param>
    /// <param name="configuration">The host configuration.</param>
    void OnConfigureServices(IServiceCollection services, IConfiguration configuration);
}
