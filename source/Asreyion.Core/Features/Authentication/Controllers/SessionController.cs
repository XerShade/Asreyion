using Asreyion.Core.Features.Authentication.Models;
using Asreyion.Core.Features.Database.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Asreyion.Core.Features.Authentication.Controllers;

public class SessionController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly SignInManager<ApplicationUser> SignInManager = signInManager;
    private readonly UserManager<ApplicationUser> UserManager = userManager;

    [HttpGet, AllowAnonymous]
    public IActionResult Index() 
        => this.RedirectToAction("Login");

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
                area = "Authentication",
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

        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
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

        string? email = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        string? displayName = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ??
                             info.Principal.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;

        return this.View(
            "ExternalLoginConfirmation",
            new ExternalLoginConfirmationViewModel
            {
                ReturnUrl = returnUrl,
                Provider = info.LoginProvider,
                Email = email ?? string.Empty,
                DisplayName = displayName ?? string.Empty
            });
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
            await this.SignInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            this.TempData["ExternalLoginError"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(nameof(this.Login));
        }

        ApplicationUser user = new()
        {
            UserName = model.UserName,
            Email = model.Email,
            DisplayName = model.DisplayName,
            IsActive = true
        };

        IdentityResult result =
            await this.UserManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        result = await this.UserManager.AddLoginAsync(user, info);

        if (!result.Succeeded)
        {
            await this.UserManager.DeleteAsync(user);

            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        await this.SignInManager.SignInAsync(user, isPersistent: true);

        return this.RedirectToLocal(model.ReturnUrl);
    }

    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> ExternalLoginLinking(string? returnUrl = null)
    {
        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
            await this.SignInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            this.TempData["ExternalLoginError"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(nameof(this.Login));
        }

        return this.View("ExternalLoginLinking", new ExternalLoginLinkingViewModel
        {
            Provider = info.LoginProvider,
            ReturnUrl = returnUrl
        });
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginLinking(ExternalLoginLinkingViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
            await this.SignInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            this.TempData["ExternalLoginError"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(nameof(this.Login));
        }

        ApplicationUser? existingUser = await this.UserManager.FindByNameAsync(model.UserName);

        if (existingUser == null)
        {
            this.ModelState.AddModelError(string.Empty, "User not found. Please check your username.");
            return this.View(model);
        }

        bool passwordValid = await this.UserManager.CheckPasswordAsync(existingUser, model.Password);

        if (!passwordValid)
        {
            this.ModelState.AddModelError(string.Empty, "Invalid password.");
            return this.View(model);
        }

        IdentityResult result = await this.UserManager.AddLoginAsync(existingUser, info);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        await this.SignInManager.SignInAsync(existingUser, isPersistent: true);

        this.TempData["SuccessMessage"] =
            $"{info.LoginProvider} has been successfully linked to your account.";

        return this.RedirectToLocal(model.ReturnUrl);
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