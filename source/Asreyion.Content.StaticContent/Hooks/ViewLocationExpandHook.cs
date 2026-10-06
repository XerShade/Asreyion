using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Content.StaticContent.Hooks;

public class ViewLocationExpandHook : IViewLocationExpandHook
{
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        newLocations.Add($"/Content/Views/{{1}}/{{0}}.cshtml");
        newLocations.Add($"/Content/Views/Shared/{{0}}.cshtml");
        newLocations.Add($"/Content/Views/{{0}}.cshtml");

        return Task.CompletedTask;
    }
}