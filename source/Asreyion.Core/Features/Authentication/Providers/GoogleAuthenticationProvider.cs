/* Disabled Google Authentication Support
 * Reason: It shall remain disabled as of until such time that Lake Ontario is not shown as "Lake America" in ANY country.
 * Note: Yes I am leaving the code here in the project. It is just disabled and not used when the project is built.
 *       I'll re-enable it when Lake Ontario is not shown as "Lake America" in any country. If you use my code and want to
 *       bend the knee feel free, just remove the comments and enable the code and it will work again as if never disabled.
 *       
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
*/