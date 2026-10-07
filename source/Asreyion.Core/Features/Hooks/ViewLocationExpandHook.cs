using Asreyion.Core.Mvc.Hooks;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asreyion.Core.Features.Hooks;

public class ViewLocationExpandHook : IViewLocationExpandHook
{
    public Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder)
    {
        newLocations.Add($"/Features/{targetFolder}/Views/{{1}}/{{0}}.cshtml");
        newLocations.Add($"/Features/{targetFolder}/Views/Shared/{{0}}.cshtml");
        newLocations.Add($"/Features/{targetFolder}/Views/{{0}}.cshtml");

        return Task.CompletedTask;
    }
}