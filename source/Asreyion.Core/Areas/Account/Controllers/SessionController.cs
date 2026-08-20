using Asreyion.Core.Areas.Account.Models;
using Asreyion.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Asreyion.Core.Areas.Account.Controllers;

[Area("Account")]
public class SessionController(
    SignInManager<ApplicationUser> signInManager) : Controller
{
    private readonly SignInManager<ApplicationUser> SignInManager = signInManager;

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        LoginViewModel model = new()
        {
            ReturnUrl = returnUrl
        };

        return this.View(model);
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        SignInResult result = await this.SignInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return !string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                this.Url.IsLocalUrl(model.ReturnUrl)
                ? this.Redirect(model.ReturnUrl)
                : this.RedirectToAction(
                "Index",
                "Home",
                new { area = "" });
        }

        if (result.IsLockedOut)
        {
            this.ModelState.AddModelError(
                string.Empty,
                "This account is currently locked.");
        }
        else
        {
            this.ModelState.AddModelError(
                string.Empty,
                "Invalid username or password.");
        }

        return this.View(model);
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await this.SignInManager.SignOutAsync();

        return this.RedirectToAction(
            "Index",
            "Home",
            new { area = "" });
    }
}