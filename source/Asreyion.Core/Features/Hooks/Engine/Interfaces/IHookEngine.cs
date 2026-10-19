using Asreyion.Core.Features.Hooks.Interfaces;

namespace Asreyion.Core.Features.Hooks.Engine.Interfaces;

/// <summary>
/// Defines a contract for a hook engine.
/// </summary>
public interface IHookEngine
{
    /// <summary>
    /// Executes all registered backend hooks for a specific action sequentially.
    /// </summary>
    /// <typeparam name="THook">The type of the hook to execute.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task result representing the asynchronous operation.</returns>
    Task ExecuteAsync<THook>(Func<THook, Task> action) where THook : IHookSubscriber;
    /// <summary>
    /// Fetches and aggregates all UI snippets registered to a unique hook point
    /// </summary>
    /// <param name="hookName">The name of the hook point.</param>
    /// <returns>Non-empty HTML snippets returned by matching subscribers.</returns>
    Task<List<string>> RenderHtmlPointsAsync(string hookName);
}
