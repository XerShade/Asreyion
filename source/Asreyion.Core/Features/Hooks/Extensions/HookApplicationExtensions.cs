using Asreyion.Core.Features.Hooks.Hooks;

namespace Asreyion.Core.Features.Hooks.Extensions;

/// <summary>
/// Provides extension methods for executing application middleware and pipeline configuration hooks.
/// </summary>
public static class HookApplicationExtensions
{
    /// <summary>
    /// Resolves and executes all registered application configuration hooks to set up the request pipeline and middleware.
    /// </summary>
    /// <param name="application">The built <see cref="WebApplication"/> instance used to configure middleware and resolve services.</param>
    /// <returns>The same <see cref="WebApplication"/> instance for fluent chaining.</returns>
    public static WebApplication ConfigureApplicationWithHooks(this WebApplication application)
    {
        // Create a scope for the mini bootstrap container
        using IServiceScope scope = application.Services.CreateScope();

        // Resolve all registered IConfigureApplicationHook instances from the built DI container.
        IEnumerable<IConfigureApplicationHook> hooks = scope.ServiceProvider.GetServices<IConfigureApplicationHook>();

        // Iterate over each discovered hook sequentially.
        foreach (IConfigureApplicationHook hook in hooks)
        {
            // Execute the hook's configuration logic, passing in the WebApplication (for pipeline setup)
            // and the current web hosting environment context.
            hook.OnConfigureApplication(application, application.Environment);
        }

        // Return the application instance to allow for a fluent setup style in Program.cs
        return application;
    }
}
