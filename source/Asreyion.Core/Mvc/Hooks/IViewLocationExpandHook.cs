using Asreyion.Core.Features.Hooks.Interfaces;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Mvc.Hooks;

public interface IViewLocationExpandHook : IHookSubscriber
{
    Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder);
}