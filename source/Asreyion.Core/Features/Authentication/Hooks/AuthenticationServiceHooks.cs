using Asreyion.Core.Features.Authentication.Data;
using Asreyion.Core.Features.Authentication.Providers.Interfaces;
using Asreyion.Core.Features.Database.DbContexts;
using Asreyion.Core.Features.Database.Hooks;
using Asreyion.Core.Features.Hooks.Attributes;
using Asreyion.Core.Features.Hooks.Hooks;
using Asreyion.Core.Mvc.Hooks;
using Asreyion.Core.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Features.Authentication.Hooks;

[HookDependencies(typeof(ProxyConfigurationHooks), typeof(DatabaseConfigurationModule), typeof(RoutingConfigurationHooks))]
public class AuthenticationServiceHooks : IConfigureServicesHook, IConfigureApplicationHook
{
    public string Name => "Authentication Services";
    public string Description => "Configures the authentication providers and services for the application.";

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