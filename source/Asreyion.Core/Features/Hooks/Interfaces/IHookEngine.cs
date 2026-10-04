namespace Asreyion.Core.Features.Hooks.Interfaces;

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
    /// <param name="hookPointName">The name of the hook point.</param>
    /// <returns>A collection of HTML snippets to inject.</returns>
    Task<List<string>> RenderHtmlPointsAsync(string hookPointName);
}