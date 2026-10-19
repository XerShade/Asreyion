namespace Asreyion.Core.Authentication.Models;

/// <summary>
/// Form values submitted when a user signs in with a local account.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Gets or sets the username entered on the sign-in form.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password entered on the sign-in form.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the sign-in should persist across browser sessions.
    /// </summary>
    public bool RememberMe { get; set; }

    /// <summary>
    /// Gets or sets the local URL to visit after a successful sign-in.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets whether password-based sign-in is available.
    /// </summary>
    public bool IsLocalLoginEnabled { get; } = false;
}
