using Asreyion.Core.Data;
using Asreyion.Core.Database.DbContexts;
using Asreyion.Core.Features.Authentication.Providers.Interfaces;
using Asreyion.Core.Modules;
using Asreyion.Core.Modules.Interfaces;
using Asreyion.Core.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Features.Authentication.Modules;

public class AuthenticationModule : ICoreModule
{
    public string Name => "Authentication";
    public string Description => "Configures the authentication providers and services for the application.";
    public Type[] Dependencies => [typeof(ProxyConfigurationModule), typeof(DatabaseConfigurationModule), typeof(RoutingModule)];

    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        _ = app.UseAuthentication();
        _ = app.UseAuthorization();
    }

    public void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        })
        .AddEntityFrameworkStores<AuthenticationDbContext>()
        .AddDefaultTokenProviders();

        AuthenticationBuilder authenticationBuilder = services.AddAuthentication();
        IReadOnlyCollection<Type> providerTypes = new AssemblyTypeDiscovery()
            .Discover<IAuthenticationProvider>(AppDomain.CurrentDomain.GetAssemblies());

        foreach(Type providerType in providerTypes)
        {
            _ = services.AddTransient(typeof(IAuthenticationProvider), providerType);

            if (Activator.CreateInstance(providerType) is not IAuthenticationProvider provider)
            {
                throw new InvalidOperationException(
                    $"Unable to create authentication provider " +
                    $"'{providerType.FullName}'.");
            }
            provider.OnConfigureAuthentication(authenticationBuilder, configuration);
        }
    }
}