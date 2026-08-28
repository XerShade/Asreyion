using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Core.Modules.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Features.Database.Modules;

public class DatabaseConfigurationModule : ICoreModule
{
    /// <inheritdoc />
    public string Name { get; } = "Database Configuration";
    /// <inheritdoc />
    public string Description { get; } = "Configures the databases used by the application.";
    /// <inheritdoc />
    public Type[] Dependencies { get; } = [];

    /// <inheritdoc />
    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    { }

    /// <inheritdoc />
    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration) 
        => services.AddDbContext<AuthenticationDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("Authentication"),
                ServerVersion.AutoDetect(configuration.GetConnectionString("Authentication"))
            ));
}