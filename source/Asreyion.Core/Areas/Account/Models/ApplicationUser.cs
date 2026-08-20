using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Areas.Account.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}