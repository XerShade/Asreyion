using Asreyion.Core.Areas.Account.Models;
using Asreyion.Core.Features.Authentication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Account.Controllers;

/// <summary>
/// Displays profiles for active accounts.
/// </summary>
[Area("Account")]
[Authorize]
[Route("Account/Profile")]
public class ProfileController(
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;

    /// <summary>
    /// Displays the signed-in user's profile or a profile selected by username.
    /// </summary>
    /// <param name="username">The account name to display, or <see langword="null"/> for the current user.</param>
    /// <returns>The profile view, or a not-found response for inactive or unknown users.</returns>
    [HttpGet]
    [HttpGet("{username}")]
    public async Task<IActionResult> Index(string? username)
    {
        ApplicationUser? user = string.IsNullOrWhiteSpace(username)
            ? await this.UserManager.GetUserAsync(this.User)
            : await this.UserManager.FindByNameAsync(username);

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
