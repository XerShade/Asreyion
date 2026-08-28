using Asreyion.Core.Modules.Interfaces;
using Asreyion.Core.Mvc.Conventions;
using Asreyion.Core.Mvc.ViewLocationExpanders;

namespace Asreyion.Core.Mvc.Modules;

public class RoutingModule : ICoreModule
{
    public string Name => "Routing";
    public string Description => "Configures the url routing of the application.";
    public Type[] Dependencies => [];

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
            .AddControllersWithViews(options =>
                options.Conventions.Add(new FeatureAreaConvention()))
            .AddRazorOptions(options =>
                options.ViewLocationExpanders.Add(new FeatureViewLocationExpander()));
}