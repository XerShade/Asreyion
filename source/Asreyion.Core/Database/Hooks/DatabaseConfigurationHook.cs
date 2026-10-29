using Asreyion.Core.Database.DbContexts;
using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Hooks;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Database.Hooks;

/// <summary>
/// Registers and configures the databases used by the application.
/// </summary>
[HookName("Database Configuration")]
[HookDescription("Configures the databases used by the application.")]
public class DatabaseConfigurationHook : IConfigureServicesHook
{
    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddDbContext<AuthenticationDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("Authentication"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("Authentication"))
                ));

        _ = services.AddDbContext<DataDbContext>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("Data"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("Data"))
                ));
    }
}
