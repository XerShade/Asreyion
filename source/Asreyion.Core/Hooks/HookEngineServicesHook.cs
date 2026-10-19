using Asreyion.Core.Hooks.Engine;
using Asreyion.Core.Hooks.Engine.Interfaces;
using Asreyion.Core.Hooks.Extensions;
using Asreyion.Core.Hooks.Hooks;
using System.Reflection;

namespace Asreyion.Core.Hooks;

/// <summary>
/// Registers the hook engine and discovers subscribers in assemblies loaded by the host.
/// </summary>
public class HookEngineServicesHook : IConfigureServicesHook
{
    /// <inheritdoc />
    public string Name { get; } = "Hook Engine";
    /// <inheritdoc />
    public string Description { get; } = "Provides the ability to run hooks to cleanly execute code within the application.";

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddSingleton<IHookEngine, HookEngine>();

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly == Assembly.GetExecutingAssembly())
            {
                // The server discovers its own hooks before this hook runs.
                continue;
            }

            _ = services.DiscoverHooks(assembly);
        }
    }
}
