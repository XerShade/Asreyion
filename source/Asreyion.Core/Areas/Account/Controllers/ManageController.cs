using Asreyion.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Controllers;

[Authorize, Area("Account")]
public class ManageController(UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;

    public async Task<IActionResult> Index()
    {
        ApplicationUser? user = await this.UserManager.GetUserAsync(this.User);
        return this.View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ApplicationUser model)
    {
        ApplicationUser? user = await this.UserManager.GetUserAsync(this.User);

        if (user != null)
        {
            user.DisplayName = model.DisplayName;
            _ = await this.UserManager.UpdateAsync(user);
        }

        return this.RedirectToAction("Index");
    }
}