using Asreyion.Core.Features.Hooks.Engine;
using Asreyion.Core.Features.Hooks.Engine.Interfaces;
using Asreyion.Core.Features.Hooks.Extensions;
using System.Reflection;

namespace Asreyion.Core.Features.Hooks.Hooks;

public class HookEngineServicesHook : IConfigureServicesHook
{
    /// <inheritdoc />
    public string Name { get; } = "Hook Engine";
    /// <inheritdoc />
    public string Description { get; } = "Provides the ability to run hooks to cleanly execute code within the application.";

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register the hook engine.
        _ = services.AddSingleton<IHookEngine, HookEngine>();

        // Iterate over all loaded assemblies.
        foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Check to see if the assembly is the executing assembly.
            if (assembly == Assembly.GetExecutingAssembly())
            {
                // Skip the executing assembly, it's hooks were already discovered.
                continue;
            }

            // Discover and add any hooks to the service collection.
            _ = services.DiscoverHooks(assembly);
        }
    }
}