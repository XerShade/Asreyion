using Asreyion.Core.Areas.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Controllers;

[Authorize, Area("Account")]
public class ProfileController(UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;

    public async Task<IActionResult> Index()
    {
        ApplicationUser? user = await this.UserManager.GetUserAsync(this.User);
        return this.View(user);
    }
}