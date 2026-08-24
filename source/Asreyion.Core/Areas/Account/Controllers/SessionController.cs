using Asreyion.Core.Areas.Account.Models;
using Asreyion.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Asreyion.Core.Areas.Account.Controllers;

[Area("Account")]
public class SessionController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly SignInManager<ApplicationUser> SignInManager = signInManager;
    private readonly UserManager<ApplicationUser> UserManager = userManager;

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
            return this.RedirectToLocal(model.ReturnUrl);
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

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(
        string provider,
        string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return this.RedirectToAction(
                nameof(this.Login),
                new { returnUrl });
        }

        string? callbackUrl = this.Url.Action(
            nameof(this.ExternalLoginCallback),
            "Session",
            new
            {
                area = "Account",
                returnUrl
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
    
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(
        string? returnUrl = null,
        string? remoteError = null)
    {
        if (!string.IsNullOrWhiteSpace(remoteError))
        {
            this.TempData["ExternalLoginError"] =
                $"External authentication failed: {remoteError}";

            return this.RedirectToAction(
                nameof(this.Login),
                new { returnUrl });
        }

        ExternalLoginInfo? info =
            await this.SignInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            this.TempData["ExternalLoginError"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(
                nameof(this.Login),
                new { returnUrl });
        }

        SignInResult result =
            await this.SignInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: true);

        if (result.Succeeded)
        {
            return this.RedirectToLocal(returnUrl);
        }

        if (result.IsLockedOut)
        {
            this.TempData["ExternalLoginError"] =
                "This account is currently locked.";

            return this.RedirectToAction(
                nameof(this.Login),
                new { returnUrl });
        }

        return this.View(
            "ExternalLoginConfirmation",
            new ExternalLoginConfirmationViewModel
            {
                ReturnUrl = returnUrl,
                Provider = info.LoginProvider
            });
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

    private IActionResult RedirectToLocal(string? returnUrl) 
        => !string.IsNullOrWhiteSpace(returnUrl) &&
            this.Url.IsLocalUrl(returnUrl)
            ? this.Redirect(returnUrl)
            : this.RedirectToAction(
            "Index",
            "Home",
            new { area = "" });
}