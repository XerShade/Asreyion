using Asreyion.Core.Areas.Account.Models;
using Asreyion.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Account.Controllers;

[Area("Account"), AllowAnonymous]
public class RegisterController(UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;

    [HttpGet]
    public IActionResult Index()
        => this.View(new RegisterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(RegisterViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        /* Temporarily disabled while security concerns are addressed and middleware is implemented. */
        return this.View(new RegisterViewModel());

        /*
        ApplicationUser user = new()
        {
            UserName = model.UserName,
            Email = model.Email,
            DisplayName = model.DisplayName,
            IsActive = true
        };

        IdentityResult result = await this.UserManager.CreateAsync(user, model.Password!);

        if (result.Succeeded)
        {
            this.TempData["SuccessMessage"] =
                "Your account has been created successfully.";

            return this.RedirectToAction(nameof(this.Index));
        }

        foreach (IdentityError error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }

        return this.View(model);
        */
    }
}