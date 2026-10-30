using Asreyion.Core.Members.Models;
using Asreyion.Core.Authentication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Members.Controllers;

/// <summary>
/// Displays profiles for active accounts.
/// </summary>
[Area("Members")]
public class ProfileController(
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;

    /// <summary>
    /// Displays the signed-in user's profile.
    /// </summary>
    /// <returns>The profile view, or a not-found response for inactive or unknown users.</returns>
    [Authorize]
    [HttpGet("members/profile")]
    public async Task<IActionResult> Index()
    {
        ApplicationUser? user = await this.UserManager.GetUserAsync(this.User);

        return await this.GetUserAsync(user);
    }

    /// <summary>
    /// Displays the signed-in user's profile or a profile selected by username.
    /// </summary>
    /// <param name="username">The account name to display, or <see langword="null"/> for the current user.</param>
    /// <returns>The profile view, or a not-found response for inactive or unknown users.</returns>
    [HttpGet("members/profile/{username}")]
    public async Task<IActionResult> Index(string? username)
    {
        ApplicationUser? user = string.IsNullOrWhiteSpace(username)
            ? await this.UserManager.GetUserAsync(this.User)
            : await this.UserManager.FindByNameAsync(username);

        return await this.GetUserAsync(user);
    }

    /// <summary>
    /// Displays the profile of the specified user.
    /// </summary>
    /// <param name="user">The user to display.</param>
    /// <returns>The profile view, or a not-found response for inactive or unknown users.</returns>
    private async Task<IActionResult> GetUserAsync(ApplicationUser? user)
    {
        if (user is null || !user.IsActive)
        {
            return this.NotFound();
        }

        ProfileViewModel model = new()
        {
            UserName = user.UserName ?? string.Empty,
            DisplayName = user.DisplayName,
            CreatedAt = user.CreatedAt
        };

        return this.View(model);
    }
}
