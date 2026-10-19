using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Authentication.Models;

/// <summary>
/// Credentials used to connect an external provider to an existing account.
/// </summary>
public sealed class ExternalLoginLinkingViewModel
{
    /// <summary>
    /// Gets or sets the username of the existing account.
    /// </summary>
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password used to verify account ownership.
    /// </summary>
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the local URL to visit after linking.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the external provider being linked.
    /// </summary>
    public string Provider { get; set; } = string.Empty;
}
