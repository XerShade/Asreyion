using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Hooks;
using System.Reflection;

namespace Asreyion.Core.Mvc.Hooks;

/// <summary>
/// Registers MVC routing services and maps the application's endpoints.
/// </summary>
[HookName("Routing Configuration")]
[HookDescription("Registers MVC routing services and maps the application's endpoints.")]
public class RoutingConfigurationHook : IConfigureServicesHook, IConfigureApplicationHook
{
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
