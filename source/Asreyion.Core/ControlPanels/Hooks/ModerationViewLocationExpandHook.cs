using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Modules.Blog.Moderation.Hooks;

/// <summary>
/// Adds the blog module's administration views to MVC view lookup.
/// </summary>
[HookName("Moderation Panel View Location Expander")]
[HookDescription("Adds moderation panel view folders to MVC lookup.")]
public sealed class ModerationViewLocationExpandHook : IViewLocationExpandHook
{
    /// <inheritdoc />
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        if (string.Equals(context.AreaName, "Moderation", StringComparison.OrdinalIgnoreCase))
        {
            newLocations.Add(item: "/Moderation/Views/{1}/{0}.cshtml");
            newLocations.Add("/Moderation/Views/Shared/{0}.cshtml");
        }

        return Task.CompletedTask;
    }
}
