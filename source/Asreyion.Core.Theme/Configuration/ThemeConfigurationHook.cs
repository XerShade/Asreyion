using Asreyion.Core.Hooks.Hooks;

namespace Asreyion.Core.Theme.Configuration;

/// <summary>
/// Binds site theme settings from the host configuration.
/// </summary>
public sealed class ThemeConfigurationHook : IConfigureServicesHook
{
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ThemeOptions>(configuration.GetSection("Theme"));
    }
}
