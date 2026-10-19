using Asreyion.Core.Features.Hooks.Hooks;
using System.Reflection;

namespace Asreyion.Core.Mvc.Hooks;

/// <summary>
/// Registers MVC routing services and maps the application's endpoints.
/// </summary>
public class RoutingConfigurationHooks : IConfigureServicesHook, IConfigureApplicationHook
{
    /// <inheritdoc />
    public string Name => "Routing";
    /// <inheritdoc />
    public string Description => "Configures the url routing of the application.";

    /// <inheritdoc />
    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        _ = app.UseRouting();

        _ = app.MapStaticAssets();

        _ = app.MapControllers();

        _ = app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        _ = app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
    }

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        => services
            .AddControllersWithViews()
            .AddRazorOptions(options => options.ViewLocationExpanders.Add(new ViewLocationExpander()))
            .AddApplicationPart(Assembly.GetExecutingAssembly());
}
