namespace Asreyion.Core.Areas.Account.Models;

/// <summary>
/// Profile details displayed on an account profile page.
/// </summary>
public class ProfileViewModel
{
    /// <summary>
    /// Gets or sets the account username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the name shown on the profile.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets when the account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
