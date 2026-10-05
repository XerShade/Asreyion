using Asreyion.Core.Features.Hooks.Interfaces;
using Asreyion.Core.Features.Navigation.Builder;

namespace Asreyion.Core.Features.Navigation.Hooks;

/// <summary>
/// Defines a contract for a hook that contributes navigation items to menus.
/// </summary>
public interface IOnBuildNavigationHook : IHookSubscriber
{
    /// <summary>
    /// Builds navigation items for the given menu builder.
    /// </summary>
    /// <param name="builder">The navigation menu builder.</param>
    Task BuildNavigationAsync(INavigationBuilder builder);
}

