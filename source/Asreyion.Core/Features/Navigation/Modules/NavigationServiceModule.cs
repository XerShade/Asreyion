using Asreyion.Core.Features.Database.Modules;
using Asreyion.Core.Features.Navigation.Services;
using Asreyion.Core.Features.Navigation.Services.Interfaces;
using Asreyion.Core.Modules.Interfaces;

namespace Asreyion.Core.Features.Navigation.Modules;

public class NavigationServiceModule : ICoreModule
{
    public string Name => "Navigation Menu Service";

    public string Description => "Adds a service capable of organizing and displaying navigation menus to the application.";

    public Type[] Dependencies => [typeof(DatabaseConfigurationModule)];

    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    { /* Does not actually have any configuration. */ }

    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<INavigationService, NavigationService>();
}