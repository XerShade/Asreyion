using Asreyion.Core.Features.Hooks.Interfaces;

namespace Asreyion.Core.Features.Hooks;

/// <summary>
/// Defines a contract for a frontend hook for injecting HTML/Assets.
/// </summary>
public interface IOnRenderHtmlHook : IHookSubscriber
{
    /// <summary>
    /// The name of the hook point.
    /// </summary>
    /// <remarks>A kebab-case case formatted name, example: "admin-settings-sidebar"</remarks>
    string HookName { get; }
    /// <summary>
    /// Renders the HTML to inject.
    /// </summary>
    /// <returns>A string of HTML.</returns>
    Task<string> RenderHtmlAsync();
}