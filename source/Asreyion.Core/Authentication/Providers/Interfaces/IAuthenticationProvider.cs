using Microsoft.AspNetCore.Authentication;

namespace Asreyion.Core.Authentication.Providers.Interfaces;

/// <summary>
/// Adds an external authentication provider to the application's authentication builder.
/// </summary>
public interface IAuthenticationProvider
{
    /// <summary>
    /// Gets the provider's display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registers the provider using settings from application configuration.
    /// </summary>
    /// <param name="builder">The authentication builder being configured.</param>
    /// <param name="configuration">The application configuration containing provider settings.</param>
    void OnConfigureAuthentication(AuthenticationBuilder builder, IConfiguration configuration);
}
