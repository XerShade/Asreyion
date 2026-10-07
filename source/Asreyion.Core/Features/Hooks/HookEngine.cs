using Asreyion.Core.Features.Hooks;
using Asreyion.Core.Features.Hooks.Interfaces;

namespace Asreyion.Core.Features.Hooks;

/// <summary>
/// Defines a hook engine used to execute hooks within a scope during the application lifecycle.
/// </summary>
/// <param name="serviceProvider">The service provider to use when creating scopes.</param>
/// <param name="logger">The optional logger for logging hook failures.</param>
public class HookEngine(IServiceProvider serviceProvider, ILogger<HookEngine>? logger = null) : IHookEngine
{
    /// <summary>
    /// Gets a reference to the service provider to use when creating scopes.
    /// </summary>
    private IServiceProvider ServiceProvider { get; } = serviceProvider;

    /// <summary>
    /// Gets a reference to the logger.
    /// </summary>
    private ILogger<HookEngine>? Logger { get; } = logger;

    /// <inheritdoc />
    public async Task ExecuteAsync<THook>(Func<THook, Task> action) where THook : IHookSubscriber
    {
        // Create a scope to use the service provider.
        using IServiceScope scope = this.ServiceProvider.CreateScope();

        // Get the hooks registered in the scope.
        IEnumerable<THook> hooks = scope.ServiceProvider.GetServices<THook>();

        // Iterate over the hooks and execute the action safely.
        foreach (THook hook in hooks)
        {
            try
            {
                await action(hook);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "Error executing hook subscriber {HookType}", hook.GetType().FullName);
            }
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> RenderHtmlPointsAsync(string hookName)
    {
        // Create a scope to use the service provider.
        using IServiceScope scope = this.ServiceProvider.CreateScope();

        // Get the hooks that inherit from IOnRenderHtmlHook registered in the scope.
        IEnumerable<IOnRenderHtmlHook> hooks = scope.ServiceProvider.GetServices<IOnRenderHtmlHook>();

        // Iterate over the hooks and execute the action safely.
        List<string> outputs = [];
        foreach (IOnRenderHtmlHook hook in hooks.Where(h => string.Equals(h.HookName, hookName, StringComparison.OrdinalIgnoreCase)))
        {
            try
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
            catch (Exception ex)
            {
                this.Logger?.LogError(ex, "Error rendering HTML hook point '{HookName}' from subscriber {HookType}", hookName, hook.GetType().FullName);
            }
        }

        // Return the HTML outputs.
        return outputs;
    }
}