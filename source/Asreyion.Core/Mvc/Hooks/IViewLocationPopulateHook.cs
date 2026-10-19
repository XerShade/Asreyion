using Asreyion.Core.Hooks.Interfaces;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Asreyion.Core.Mvc.Hooks;

/// <summary>
/// Adds view locations before module-specific expansion is applied.
/// </summary>
public interface IViewLocationPopulateHook : IHookSubscriber
{
    /// <summary>
    /// Adds view search locations to the current lookup context.
    /// </summary>
    /// <param name="context">The current view lookup context.</param>
    Task OnPopulateAsync(ViewLocationExpanderContext context);
}
