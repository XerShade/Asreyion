using Asreyion.Core.Features.Hooks.Interfaces;

namespace Asreyion.Core.Features.Hooks.Hooks;

/// <summary>
/// Defines a contract for a frontend hook for injecting HTML/Assets.
/// </summary>
public interface IOnRenderHtmlHook : IHookSubscriber
{
    /// <summary>
    /// Gets the name of the hook point where this subscriber contributes markup.
    /// </summary>
    /// <remarks>A kebab-case case formatted name, example: "admin-settings-sidebar"</remarks>
    string HookName { get; }
    /// <summary>
    /// Creates the markup for this hook point.
    /// </summary>
    /// <returns>The HTML fragment to render, or an empty string when there is no output.</returns>
    Task<string> RenderHtmlAsync();
}
