using Microsoft.AspNetCore.Identity;

namespace Asreyion.Core.Features.Authentication.Data;

/// <summary>
/// An application identity with profile and account lifecycle information.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets the name displayed on the user's profile.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the user can sign in.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when the account was deactivated, if applicable.
    /// </summary>
    public DateTime? DeactivatedAt { get; set; }
}
