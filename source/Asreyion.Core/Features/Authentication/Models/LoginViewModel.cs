namespace Asreyion.Core.Features.Authentication.Models;

public class LoginViewModel
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public bool IsLocalLoginEnabled { get; } = false;
}