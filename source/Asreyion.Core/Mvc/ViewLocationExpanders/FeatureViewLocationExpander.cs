using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Mvc.ViewLocationExpanders;

public class FeatureViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        context.Values["area"] = context.AreaName ?? string.Empty;

        if (context.ActionContext.RouteData.Values.TryGetValue("feature", out var featureObj) &&
            featureObj is string featureName &&
            !string.IsNullOrWhiteSpace(featureName))
        {
            context.Values["feature"] = featureName;
        }
        else
        {
            context.Values["feature"] = string.Empty;
        }
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        context.Values.TryGetValue("area", out string? areaName);
        context.Values.TryGetValue("feature", out string? featureName);

        // Fall back to the Area Name if it's a standalone feature folder
        string targetAreaFolder = !string.IsNullOrEmpty(areaName) ? areaName : "{1}";

        var newLocations = new List<string>();

        if (!string.IsNullOrEmpty(areaName))
        {
            // Pattern requested: Features/{Area}/Views/{Controller}/{Action}.cshtml
            newLocations.Add($"/Features/{targetAreaFolder}/Views/{{1}}/{{0}}.cshtml");
            newLocations.Add($"/Features/{targetAreaFolder}/Views/Shared/{{0}}.cshtml");
            newLocations.Add($"/Features/{targetAreaFolder}/{{0}}.cshtml");
        }
        else
        {
            // Fallback layout when there is no Area at all
            newLocations.Add($"/Features/{targetAreaFolder}/Views/{{0}}.cshtml");
            newLocations.Add($"/Features/Shared/{{0}}.cshtml");
        }

        return newLocations.Concat(viewLocations);
    }
}
