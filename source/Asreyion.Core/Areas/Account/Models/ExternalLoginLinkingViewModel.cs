using System.ComponentModel.DataAnnotations;

namespace Asreyion.Core.Areas.Account.Models;

public sealed class ExternalLoginLinkingViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }

    public string Provider { get; set; } = string.Empty;
}
