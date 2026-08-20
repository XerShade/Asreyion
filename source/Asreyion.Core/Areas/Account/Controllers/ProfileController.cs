using Asreyion.Core.Areas.Account.Models;
using Asreyion.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Controllers;

[Area("Account")]
[Authorize]
[Route("Account/Profile")]
public class ProfileController(
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;

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