using Asreyion.Core.Features.Hooks.Definitions;
using Asreyion.Core.Features.Hooks.Interfaces;

namespace Asreyion.Core.Features.Hooks;

/// <summary>
/// Defines a hook engine used to execute hooks within a scope during the application lifecycle.
/// </summary>
/// <param name="serviceProvider">The service provider to use when creating scopes.</param>
public class HookEngine(IServiceProvider serviceProvider) : IHookEngine
{
    /// <summary>
    /// Gets a reference to the service provider to use when creating scopes.
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = serviceProvider;

    /// <inheritdoc />
    public async Task ExecuteAsync<THook>(Func<THook, Task> action) where THook : IHookSubscriber
    {
        // Create a scope to use the service provider.
        using IServiceScope scope = this.ServiceProvider.CreateScope();

        // Get the hooks registered in the scope.
        IEnumerable<THook> hooks = scope.ServiceProvider.GetServices<THook>();

        // Iterate over the hooks and execute the action.
        foreach (THook hook in hooks)
        {
            // Execute the action.
            await action(hook);
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> RenderHtmlPointsAsync(string hookName)
    {
        // Create a scope to use the service provider.
        using IServiceScope scope = this.ServiceProvider.CreateScope();

        // Get the hooks that inherit from IOnRenderHtmlHook registered in the scope.
        IEnumerable<IOnRenderHtmlHook> hooks = scope.ServiceProvider.GetServices<IOnRenderHtmlHook>();

        // Iterate over the hooks and execute the action.
        List<string> outputs = [];
        foreach (IOnRenderHtmlHook? hook in hooks.Where(h => h.HookName == hookName))
        {
            // Execute the action and get the HTML.
            string html = await hook.RenderHtmlAsync();

            // Validate the HTML.
            if (!string.IsNullOrEmpty(html))
            {
                // Add the HTML to the list.
                outputs.Add(html);
            }
        }

        // Return the HTML outputs.
        return outputs;
    }
}