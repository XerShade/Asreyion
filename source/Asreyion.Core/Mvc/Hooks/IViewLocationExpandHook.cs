using Asreyion.Core.Features.Hooks.Interfaces;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Mvc.Hooks;

/// <summary>
/// Adds view search locations for a feature or module.
/// </summary>
public interface IViewLocationExpandHook : IHookSubscriber
{
    /// <summary>
    /// Adds candidate view paths for the requested view location.
    /// </summary>
    /// <param name="context">The current view lookup context.</param>
    /// <param name="newLocations">The ordered search locations to extend.</param>
    /// <param name="targetFolder">The feature folder used to construct view paths.</param>
    Task OnExpandAsync(ViewLocationExpanderContext context, List<string> newLocations, string targetFolder);
}
