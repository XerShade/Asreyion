using Asreyion.Core.Modules.Interfaces;
using Asreyion.Modules.SimpleContent.Services;
using Asreyion.Modules.SimpleContent.Services.Interfaces;

namespace Asreyion.Modules.SimpleContent.Modules;

public class SimpleContentModule : ICoreModule
{
    public string Name => "SimpleContent";
    public string Description => "SimpleContentModule";
    public Type[] Dependencies => [];

    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    { }

    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        => services.AddTransient<IContentProvider, MarkdownContentProvider>();
}