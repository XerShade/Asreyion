using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.ControlPanels.Account.Models;

/// <summary>
/// Account details and connected sign-in providers shown on the account dashboard.
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Gets or sets the account username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the account email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the name shown on the user's profile.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the external sign-in providers connected to the account.
    /// </summary>
    public IList<UserLoginInfo>? ExternalLogins { get; set; }
}
