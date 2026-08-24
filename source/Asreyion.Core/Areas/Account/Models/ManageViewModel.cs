using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Areas.Account.Models;

public class ManageViewModel
{
    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? DisplayName { get; set; }

    public IList<UserLoginInfo>? ExternalLogins { get; set; }
}