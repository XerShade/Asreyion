using Asreyion.Core.Database.Hooks;
using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Hooks;
using Asreyion.Core.Navigation.Services;
using Asreyion.Core.Navigation.Services.Interfaces;

namespace Asreyion.Core.Navigation.Hooks;

/// <summary>
/// Registers the service that reads and maintains navigation menus.
/// </summary>
[HookDependencies(typeof(DatabaseConfigurationModule))]
public class NavigationServiceHooks : IConfigureServicesHook
{
    /// <inheritdoc />
    public string Name => "Navigation Menu Service";

    /// <inheritdoc />
    public string Description => "Adds a service capable of organizing and displaying navigation menus to the application.";

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<INavigationService, NavigationService>();
}
