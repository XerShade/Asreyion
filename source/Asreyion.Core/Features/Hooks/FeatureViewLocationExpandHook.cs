using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Features.Hooks;

/// <summary>
/// Adds the shared feature view folders to MVC view lookup.
/// </summary>
public class FeatureViewLocationExpandHook : IViewLocationExpandHook
{
    /// <inheritdoc />
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        newLocations.Add($"/Features/{targetFolder}/Views/{{1}}/{{0}}.cshtml");
        newLocations.Add($"/Features/{targetFolder}/Views/Shared/{{0}}.cshtml");
        newLocations.Add($"/Features/{targetFolder}/Views/{{0}}.cshtml");

        return Task.CompletedTask;
    }
}
