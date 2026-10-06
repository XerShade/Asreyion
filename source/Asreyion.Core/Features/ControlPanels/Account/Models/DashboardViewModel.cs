using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Features.ControlPanels.Account.Models;

public class DashboardViewModel
{
    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? DisplayName { get; set; }

    public IList<UserLoginInfo>? ExternalLogins { get; set; }
}