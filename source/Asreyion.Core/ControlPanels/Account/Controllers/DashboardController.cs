using Asreyion.Core.ControlPanels.Account.Models;
using Asreyion.Core.Authentication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.ControlPanels.Account.Controllers;

/// <summary>
/// Manages account settings and connected external sign-in providers.
/// </summary>
/// <param name="userManager">Reads and updates the signed-in user's account.</param>
/// <param name="signInManager">Starts and ends the user's authentication session.</param>
[Area("Account"), Authorize]
public class DashboardController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;
    private readonly SignInManager<ApplicationUser> SignInManager = signInManager;

    /// <summary>
    /// Displays account details and linked providers.
    /// </summary>
    /// <returns>The account dashboard view.</returns>
    [Route("account/dashboard")]
    public async Task<IActionResult> Index()
    {
        ApplicationUser? user = await this.UserManager.GetUserAsync(this.User);

        if (user is null)
        {
            return this.NotFound();
        }

        IList<UserLoginInfo> externalLogins = await this.UserManager.GetLoginsAsync(user);

        DashboardViewModel model = new()
        {
            DisplayName = user.DisplayName,
            Email = user.Email ?? throw new NullReferenceException("Email cannot be null."),
            UserName = user.UserName ?? throw new NullReferenceException("UserName cannot be null."),
            ExternalLogins = externalLogins
        };

        return this.View(model);
    }

    /// <summary>
    /// Updates the user's profile settings.
    /// </summary>
    /// <param name="model">The submitted account settings.</param>
    /// <returns>The dashboard view with validation results.</returns>
    [HttpPost, ValidateAntiForgeryToken, Route("account/settings/update")]
    public async Task<IActionResult> Update(DashboardViewModel model)
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

        IList<UserLoginInfo> externalLogins = await this.UserManager.GetLoginsAsync(user);
        model.ExternalLogins = externalLogins;

        return this.View("Index", model);
    }

    /// <summary>
    /// Deactivates the signed-in user's account.
    /// </summary>
    /// <returns>A redirect after deactivation, or a not-found response.</returns>
    [HttpPost, ValidateAntiForgeryToken, Route("account/deactivate")]
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

    /// <summary>
    /// Starts the process of connecting an external sign-in provider.
    /// </summary>
    /// <param name="provider">The configured provider name.</param>
    /// <returns>A provider challenge or a redirect to the dashboard.</returns>
    [HttpPost, ValidateAntiForgeryToken, Route("account/authentication/external-login/connect")]
    public IActionResult ConnectExternalLogin(string provider)
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        string? callbackUrl = this.Url.Action(
            nameof(this.ConnectExternalLoginCallback),
            "Dashboard",
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

    /// <summary>
    /// Completes an external provider connection.
    /// </summary>
    /// <param name="remoteError">An error reported by the provider, if any.</param>
    /// <returns>A redirect to the account dashboard.</returns>
    [HttpGet, Route("account/authentication/external-login/callback")]
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

        Microsoft.AspNetCore.Identity.ExternalLoginInfo? info =
            await this.SignInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            this.TempData["ErrorMessage"] =
                "Unable to retrieve external login information.";

            return this.RedirectToAction(nameof(this.Index));
        }

        IList<UserLoginInfo> existingLogins = await this.UserManager.GetLoginsAsync(user);
        bool hasProvider = existingLogins.Any(l => l.LoginProvider == info.LoginProvider);

        if (hasProvider)
        {
            this.TempData["ErrorMessage"] =
                $"You already have a {info.LoginProvider} account connected.";

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

    /// <summary>
    /// Disconnects an external provider from the signed-in account.
    /// </summary>
    /// <param name="provider">The provider name.</param>
    /// <param name="providerKey">The provider's account key.</param>
    /// <returns>A redirect to the dashboard with the operation result.</returns>
    [HttpPost, ValidateAntiForgeryToken, Route("account/authentication/external-login/disconnect")]
    public async Task<IActionResult> DisconnectExternalLogin(string provider, string providerKey)
    {
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(providerKey))
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        ApplicationUser? user =
            await this.UserManager.GetUserAsync(this.User);

        if (user == null)
        {
            return this.NotFound();
        }

        IList<UserLoginInfo> logins = await this.UserManager.GetLoginsAsync(user);

        if (logins.Count == 1 && await this.UserManager.HasPasswordAsync(user) == false)
        {
            this.TempData["ErrorMessage"] =
                "You cannot disconnect your only external login without first setting a password.";

            return this.RedirectToAction(nameof(this.Index));
        }

        IdentityResult result =
            await this.UserManager.RemoveLoginAsync(user, provider, providerKey);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                this.TempData["ErrorMessage"] = error.Description;
                break;
            }

            return this.RedirectToAction(nameof(this.Index));
        }

        this.TempData["SuccessMessage"] =
            $"{provider} has been disconnected from your account.";

        return this.RedirectToAction(nameof(this.Index));
    }
}
