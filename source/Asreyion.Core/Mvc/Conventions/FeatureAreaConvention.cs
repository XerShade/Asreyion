using Asreyion.Core.Mvc.Attributes;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;

namespace Asreyion.Core.Mvc.Conventions;

public class FeatureAreaConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        // 1. Check for explicit Cross-Feature Attribute assignments
        var featureAreaAttr = controller.ControllerType.GetCustomAttribute<FeatureAreaAttribute>();
        if (featureAreaAttr != null)
        {
            controller.RouteValues["area"] = featureAreaAttr.AreaName;
            controller.RouteValues["feature"] = featureAreaAttr.FeatureName;
            return;
        }

        // 2. Process via Namespace
        var controllerNamespace = controller.ControllerType.Namespace;
        if (string.IsNullOrEmpty(controllerNamespace))
            return;

        // Strip out "Controllers" from the namespace parts array entirely
        var parts = controllerNamespace.Split('.')
            .Where(p => !p.Equals("Controllers", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        int featuresIndex = Array.IndexOf(parts, "Features");

        if (featuresIndex != -1)
        {
            // Case A: Highly Nested Area Component (e.g., Asreyion.Core.Features.Account.Billing)
            if (parts.Length > featuresIndex + 2)
            {
                controller.RouteValues["area"] = parts[featuresIndex + 1];
                controller.RouteValues["feature"] = parts[featuresIndex + 2];
            }
            // Case B: Direct / Standalone Feature (e.g., Asreyion.Core.Features.Authentication)
            else if (parts.Length > featuresIndex + 1)
            {
                controller.RouteValues["area"] = parts[featuresIndex + 1];
                controller.RouteValues["feature"] = null; // Forces ViewExpander fallback to {1}
            }
        }
    }
}
