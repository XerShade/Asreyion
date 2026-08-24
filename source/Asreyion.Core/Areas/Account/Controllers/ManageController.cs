using Asreyion.Core.Areas.Account.Models;
using Asreyion.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Controllers;

[Area("Account"), Authorize]
public class ManageController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;
    private readonly SignInManager<ApplicationUser> SignInManager = signInManager;

    public async Task<IActionResult> Index()
    {
        ApplicationUser? user = await this.UserManager.GetUserAsync(this.User);

        if (user is null)
        {
            return this.NotFound();
        }

        ManageViewModel model = new()
        {
            DisplayName = user.DisplayName,
            Email = user.Email ?? throw new NullReferenceException("Email cannot be null."),
            UserName = user.UserName ?? throw new NullReferenceException("UserName cannot be null.")
        };

        return this.View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ManageViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View("Index", model);
        }

        ApplicationUser? user = await this.UserManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound();
        }

        user.DisplayName = model.DisplayName ?? throw new NullReferenceException("DisplayName cannot be null.");

        IdentityResult result = await this.UserManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            this.TempData["SuccessMessage"] =
                "Your account settings have been updated successfully.";

            return this.RedirectToAction(nameof(this.Index));
        }

        foreach (IdentityError error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }

        return this.View("Index", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate()
    {
        ApplicationUser? user =
            await this.UserManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound();
        }

        user.IsActive = false;
        user.DeactivatedAt = DateTime.UtcNow;

        IdentityResult result =
            await this.UserManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.TempData["ErrorMessage"] = error.Description;
                break;
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        _ = await this.UserManager.UpdateSecurityStampAsync(user);

        this.TempData["SuccessMessage"] =
            "Your account has been deactivated.";

        return this.RedirectToAction(nameof(this.Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult ConnectExternalLogin(string provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        string? callbackUrl = this.Url.Action(
            nameof(this.ConnectExternalLoginCallback),
            "Manage",
            new
            {
                area = "Account"
            });

        if (string.IsNullOrWhiteSpace(callbackUrl))
        {
            return this.StatusCode(500);
        }

        Microsoft.AspNetCore.Authentication.AuthenticationProperties properties =
            this.SignInManager.ConfigureExternalAuthenticationProperties(
                provider,
                callbackUrl);

        return this.Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ConnectExternalLoginCallback(
    string? remoteError = null)
    {
        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            this.TempData["ErrorMessage"] =
                $"Unable to connect external account: {remoteError}";

            return this.RedirectToAction(nameof(this.Index));
        }

        ApplicationUser? user =
            await this.UserManager.GetUserAsync(this.User);

        if (user is null)
        {
            return this.Challenge();
        }

        ExternalLoginInfo? info =
            await this.SignInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            this.TempData["ErrorMessage"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(nameof(this.Index));
        }

        IdentityResult result =
            await this.UserManager.AddLoginAsync(user, info);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            this.TempData["ErrorMessage"] =
                "Unable to connect the external account.";

            return this.RedirectToAction(nameof(this.Index));
        }

        this.TempData["SuccessMessage"] =
            $"{info.LoginProvider} has been connected to your account.";

        return this.RedirectToAction(nameof(this.Index));
    }
}