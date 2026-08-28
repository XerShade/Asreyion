using Microsoft.AspNetCore.Authentication;

namespace Asreyion.Core.Features.Authentication.Providers.Interfaces;

public interface IAuthenticationProvider
{
    string Name { get; }

    void OnConfigureAuthentication(AuthenticationBuilder builder, IConfiguration configuration);
}