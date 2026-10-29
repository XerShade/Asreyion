using Asreyion.Core.Authentication.Data;
using Asreyion.Core.Authentication.Providers.Interfaces;
using Asreyion.Core.Database.DbContexts;
using Asreyion.Core.Database.Hooks;
using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Hooks.Hooks;
using Asreyion.Core.Mvc.Hooks;
using Asreyion.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Authentication.Hooks;

/// <summary>
/// Registers identity, external providers, and authentication middleware.
/// </summary>
[HookName("Authentication Service")]
[HookDescription("Registers identity, external providers, and authentication middleware.")]
[HookDependencies(typeof(ProxyConfigurationHook), typeof(DatabaseConfigurationHook), typeof(RoutingConfigurationHook))]
public class AuthenticationServiceHook : IConfigureServicesHook, IConfigureApplicationHook
{
    /// <inheritdoc />
    public void OnConfigureApplication(WebApplication app, IWebHostEnvironment env)
    {
        _ = app.UseAuthentication();
        _ = app.UseAuthorization();
    }

    /// <inheritdoc />
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
        IReadOnlyCollection<Type> providerTypes = new AssemblyTypeDiscoveryService()
            .Discover<IAuthenticationProvider>(AppDomain.CurrentDomain.GetAssemblies());

        foreach (Type providerType in providerTypes)
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
