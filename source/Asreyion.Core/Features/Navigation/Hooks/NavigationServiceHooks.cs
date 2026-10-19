using Asreyion.Core.Features.Database.Hooks;
using Asreyion.Core.Features.Hooks.Attributes;
using Asreyion.Core.Features.Hooks.Hooks;
using Asreyion.Core.Features.Navigation.Services;
using Asreyion.Core.Features.Navigation.Services.Interfaces;

namespace Asreyion.Core.Features.Navigation.Hooks;

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
