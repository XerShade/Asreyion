using Asreyion.Core.Configuration;
using Asreyion.Core.Modules;
using Asreyion.Core.Modules.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(HostConfiguration.Create(args));

AssemblyModuleDiscovery moduleDiscovery = new();
ModuleDependencySorter dependencySorter = new();

IReadOnlyCollection<ICoreModule> modules = dependencySorter.Sort(
    moduleDiscovery.Discover(AppDomain.CurrentDomain.GetAssemblies()));

foreach (ICoreModule module in modules)
{
    module.OnConfigureServices(builder.Services, builder.Configuration);
}

WebApplication app = builder.Build();

foreach (ICoreModule module in modules)
{
    module.OnConfigureApplication(app, app.Environment);
}

app.Run();
