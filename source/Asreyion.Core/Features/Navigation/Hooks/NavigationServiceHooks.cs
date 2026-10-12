using Asreyion.Core.Features.Database.Hooks;
using Asreyion.Core.Features.Hooks.Attributes;
using Asreyion.Core.Features.Hooks.Hooks;
using Asreyion.Core.Features.Navigation.Services;
using Asreyion.Core.Features.Navigation.Services.Interfaces;

namespace Asreyion.Core.Features.Navigation.Hooks;

[HookDependencies(typeof(DatabaseConfigurationModule))]
public class NavigationServiceHooks : IConfigureServicesHook
{
    public string Name => "Navigation Menu Service";

    public string Description => "Adds a service capable of organizing and displaying navigation menus to the application.";

    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<INavigationService, NavigationService>();
}