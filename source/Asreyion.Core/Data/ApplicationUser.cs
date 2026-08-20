using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Data;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}