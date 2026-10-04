using Asreyion.Core.Features.Hooks.Extensions;
using Asreyion.Core.Features.Hooks.Interfaces;
using Asreyion.Core.Modules.Interfaces;
using System.Reflection;

namespace Asreyion.Core.Features.Hooks.Modules;

public class HookEngineModule : ICoreModule
{
    /// <inheritdoc />
    public string Name { get; } = "Hook Engine";
    /// <inheritdoc />
    public string Description { get; } = "Provides the ability to run hooks to cleanly execute code within the application.";
    /// <inheritdoc />
    public Type[] Dependencies { get; } = [];

    /// <inheritdoc />
    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    { }

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register the hook engine.
        _ = services.AddSingleton<IHookEngine, HookEngine>();

        // Iterate over all loaded assemblies.
        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Discover and add any hooks to the service collection.
            services.AddHooks(assembly);
        }
    }
}