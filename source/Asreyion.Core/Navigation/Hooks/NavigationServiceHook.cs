using Asreyion.Core.Database.Hooks;
using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Hooks;
using Asreyion.Core.Navigation.Services;
using Asreyion.Core.Navigation.Services.Interfaces;

namespace Asreyion.Core.Navigation.Hooks;

/// <summary>
/// Adds a service capable of organizing and displaying navigation menus to the application.
/// </summary>
[HookName("Navigation Menu Service")]
[HookDescription("Adds a service capable of organizing and displaying navigation menus to the application.")]
[HookDependencies(typeof(DatabaseConfigurationHook))]
public class NavigationServiceHook : IConfigureServicesHook
{
    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<INavigationService, NavigationService>();
}
