using Asreyion.Core.Features.Hooks.Hooks;
using System.Reflection;

namespace Asreyion.Core.Mvc.Hooks;

public class RoutingConfigurationHooks : IConfigureServicesHook, IConfigureApplicationHook
{
    public string Name => "Routing";
    public string Description => "Configures the url routing of the application.";

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

    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        => services
            .AddControllersWithViews()
            .AddRazorOptions(options => options.ViewLocationExpanders.Add(new ViewLocationExpander()))
            .AddApplicationPart(Assembly.GetExecutingAssembly());
}