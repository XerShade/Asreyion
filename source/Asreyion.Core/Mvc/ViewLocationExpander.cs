using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Mvc;

public class ViewLocationExpander : IViewLocationExpander
{
    private static readonly string[] ViewRootFolders = ["Features", "Content"];

    public virtual void PopulateValues(ViewLocationExpanderContext context)
    {
        _ = context.Values["area"] = context.AreaName ?? string.Empty;

        context.Values["feature"] = context.ActionContext.RouteData.Values.TryGetValue("feature", out object? featureObj) &&
            featureObj is string featureName &&
            !string.IsNullOrWhiteSpace(featureName)
            ? featureName
            : string.Empty;
    }

    public virtual IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        _ = context.Values.TryGetValue("area", out string? areaName);

        bool hasArea = !string.IsNullOrEmpty(areaName);
        string? targetAreaFolder = hasArea ? areaName : "{1}";

        List<string> newLocations = [];

        // Dynamically iterate over every configured root folder path
        foreach (string root in ViewRootFolders)
        {
            if (hasArea)
            {
                // Structural pattern: /{Root}/{Area}/Views/{Controller}/{Action}.cshtml
                newLocations.Add($"/{root}/{targetAreaFolder}/Views/{{1}}/{{0}}.cshtml");
                newLocations.Add($"/{root}/{targetAreaFolder}/Views/Shared/{{0}}.cshtml");
                newLocations.Add($"/{root}/{targetAreaFolder}/Views/{{0}}.cshtml");
            }
            else
            {
                // Fallback layout pattern when there is no Area detected
                newLocations.Add($"/{root}/Views/{{1}}/{{0}}.cshtml");
                newLocations.Add($"/{root}/Views/Shared/{{0}}.cshtml");
            }
        }

        if(hasArea)
        {
            // Structural pattern: /Views/{Area}/{Controller}/{Action}.cshtml
            newLocations.Add($"/Views/{targetAreaFolder}/{{1}}/{{0}}.cshtml");
            newLocations.Add($"/Views/{targetAreaFolder}/Shared/{{0}}.cshtml");
        }
        else
        {
            // Fallback layout pattern when there is no Area detected
            newLocations.Add($"/Views/{{1}}/{{0}}.cshtml");
            newLocations.Add($"/Views/Shared/{{0}}.cshtml");
        }

        // Return our custom paths first, then fall back to the default convention paths
        return newLocations.Concat(viewLocations);
    }
}