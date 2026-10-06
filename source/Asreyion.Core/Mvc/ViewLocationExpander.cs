using Asreyion.Core.Features.Hooks.Interfaces;
using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Mvc;

public class ViewLocationExpander : IViewLocationExpander
{
    /// <summary>
    /// Gets a reference to the hook engine.
    /// </summary>
    private IHookEngine? HookEngine { get; set; } = default!;

    /// <inheritdoc />
    public virtual void PopulateValues(ViewLocationExpanderContext context)
    {
        // Acquire the hook engine if it hasn't been acquired yet.
        this.HookEngine ??= context.ActionContext.HttpContext.RequestServices.GetRequiredService<IHookEngine>();

        // Execute the hook, and wait for it to complete.
        Task hookTask = this.HookEngine.ExecuteAsync<IViewLocationPopulateHook>(h => h.OnPopulateAsync(context));
        while (!hookTask.IsCompleted)
        {
            // Do nothing, we just 
        }
    }

    /// <inheritdoc />
    public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        // Grab some useful information.
        string? areaName = context.AreaName ?? string.Empty;
        bool hasArea = !string.IsNullOrEmpty(areaName);
        string? targetFolder = hasArea ? areaName : "{1}";

        // Create a new list of view locations.
        List<string> newLocations = [];

        // Check to see if this is inside an Area.
        if(hasArea)
        {
            // Structural pattern: /Views/{Area}/{Controller}/{Action}.cshtml
            newLocations.Add($"/Areas/{targetFolder}/Views/{{1}}/{{0}}.cshtml");
            newLocations.Add($"/Areas/{targetFolder}/Views/Shared/{{0}}.cshtml");
            newLocations.Add($"/Areas/{targetFolder}/Views/{{0}}.cshtml");
            newLocations.Add($"/Views/{targetFolder}/{{1}}/{{0}}.cshtml");
            newLocations.Add($"/Views/{targetFolder}/Shared/{{0}}.cshtml");
        }
        else
        {
            // Fallback layout pattern when there is no Area detected
            newLocations.Add($"/{targetFolder}/Views/{{1}}/{{0}}.cshtml");
            newLocations.Add($"/{targetFolder}/Views/Shared/{{0}}.cshtml");
            newLocations.Add($"/{targetFolder}/Views/{{0}}.cshtml");
            newLocations.Add($"/Views/{{1}}/{{0}}.cshtml");
            newLocations.Add($"/Views/Shared/{{0}}.cshtml");
        }

        // Acquire the hook engine if it hasn't been acquired yet.
        this.HookEngine ??= context.ActionContext.HttpContext.RequestServices.GetRequiredService<IHookEngine>();

        // Execute the hook, and wait for it to complete.
        Task hookTask = this.HookEngine.ExecuteAsync<IViewLocationExpandHook>(h => h.OnExpandAsync(context, newLocations, targetFolder));
        while (!hookTask.IsCompleted)
        {
            // Do nothing, we just 
        }

        // Return our custom paths first, then fall back to the default convention paths
        return newLocations.Concat(viewLocations);
    }
}