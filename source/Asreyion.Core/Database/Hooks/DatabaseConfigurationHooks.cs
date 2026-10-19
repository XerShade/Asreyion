using Asreyion.Core.Database.DbContexts;
using Asreyion.Core.Hooks.Hooks;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Database.Hooks;

/// <summary>
/// Registers the application's authentication and content database contexts.
/// </summary>
public class DatabaseConfigurationModule : IConfigureServicesHook
{
    /// <inheritdoc />
    public string Name { get; } = "Database Configuration";
    /// <inheritdoc />
    public string Description { get; } = "Configures the databases used by the application.";

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
