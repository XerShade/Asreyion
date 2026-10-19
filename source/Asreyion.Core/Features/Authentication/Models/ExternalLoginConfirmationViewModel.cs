using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Features.Authentication.Models;

/// <summary>
/// Profile details used to create an account after an external sign-in.
/// </summary>
public sealed class ExternalLoginConfirmationViewModel
{
    /// <summary>
    /// Gets or sets the username for the new account.
    /// </summary>
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address supplied by the provider.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name shown on the user's profile.
    /// </summary>
    [Required]
    [Display(Name = "Display name")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the local URL to visit after account creation.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the provider that authenticated the user.
    /// </summary>
    public string Provider { get; set; } = string.Empty;
}
