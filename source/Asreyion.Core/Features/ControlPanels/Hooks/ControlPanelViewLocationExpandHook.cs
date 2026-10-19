using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Features.ControlPanels.Hooks;

/// <summary>
/// Adds administration, account, and moderation view folders to MVC lookup.
/// </summary>
public class ControlPanelViewLocationExpandHook : IViewLocationExpandHook
{
    /// <inheritdoc />
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        newLocations.Add($"/Features/ControlPanels/{targetFolder}/Views/{{1}}/{{0}}.cshtml");
        newLocations.Add($"/Features/ControlPanels/{targetFolder}/Views/Shared/{{0}}.cshtml");
        newLocations.Add($"/Features/ControlPanels/{targetFolder}/Views/{{0}}.cshtml");

        return Task.CompletedTask;
    }
}
