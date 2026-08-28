namespace Asreyion.Core.Modules.Interfaces;

public interface ICoreModule
{
    string Name { get; }
    string Description { get; }
    Type[] Dependencies { get; }

    void OnConfigureApplication(WebApplication app, IWebHostEnvironment env);
    void OnConfigureServices(IServiceCollection services, IConfiguration configuration);
}