using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Modules.Blog.Administration.Hooks;

/// <summary>Adds the blog module's administration views to MVC view lookup.</summary>
public sealed class BlogAdministrationViewLocationHook : IViewLocationExpandHook
{
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        if (string.Equals(context.AreaName, "Administration", StringComparison.OrdinalIgnoreCase)
            && string.Equals(context.ControllerName, "BlogManagement", StringComparison.OrdinalIgnoreCase))
        {
            newLocations.Add(item: "/Administration/Views/{1}/{0}.cshtml");
            newLocations.Add("/Administration/Views/Shared/{0}.cshtml");
        }

        return Task.CompletedTask;
    }
}
