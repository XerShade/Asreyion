using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Modules.Blog.Account.Hooks;

/// <summary>
/// Adds the blog module's administration views to MVC view lookup.
/// </summary>
[HookName("Account Panel View Location Expander")]
[HookDescription("Adds administation panel view folders to MVC lookup.")]
public sealed class AccountViewLocationExpandHook : IViewLocationExpandHook
{
    /// <inheritdoc />
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        if (string.Equals(context.AreaName, "Account", StringComparison.OrdinalIgnoreCase))
        {
            newLocations.Add(item: "/Account/Views/{1}/{0}.cshtml");
            newLocations.Add("/Account/Views/Shared/{0}.cshtml");
        }

        return Task.CompletedTask;
    }
}
