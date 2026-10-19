using Asreyion.Core.Features.Authentication.Providers.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Asreyion.Core.Features.Authentication.Providers;

/// <summary>
/// Registers Discord as an external sign-in provider.
/// </summary>
public class DiscordAuthenticationProvider : IAuthenticationProvider
{
    /// <inheritdoc />
    public string Name => "Discord";

    /// <inheritdoc />
    public void OnConfigureAuthentication(AuthenticationBuilder builder, IConfiguration configuration)
    {
        string clientId =
            configuration["Authentication:Discord:ClientId"]
            ?? throw new InvalidOperationException(
                "Discord authentication ClientId is not configured.");

        string clientSecret =
            configuration["Authentication:Discord:ClientSecret"]
            ?? throw new InvalidOperationException(
                "Discord authentication ClientSecret is not configured.");

        _ = builder.AddDiscord(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;

            options.Scope.Add("identify");
            options.Scope.Add("email");
        });
    }
}
