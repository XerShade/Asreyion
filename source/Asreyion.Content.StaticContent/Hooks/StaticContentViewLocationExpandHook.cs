using Asreyion.Core.Hooks.Attributes;
using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Content.StaticContent.Hooks;

/// <summary>
/// Adds the static content module's view folder to MVC lookup.
/// </summary>
[HookName("Static Content View Location Expander")]
[HookDescription("Adds the static content module's view folder to MVC lookup.")]
public class StaticContentViewLocationExpandHook : IViewLocationExpandHook
{
    /// <inheritdoc />
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        newLocations.Add($"/Content/Views/{{1}}/{{0}}.cshtml");
        newLocations.Add($"/Content/Views/Shared/{{0}}.cshtml");
        newLocations.Add($"/Content/Views/{{0}}.cshtml");

        return Task.CompletedTask;
    }
}
