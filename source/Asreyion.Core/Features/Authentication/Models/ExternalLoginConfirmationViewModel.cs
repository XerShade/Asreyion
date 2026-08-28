using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Features.Authentication.Models;

public sealed class ExternalLoginConfirmationViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Display name")]
    public string DisplayName { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }

    public string Provider { get; set; } = string.Empty;
}