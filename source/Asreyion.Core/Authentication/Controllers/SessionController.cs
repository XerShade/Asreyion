using Asreyion.Core.Authentication.Models;
using Asreyion.Core.Authentication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Asreyion.Core.Authentication.Controllers;

/// <summary>
/// Handles local sign-in, external sign-in, account linking, and sign-out.
/// </summary>
/// <param name="signInManager">Coordinates authentication and sign-in operations.</param>
/// <param name="userManager">Creates and updates application users.</param>
[Area("Authentication")]
public class SessionController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    /// <summary>
    /// Redirects the authentication root to the sign-in form.
    /// </summary>
    /// <returns>A redirect to the sign-in action.</returns>
    [HttpGet, AllowAnonymous]
    public IActionResult Index() 
        => this.RedirectToAction("Login");

    /// <summary>
    /// Displays the local sign-in form.
    /// </summary>
    /// <param name="returnUrl">The local destination to visit after sign-in.</param>
    /// <returns>The sign-in view.</returns>
    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        LoginViewModel model = new()
        {
            ReturnUrl = returnUrl
        };

        return this.View(model);
    }

    /// <summary>
    /// Validates credentials and signs in a local account.
    /// </summary>
    /// <param name="model">The submitted credentials and session preference.</param>
    /// <returns>The sign-in view on failure, or a redirect after success.</returns>
    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        SignInResult result = await this._signInManager.PasswordSignInAsync(
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

    /// <summary>
    /// Starts an external sign-in challenge.
    /// </summary>
    /// <param name="provider">The configured external provider name.</param>
    /// <param name="returnUrl">The local destination to visit after sign-in.</param>
    /// <returns>A provider challenge or a redirect to the sign-in form.</returns>
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
            this._signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                callbackUrl);

        return this.Challenge(properties, provider);
    }

    /// <summary>
    /// Completes external sign-in and either signs in or requests account details.
    /// </summary>
    /// <param name="returnUrl">The local destination to visit after sign-in.</param>
    /// <param name="remoteError">An error reported by the external provider, if any.</param>
    /// <returns>A redirect or the account confirmation view.</returns>
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
            await this._signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            this.TempData["ExternalLoginError"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(
                nameof(this.Login),
                new { returnUrl });
        }

        SignInResult result =
            await this._signInManager.ExternalLoginSignInAsync(
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

    /// <summary>
    /// Creates a local account for a user authenticated by an external provider.
    /// </summary>
    /// <param name="model">The new account's profile details.</param>
    /// <returns>The confirmation view on failure, or a redirect after account creation.</returns>
    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
            await this._signInManager.GetExternalLoginInfoAsync();

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
            await this._userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        result = await this._userManager.AddLoginAsync(user, info);

        if (!result.Succeeded)
        {
            await this._userManager.DeleteAsync(user);

            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        await this._signInManager.SignInAsync(user, isPersistent: true);

        return this.RedirectToLocal(model.ReturnUrl);
    }

    /// <summary>
    /// Displays the form for connecting an external provider to an existing account.
    /// </summary>
    /// <param name="returnUrl">The local destination to visit after linking.</param>
    /// <returns>The provider linking view.</returns>
    [HttpGet, AllowAnonymous]
    public async Task<IActionResult> ExternalLoginLinking(string? returnUrl = null)
    {
        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
            await this._signInManager.GetExternalLoginInfoAsync();

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

    /// <summary>
    /// Verifies credentials and connects the external provider.
    /// </summary>
    /// <param name="model">The existing account credentials and provider.</param>
    /// <returns>The linking view on failure, or a redirect after linking.</returns>
    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginLinking(ExternalLoginLinkingViewModel model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View(model);
        }

        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
            await this._signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            this.TempData["ExternalLoginError"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(nameof(this.Login));
        }

        ApplicationUser? existingUser = await this._userManager.FindByNameAsync(model.UserName);

        if (existingUser == null)
        {
            this.ModelState.AddModelError(string.Empty, "User not found. Please check your username.");
            return this.View(model);
        }

        bool passwordValid = await this._userManager.CheckPasswordAsync(existingUser, model.Password);

        if (!passwordValid)
        {
            this.ModelState.AddModelError(string.Empty, "Invalid password.");
            return this.View(model);
        }

        IdentityResult result = await this._userManager.AddLoginAsync(existingUser, info);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }

            return this.View(model);
        }

        await this._signInManager.SignInAsync(existingUser, isPersistent: true);

        this.TempData["SuccessMessage"] =
            $"{info.LoginProvider} has been successfully linked to your account.";

        return this.RedirectToLocal(model.ReturnUrl);
    }

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    /// <returns>A redirect to the home page.</returns>
    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await this._signInManager.SignOutAsync();

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
