using Asreyion.Core.Features.Authentication.Providers.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace Asreyion.Core.Features.Authentication.Providers;

public sealed class GoogleAuthenticationProvider : IAuthenticationProvider
{
    public string Name => "Google";

    public void OnConfigureAuthentication(AuthenticationBuilder builder, IConfiguration configuration)
    {
        string clientId =
            configuration["Authentication:Google:ClientId"]
            ?? throw new InvalidOperationException(
                "Google authentication ClientId is not configured.");

        string clientSecret =
            configuration["Authentication:Google:ClientSecret"]
            ?? throw new InvalidOperationException(
                "Google authentication ClientSecret is not configured.");

        _ = builder.AddGoogle(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
        });
    }
}